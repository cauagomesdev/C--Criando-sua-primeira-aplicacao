import { router } from 'expo-router';
import { useState } from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';

import { Icon } from '@/components/icon';
import { LargeTitle, Screen } from '@/components/screen';
import {
  AppText,
  Button,
  Card,
  EmptyState,
  IconButton,
  ProgressBar,
  SectionHeader,
  useLevelMeta,
} from '@/components/ui';
import { WeekStrip } from '@/components/week-strip';
import { Radius, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import {
  classesOn,
  dayVerdict,
  joinNames,
  plural,
  type DayClass,
  type DayVerdict,
} from '@/lib/calc';
import {
  WEEKDAY_LONG,
  greeting,
  longDateLabel,
  parseISODate,
  todayISO,
  weekDates,
  weekdayOf,
} from '@/lib/dates';
import { successFeedback, tapFeedback, warningFeedback } from '@/lib/feedback';
import { useStore } from '@/lib/store';
import type { AttendanceStatus } from '@/lib/types';

/** "Resta 1 falta" | "Restam 3 faltas" | "Nenhuma falta sobrando" */
function restam(n: number) {
  if (n <= 0) return 'Nenhuma falta sobrando';
  return `${n === 1 ? 'Resta' : 'Restam'} ${plural(n, 'falta')}`;
}

/** "hoje" | "na quinta" | "no sábado" */
function dayPhrase(date: string, today: string) {
  if (date === today) return 'hoje';
  const wd = weekdayOf(date);
  return `${wd === 0 || wd === 6 ? 'no' : 'na'} ${WEEKDAY_LONG[wd]}`;
}

export default function TodayScreen() {
  const { data, setAttendance, clearAttendance, loadSample } = useStore();
  const theme = useTheme();
  const today = todayISO();
  const [selected, setSelected] = useState(today);
  const week = weekDates(parseISODate(today));

  if (data.subjects.length === 0) {
    return (
      <Screen>
        <LargeTitle eyebrow={`${greeting()} 👋`} title="Posso faltar?" />
        <EmptyState
          emoji="🎒"
          title="Bora começar?"
          message="Cadastre suas matérias e descubra, num toque, se dá pra faltar sem reprovar.">
          <Button
            title="Cadastrar matéria"
            icon="plus"
            size="lg"
            onPress={() => router.push('/materia/form')}
          />
          <Button
            title="Ver com dados de exemplo"
            icon="sparkles"
            variant="secondary"
            onPress={loadSample}
          />
        </EmptyState>
      </Screen>
    );
  }

  const verdict = dayVerdict(selected, data.subjects, data.records);
  const phrase = dayPhrase(selected, today);
  const isFuture = selected > today;

  const mark = (item: DayClass, status: AttendanceStatus) => {
    if (item.record?.status === status) {
      tapFeedback();
      clearAttendance(item.subject.id, selected);
      return;
    }
    if (status === 'absent' && item.ifSkip.level === 'failed') warningFeedback();
    else successFeedback();
    setAttendance(item.subject.id, selected, status, item.classes);
  };

  return (
    <Screen>
      <LargeTitle
        eyebrow={`${greeting()} 👋  ·  ${longDateLabel(today)}`}
        title="Posso faltar?"
        right={
          <IconButton
            icon="plus"
            accessibilityLabel="Nova matéria"
            color={theme.primary}
            background={theme.primarySoft}
            size={44}
            onPress={() => router.push('/materia/form')}
          />
        }
      />

      <WeekStrip
        dates={week}
        selected={selected}
        today={today}
        onSelect={setSelected}
        hasClasses={(date) => data.subjects.some((s) => classesOn(s, weekdayOf(date)) > 0)}
      />

      <VerdictCard verdict={verdict} phrase={phrase} />

      {verdict.classes.length > 0 && (
        <>
          <SectionHeader
            title={`Aulas ${phrase === 'hoje' ? 'de hoje' : phrase.replace(/^n[ao] /, 'de ')}`}
            action={
              isFuture ? (
                <AppText variant="caption" color="textSecondary">
                  Simulação
                </AppText>
              ) : (
                <AppText variant="caption" color="textSecondary">
                  Toque para registrar
                </AppText>
              )
            }
          />
          <View style={styles.list}>
            {verdict.classes.map((item) => (
              <DayClassCard
                key={item.subject.id}
                item={item}
                canRegister={!isFuture}
                onMark={(status) => mark(item, status)}
              />
            ))}
          </View>
        </>
      )}
    </Screen>
  );
}

/* ------------------------------------------------------------------ */

function VerdictCard({ verdict, phrase }: { verdict: DayVerdict; phrase: string }) {
  const theme = useTheme();
  const names = joinNames(verdict.critical.map((c) => c.subject.name));
  const plural_ = verdict.critical.length > 1;

  const content = {
    free: {
      emoji: '🏖️',
      answer: 'Dia livre!',
      message: `Você não tem aula ${phrase}. Aproveita!`,
      color: theme.primary,
      soft: theme.primarySoft,
    },
    done: {
      emoji: '✅',
      answer: 'Tudo registrado',
      message: `Você já marcou todas as aulas ${phrase === 'hoje' ? 'de hoje' : 'desse dia'}.`,
      color: theme.primary,
      soft: theme.primarySoft,
    },
    yes: {
      emoji: '😎',
      answer: 'SIM, pode!',
      message: verdict.classes.some((c) => c.record)
        ? 'Nas aulas que ainda faltam marcar, dá pra faltar tranquilo.'
        : 'Mesmo faltando, você continua tranquilo em todas as matérias.',
      color: theme.success,
      soft: theme.successSoft,
    },
    careful: {
      emoji: '👀',
      answer: 'Pode, mas…',
      message: `Se faltar, ${names} ${plural_ ? 'ficam' : 'fica'} perto do limite de faltas.`,
      color: theme.warning,
      soft: theme.warningSoft,
    },
    no: {
      emoji: '🚫',
      answer: 'NÃO! Melhor ir',
      message: `Se faltar, você reprova por falta em ${names}.`,
      color: theme.danger,
      soft: theme.dangerSoft,
    },
  }[verdict.kind];

  return (
    <View style={[styles.verdict, { backgroundColor: content.soft }]}>
      <View style={styles.verdictTop}>
        <View style={styles.verdictText}>
          <Text style={[styles.verdictQuestion, { color: content.color }]}>
            Posso faltar {phrase}?
          </Text>
          <AppText variant="hero" style={{ color: content.color }}>
            {content.answer}
          </AppText>
        </View>
        <Text style={styles.verdictEmoji}>{content.emoji}</Text>
      </View>
      <AppText style={styles.verdictMessage}>{content.message}</AppText>
    </View>
  );
}

/* ------------------------------------------------------------------ */

function DayClassCard({
  item,
  canRegister,
  onMark,
}: {
  item: DayClass;
  canRegister: boolean;
  onMark: (status: AttendanceStatus) => void;
}) {
  const theme = useTheme();
  const levelMeta = useLevelMeta();
  const { subject, now, ifSkip, record } = item;
  const after = levelMeta(ifSkip.level);
  const nowMeta = levelMeta(now.level);

  let simulation: string;
  if (record?.status === 'absent') {
    simulation =
      now.remaining >= 0
        ? `Faltou. ${restam(now.remaining)}.`
        : 'Faltou e passou do limite de faltas.';
  } else if (record?.status === 'present') {
    simulation = `Presença confirmada! ${restam(now.remaining)}.`;
  } else if (ifSkip.level === 'failed') {
    simulation = `Se faltar: reprova por falta (${plural(-ifSkip.remaining, 'aula')} além do limite)`;
  } else {
    simulation = `Se faltar: sobram ${plural(ifSkip.remaining, 'falta')}`;
  }
  const simulationColor = record
    ? record.status === 'present'
      ? theme.success
      : nowMeta.color
    : after.color;

  return (
    <Card
      style={styles.classCard}
      onPress={() => router.push({ pathname: '/materia/[id]', params: { id: subject.id } })}>
      <View style={[styles.stripe, { backgroundColor: subject.color }]} />
      <View style={styles.classBody}>
        <View style={styles.classHeader}>
          <View style={styles.flex}>
            <AppText variant="heading" numberOfLines={1}>
              {subject.name}
            </AppText>
            <AppText variant="caption" color="textSecondary">
              {plural(item.classes, 'aula')} · {now.absences}/{now.maxAbsences} faltas usadas
            </AppText>
          </View>
          <Icon name="chevron" size={16} color={theme.textMuted} />
        </View>

        <ProgressBar value={now.usedRatio} color={nowMeta.color} height={6} />

        <View style={[styles.simulation, { backgroundColor: theme.cardAlt }]}>
          <Text style={[styles.simulationText, { color: simulationColor }]}>{simulation}</Text>
        </View>

        {canRegister && (
          <View style={styles.markRow}>
            <MarkButton
              label="Fui"
              icon="check"
              active={record?.status === 'present'}
              color={theme.success}
              soft={theme.successSoft}
              onPress={() => onMark('present')}
            />
            <MarkButton
              label="Faltei"
              icon="close"
              active={record?.status === 'absent'}
              color={theme.danger}
              soft={theme.dangerSoft}
              onPress={() => onMark('absent')}
            />
          </View>
        )}
      </View>
    </Card>
  );
}

function MarkButton({
  label,
  icon,
  active,
  color,
  soft,
  onPress,
}: {
  label: string;
  icon: 'check' | 'close';
  active: boolean;
  color: string;
  soft: string;
  onPress: () => void;
}) {
  const theme = useTheme();
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityState={{ selected: active }}
      onPress={onPress}
      style={({ pressed }) => [
        styles.markButton,
        {
          backgroundColor: active ? color : soft,
          borderColor: active ? color : 'transparent',
        },
        pressed && { opacity: 0.75 },
      ]}>
      <Icon name={icon} size={16} color={active ? theme.onPrimary : color} />
      <Text style={[styles.markText, { color: active ? theme.onPrimary : color }]}>{label}</Text>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1 },
  list: { gap: Spacing.three },
  verdict: {
    marginTop: Spacing.five,
    borderRadius: Radius.xl,
    padding: Spacing.five,
    gap: Spacing.three,
  },
  verdictTop: { flexDirection: 'row', alignItems: 'center', gap: Spacing.three },
  verdictText: { flex: 1, gap: 4 },
  verdictQuestion: { fontSize: 15, fontWeight: '800' },
  verdictEmoji: { fontSize: 56 },
  verdictMessage: { fontSize: 16, lineHeight: 22 },
  classCard: { flexDirection: 'row', padding: 0, overflow: 'hidden' },
  stripe: { width: 6 },
  classBody: { flex: 1, padding: Spacing.four, gap: Spacing.three },
  classHeader: { flexDirection: 'row', alignItems: 'center', gap: Spacing.two },
  simulation: { borderRadius: Radius.sm, paddingHorizontal: 12, paddingVertical: 8 },
  simulationText: { fontSize: 14, fontWeight: '700' },
  markRow: { flexDirection: 'row', gap: Spacing.two },
  markButton: {
    flex: 1,
    height: 44,
    borderRadius: Radius.md,
    borderWidth: 1.5,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    gap: 6,
  },
  markText: { fontSize: 15, fontWeight: '800' },
});
