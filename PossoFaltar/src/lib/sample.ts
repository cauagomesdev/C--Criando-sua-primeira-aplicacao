import { addDays, toISODate } from './dates';
import { newId } from './id';
import type { AppData, Assessment, AttendanceRecord, ClassSlot, Subject } from './types';

interface SampleSpec {
  name: string;
  professor: string;
  color: string;
  totalClasses: number;
  schedule: ClassSlot[];
  /** Quantos dias de aula o aluno faltou no exemplo. */
  missedMeetings: number;
  passingGrade: number;
  assessments: [name: string, weight: number, grade: number | null][];
}

const SPECS: SampleSpec[] = [
  {
    name: 'Desenvolvimento Mobile',
    professor: 'Prof. Ricardo',
    color: '#5B4BF5',
    totalClasses: 80,
    schedule: [
      { weekday: 1, classes: 2 },
      { weekday: 3, classes: 2 },
    ],
    missedMeetings: 2,
    passingGrade: 6,
    assessments: [
      ['N1', 1, 8.5],
      ['N2', 1, null],
    ],
  },
  {
    name: 'Banco de Dados',
    professor: 'Profa. Juliana',
    color: '#00A3FF',
    totalClasses: 80,
    schedule: [
      { weekday: 2, classes: 2 },
      { weekday: 4, classes: 2 },
    ],
    missedMeetings: 6,
    passingGrade: 6,
    assessments: [
      ['N1', 1, 5],
      ['N2', 1, null],
    ],
  },
  {
    name: 'Cálculo II',
    professor: 'Prof. Marcos',
    color: '#FF6B6B',
    totalClasses: 60,
    schedule: [
      { weekday: 3, classes: 2 },
      { weekday: 5, classes: 2 },
    ],
    missedMeetings: 7,
    passingGrade: 6,
    assessments: [
      ['N1', 4, 3],
      ['N2', 6, null],
    ],
  },
  {
    name: 'Engenharia de Software',
    professor: 'Profa. Camila',
    color: '#12B886',
    totalClasses: 40,
    schedule: [{ weekday: 4, classes: 2 }],
    missedMeetings: 1,
    passingGrade: 6,
    assessments: [
      ['N1', 1, 9],
      ['N2', 1, 8],
    ],
  },
  {
    name: 'Inglês Técnico',
    professor: 'Prof. Daniel',
    color: '#F59F00',
    totalClasses: 40,
    schedule: [{ weekday: 5, classes: 2 }],
    missedMeetings: 3,
    passingGrade: 6,
    assessments: [
      ['N1', 1, null],
      ['N2', 1, null],
    ],
  },
];

/** Dados de exemplo relativos à data de hoje (8 semanas de semestre já cursadas). */
export function buildSampleData(today = new Date()): AppData {
  const subjects: Subject[] = [];
  const records: AttendanceRecord[] = [];
  const start = addDays(today, -56);

  SPECS.forEach((spec, index) => {
    const id = newId();
    const assessments: Assessment[] = spec.assessments.map(([name, weight, grade]) => ({
      id: newId(),
      name,
      weight,
      grade,
    }));
    subjects.push({
      id,
      name: spec.name,
      professor: spec.professor,
      color: spec.color,
      totalClasses: spec.totalClasses,
      minAttendance: 75,
      schedule: spec.schedule,
      passingGrade: spec.passingGrade,
      maxGrade: 10,
      assessments,
      createdAt: Date.now() + index,
    });

    // Dias de aula passados (até ontem).
    const meetings: { date: string; classes: number }[] = [];
    for (let d = start; toISODate(d) < toISODate(today); d = addDays(d, 1)) {
      const slot = spec.schedule.find((s) => s.weekday === d.getDay());
      if (slot) meetings.push({ date: toISODate(d), classes: slot.classes });
    }
    // Espalha as faltas ao longo do semestre.
    const step = meetings.length / Math.max(1, spec.missedMeetings);
    const missed = new Set(
      Array.from({ length: spec.missedMeetings }, (_, i) => Math.floor(i * step + step / 2)),
    );
    meetings.forEach((m, i) => {
      records.push({
        id: newId(),
        subjectId: id,
        date: m.date,
        classes: m.classes,
        status: missed.has(i) ? 'absent' : 'present',
      });
    });
  });

  return { version: 1, subjects, records };
}
