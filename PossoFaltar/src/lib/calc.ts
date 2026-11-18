import { weekdayOf } from './dates';
import type { AttendanceRecord, Subject, Weekday } from './types';

export type RiskLevel = 'safe' | 'warning' | 'danger' | 'failed';

export interface AttendanceSummary {
  /** Máximo de aulas que dá para faltar sem reprovar. */
  maxAbsences: number;
  /** Aulas faltadas até agora. */
  absences: number;
  /** Aulas assistidas até agora. */
  presences: number;
  /** Faltas que ainda podem ser usadas (negativo = reprovado). */
  remaining: number;
  /** Fração das faltas permitidas que já foi usada (0 a 1+). */
  usedRatio: number;
  /** % de presença nas aulas registradas, ou null se não há registros. */
  currentAttendance: number | null;
  /** Frequência final garantida se não faltar mais nenhuma aula. */
  guaranteedAttendance: number;
  /** Média de aulas por dia de aula (para converter faltas em "dias"). */
  classesPerMeeting: number;
  /** Quantos dias inteiros de aula ainda dá para faltar. */
  remainingMeetings: number;
  level: RiskLevel;
}

export function maxAbsencesFor(subject: Pick<Subject, 'totalClasses' | 'minAttendance'>) {
  return Math.max(
    0,
    Math.floor((subject.totalClasses * (100 - subject.minAttendance)) / 100 + 1e-9),
  );
}

export function classesPerMeeting(subject: Pick<Subject, 'schedule'>) {
  if (subject.schedule.length === 0) return 2;
  const total = subject.schedule.reduce((sum, s) => sum + s.classes, 0);
  return Math.max(1, Math.round(total / subject.schedule.length));
}

export function classesOn(subject: Subject, weekday: Weekday): number {
  return subject.schedule.find((s) => s.weekday === weekday)?.classes ?? 0;
}

function levelFor(remaining: number, maxAbsences: number, perMeeting: number): RiskLevel {
  if (remaining < 0) return 'failed';
  const usedRatio = maxAbsences === 0 ? 1 : (maxAbsences - remaining) / maxAbsences;
  if (remaining < perMeeting || usedRatio >= 0.8) return 'danger';
  if (usedRatio >= 0.5) return 'warning';
  return 'safe';
}

export function summarizeAttendance(
  subject: Subject,
  records: AttendanceRecord[],
  extraAbsences = 0,
): AttendanceSummary {
  let absences = extraAbsences;
  let presences = 0;
  for (const r of records) {
    if (r.subjectId !== subject.id) continue;
    if (r.status === 'absent') absences += r.classes;
    else presences += r.classes;
  }
  const maxAbsences = maxAbsencesFor(subject);
  const remaining = maxAbsences - absences;
  const perMeeting = classesPerMeeting(subject);
  const registered = absences - extraAbsences + presences;
  return {
    maxAbsences,
    absences,
    presences,
    remaining,
    usedRatio: maxAbsences === 0 ? (absences > 0 ? 1 : 0) : absences / maxAbsences,
    currentAttendance:
      registered > 0 ? (presences / registered) * 100 : null,
    guaranteedAttendance:
      subject.totalClasses > 0
        ? Math.max(0, ((subject.totalClasses - absences) / subject.totalClasses) * 100)
        : 0,
    classesPerMeeting: perMeeting,
    remainingMeetings: remaining > 0 ? Math.floor(remaining / perMeeting) : 0,
    level: levelFor(remaining, maxAbsences, perMeeting),
  };
}

/* ------------------------------------------------------------------ */
/* "Posso faltar?" — veredito de um dia                                */
/* ------------------------------------------------------------------ */

export interface DayClass {
  subject: Subject;
  classes: number;
  record: AttendanceRecord | undefined;
  now: AttendanceSummary;
  /** Situação caso falte nesse dia. */
  ifSkip: AttendanceSummary;
}

export type DayVerdictKind = 'free' | 'done' | 'yes' | 'careful' | 'no';

export interface DayVerdict {
  kind: DayVerdictKind;
  classes: DayClass[];
  /** Matérias que pedem atenção (careful) ou que reprovariam (no). */
  critical: DayClass[];
}

