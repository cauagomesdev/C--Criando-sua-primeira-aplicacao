import { router, useLocalSearchParams } from 'expo-router';
import { useState } from 'react';
import { Platform, Pressable, ScrollView, StyleSheet, Text, View } from 'react-native';

import { Icon } from '@/components/icon';
import { NavHeader } from '@/components/nav-header';
import { Screen } from '@/components/screen';
import { AppText, Button, Card, Stepper, useLevelMeta } from '@/components/ui';
import { Radius, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import { classesOn, classesPerMeeting, plural, summarizeAttendance } from '@/lib/calc';
import {
  WEEKDAY_SHORT,
  addDays,
  parseISODate,
  relativeDayLabel,
  toISODate,
  todayISO,
  weekdayOf,
} from '@/lib/dates';
import { successFeedback, tapFeedback, warningFeedback } from '@/lib/feedback';
import { useStore, useSubject } from '@/lib/store';
import type { AttendanceStatus } from '@/lib/types';

const DAYS_BACK = 21;

export default function RegisterScreen() {
  const params = useLocalSearchParams<{ subjectId: string; status?: AttendanceStatus }>();
  const subject = useSubject(params.subjectId);
  const { data, setAttendance } = useStore();
  const theme = useTheme();
  const levelMeta = useLevelMeta();
  const today = todayISO();

  // Últimos dias, do mais recente para o mais antigo.
  const days = Array.from({ length: DAYS_BACK }, (_, i) =>
    toISODate(addDays(parseISODate(today), -i)),
  );
  const firstClassDay =
    subject && days.find((d) => classesOn(subject, weekdayOf(d)) > 0 && d <= today);

  const [status, setStatus] = useState<AttendanceStatus>(params.status ?? 'absent');
  const [date, setDate] = useState(firstClassDay ?? today);
  const [classes, setClasses] = useState(() =>
    subject ? classesOn(subject, weekdayOf(firstClassDay ?? today)) || classesPerMeeting(subject) : 2,
  );

  if (!subject) {
    return (
      <Screen withTabBar={false} header={<NavHeader backIcon="close" />}>
        <AppText>Matéria não encontrada.</AppText>
      </Screen>
    );
  }

  const existing = data.records.find((r) => r.subjectId === subject.id && r.date === date);
  const others = data.records.filter((r) => !(r.subjectId === subject.id && r.date === date));
  const after = summarizeAttendance(subject, others, status === 'absent' ? classes : 0);
  const meta = levelMeta(after.level);

  const pickDate = (d: string) => {
    setDate(d);
    const scheduled = classesOn(subject, weekdayOf(d));
    if (scheduled > 0) setClasses(scheduled);
  };

  const save = () => {
    if (status === 'absent' && after.level === 'failed') warningFeedback();
    else successFeedback();
    setAttendance(subject.id, date, status, classes);
    router.back();
  };

  const accent = status === 'absent' ? theme.danger : theme.success;

  return (
    <Screen
      withTabBar={false}
      header={
        <NavHeader
          title={subject.name}
          backIcon="close"
          safeTop={Platform.OS !== 'ios'}
        />
      }>
      {/* Presença ou falta */}
      <View style={[styles.segment, { backgroundColor: theme.cardAlt }]}>
        {(['present', 'absent'] as const).map((s) => {
          const active = s === status;
          const color = s === 'present' ? theme.success : theme.danger;
          return (
            <Pressable
              key={s}
              accessibilityRole="button"
              accessibilityState={{ selected: active }}
              onPress={() => {
                tapFeedback();
                setStatus(s);
              }}
              style={[styles.segmentItem, active && { backgroundColor: theme.card }]}>
              <Icon name={s === 'present' ? 'check' : 'close'} size={16} color={active ? color : theme.textSecondary} />
              <Text style={[styles.segmentText, { color: active ? color : theme.textSecondary }]}>
                {s === 'present' ? 'Fui à aula' : 'Faltei'}
              </Text>
            </Pressable>
          );
        })}
      </View>

      {/* Data */}
      <AppText variant="label" color="textSecondary" style={styles.label}>
        Quando?
      </AppText>
      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        contentContainerStyle={styles.dates}>
        {days.map((d) => {
          const selected = d === date;
          const hasClass = classesOn(subject, weekdayOf(d)) > 0;
          const recorded = data.records.find((r) => r.subjectId === subject.id && r.date === d);
          const dd = parseISODate(d);
          return (
            <Pressable
              key={d}
              accessibilityRole="button"
              accessibilityState={{ selected }}
              onPress={() => {
                tapFeedback();
                pickDate(d);
              }}
              style={[
                styles.dateItem,
                {
                  backgroundColor: selected ? accent : theme.card,
                  borderColor: selected ? accent : theme.border,
                  opacity: hasClass || selected ? 1 : 0.45,
                },
              ]}>
              <Text style={[styles.dateWeekday, { color: selected ? '#fff' : theme.textSecondary }]}>
                {d === today ? 'Hoje' : WEEKDAY_SHORT[dd.getDay()]}
              </Text>
              <Text style={[styles.dateDay, { color: selected ? '#fff' : theme.text }]}>
                {dd.getDate()}
              </Text>
              <View
                style={[
                  styles.dateDot,
                  {
                    backgroundColor: recorded
                      ? selected
                        ? '#fff'
                        : recorded.status === 'absent'
                          ? theme.danger
                          : theme.success
                      : 'transparent',
                  },
                ]}
              />
            </Pressable>
          );
        })}
      </ScrollView>
      <AppText variant="caption" color="textMuted" style={styles.legend}>
        Dias apagados não têm aula dessa matéria · bolinha = já registrado
      </AppText>

      {/* Quantidade */}
      <Card style={styles.amountCard}>
        <View style={styles.flex}>
          <AppText variant="bodyStrong">Quantas aulas?</AppText>
          <AppText variant="caption" color="textSecondary">
            {relativeDayLabel(date)}
          </AppText>
        </View>
        <Stepper value={classes} min={1} max={8} onChange={setClasses} />
      </Card>

      {/* Prévia */}
      <View style={[styles.preview, { backgroundColor: meta.soft }]}>
        <Text style={styles.previewEmoji}>{meta.emoji}</Text>
        <View style={styles.flex}>
          <AppText variant="bodyStrong" style={{ color: meta.color }}>
            {after.remaining >= 0
              ? `Depois disso: ${plural(after.remaining, 'falta restante', 'faltas restantes')}`
              : `Passa ${plural(-after.remaining, 'aula')} do limite de faltas`}
          </AppText>
          <AppText variant="caption" color="textSecondary">
            {existing
              ? `Vai substituir o registro de ${existing.status === 'absent' ? 'falta' : 'presença'} desse dia.`
              : `${after.absences} de ${after.maxAbsences} faltas usadas`}
          </AppText>
        </View>
      </View>

      <Button
        title={status === 'absent' ? 'Registrar falta' : 'Registrar presença'}
        icon={status === 'absent' ? 'close' : 'check'}
        variant="primary"
        size="lg"
        color={accent}
        style={styles.save}
        onPress={save}
      />
    </Screen>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1 },
  segment: {
    flexDirection: 'row',
    borderRadius: Radius.md,
    padding: 4,
    marginTop: Spacing.two,
  },
  segmentItem: {
    flex: 1,
    height: 44,
    borderRadius: Radius.sm,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    gap: 6,
  },
  segmentText: { fontSize: 15, fontWeight: '800' },
  label: { marginTop: Spacing.six, marginBottom: Spacing.three, paddingHorizontal: Spacing.one },
  dates: { gap: Spacing.two, paddingRight: Spacing.five },
  dateItem: {
    width: 56,
    paddingVertical: 10,
    borderRadius: Radius.md,
    borderWidth: 1.5,
    alignItems: 'center',
    gap: 2,
  },
  dateWeekday: { fontSize: 11, fontWeight: '700' },
  dateDay: { fontSize: 18, fontWeight: '800' },
  dateDot: { width: 6, height: 6, borderRadius: 3, marginTop: 2 },
  legend: { marginTop: Spacing.two, paddingHorizontal: Spacing.one },
  amountCard: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.three,
    marginTop: Spacing.five,
  },
  preview: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.three,
    padding: Spacing.four,
    borderRadius: Radius.lg,
    marginTop: Spacing.three,
  },
  previewEmoji: { fontSize: 30 },
  save: { marginTop: Spacing.five },
});