export function dayVerdict(
  date: string,
  subjects: Subject[],
  records: AttendanceRecord[],
): DayVerdict {
  const weekday = weekdayOf(date);
  const classes: DayClass[] = subjects
    .filter((s) => classesOn(s, weekday) > 0)
    .map((subject) => {
      const record = records.find((r) => r.subjectId === subject.id && r.date === date);
      const count = record?.classes ?? classesOn(subject, weekday);
      // Simulação: ignora o registro do próprio dia e soma a falta.
      const others = records.filter((r) => !(r.subjectId === subject.id && r.date === date));
      return {
        subject,
        classes: count,
        record,
        now: summarizeAttendance(subject, records),
        ifSkip: summarizeAttendance(subject, others, count),
      };
    })
    .sort((a, b) => a.ifSkip.remaining - b.ifSkip.remaining);

  if (classes.length === 0) return { kind: 'free', classes, critical: [] };

  const pending = classes.filter((c) => !c.record);
  if (pending.length === 0) return { kind: 'done', classes, critical: [] };

  const failing = pending.filter((c) => c.ifSkip.level === 'failed');
  if (failing.length > 0) return { kind: 'no', classes, critical: failing };

  const risky = pending.filter((c) => c.ifSkip.level === 'danger' || c.ifSkip.level === 'warning');
  if (risky.length > 0) return { kind: 'careful', classes, critical: risky };

  return { kind: 'yes', classes, critical: [] };
}

/* ------------------------------------------------------------------ */
/* Notas — quanto preciso tirar?                                      */
/* ------------------------------------------------------------------ */

export type GradeStatus = 'empty' | 'needs' | 'approved' | 'impossible' | 'failed';

export interface GradeSummary {
  status: GradeStatus;
  /** Média ponderada só das avaliações que já têm nota. */
  currentAverage: number | null;
  /** Média final (só quando todas as notas foram lançadas). */
  finalAverage: number | null;
  /** Nota necessária em cada avaliação pendente. */
  needed: number | null;
  pendingNames: string[];
}

export function summarizeGrades(subject: Subject): GradeSummary {
  const list = subject.assessments.filter((a) => a.weight > 0);
  if (list.length === 0) {
    return {
      status: 'empty',
      currentAverage: null,
      finalAverage: null,
      needed: null,
      pendingNames: [],
    };
  }
  const totalWeight = list.reduce((s, a) => s + a.weight, 0);
  const graded = list.filter((a) => a.grade !== null);
  const pending = list.filter((a) => a.grade === null);
  const points = graded.reduce((s, a) => s + (a.grade ?? 0) * a.weight, 0);
  const gradedWeight = graded.reduce((s, a) => s + a.weight, 0);
  const pendingWeight = totalWeight - gradedWeight;
  const currentAverage = gradedWeight > 0 ? points / gradedWeight : null;

  if (pending.length === 0) {
    const finalAverage = points / totalWeight;
    return {
      status: finalAverage + 1e-9 >= subject.passingGrade ? 'approved' : 'failed',
      currentAverage,
      finalAverage,
      needed: null,
      pendingNames: [],
    };
  }

  const rawNeeded = (subject.passingGrade * totalWeight - points) / pendingWeight;
  const needed = Math.ceil(rawNeeded * 10 - 1e-9) / 10;
  return {
    status: needed <= 0 ? 'approved' : needed > subject.maxGrade ? 'impossible' : 'needs',
    currentAverage,
    finalAverage: null,
    needed: Math.max(0, needed),
    pendingNames: pending.map((a) => a.name),
  };
}

/* ------------------------------------------------------------------ */
/* Formatação                                                          */
/* ------------------------------------------------------------------ */

/** 8.5 → "8,5" | 7 → "7,0" */
export function formatGrade(value: number): string {
  return value.toFixed(1).replace('.', ',');
}

/** 91.666 → "92%" */
export function formatPercent(value: number): string {
  return `${Math.round(value)}%`;
}

export function plural(n: number, singular: string, pluralForm = `${singular}s`) {
  return `${n} ${Math.abs(n) === 1 ? singular : pluralForm}`;
}

/** Lista legível: "A", "A e B", "A, B e C". */
export function joinNames(names: string[]): string {
  if (names.length <= 1) return names[0] ?? '';
  return `${names.slice(0, -1).join(', ')} e ${names[names.length - 1]}`;
}
