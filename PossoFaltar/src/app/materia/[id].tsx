import { router, useLocalSearchParams } from 'expo-router';
import { useState } from 'react';
import { StyleSheet, Text, TextInput, View } from 'react-native';

import { GradeResult } from '@/components/grade-result';
import { Icon } from '@/components/icon';
import { NavHeader } from '@/components/nav-header';
import { Screen } from '@/components/screen';
import { remainingLabel } from '@/components/subject-card';
import {
  AppText,
  Button,
  Card,
  EmptyState,
  IconButton,
  Pill,
  ProgressBar,
  SectionHeader,
  useLevelMeta,
} from '@/components/ui';
import { Radius, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import {
  formatGrade,
  formatPercent,
  plural,
  summarizeAttendance,
  summarizeGrades,
} from '@/lib/calc';
import { WEEKDAY_SHORT, WEEK_ORDER, relativeDayLabel } from '@/lib/dates';
import { confirmAction } from '@/lib/feedback';
import { useStore, useSubject } from '@/lib/store';
import type { Assessment, Subject } from '@/lib/types';

const HISTORY_PREVIEW = 6;

export default function SubjectDetailScreen() {
  const { id } = useLocalSearchParams<{ id: string }>();
  const subject = useSubject(id);
  const { data, deleteSubject, deleteRecord } = useStore();
  const theme = useTheme();
  const levelMeta = useLevelMeta();
  const [showAll, setShowAll] = useState(false);

  if (!subject) {
    return (
      <Screen withTabBar={false} header={<NavHeader />}>
        <EmptyState emoji="🤔" title="Matéria não encontrada" message="Ela pode ter sido excluída." />
      </Screen>
    );
  }

  const summary = summarizeAttendance(subject, data.records);
  const grades = summarizeGrades(subject);
  const meta = levelMeta(summary.level);
  const history = data.records
    .filter((r) => r.subjectId === subject.id)
    .sort((a, b) => b.date.localeCompare(a.date));
  const visibleHistory = showAll ? history : history.slice(0, HISTORY_PREVIEW);

  const openRegister = (status: 'present' | 'absent') =>
    router.push({ pathname: '/registrar', params: { subjectId: subject.id, status } });

  const remove = () =>
    confirmAction({
      title: 'Excluir matéria?',
      message: `"${subject.name}" e todo o histórico de presenças serão apagados.`,
      confirmText: 'Excluir',
      onConfirm: () => {
        router.back();
        deleteSubject(subject.id);
      },
    });

  return (
    <Screen
      withTabBar={false}
      header={
        <NavHeader
          right={
            <IconButton
              icon="edit"
              accessibilityLabel="Editar matéria"
              onPress={() =>
                router.push({ pathname: '/materia/form', params: { id: subject.id } })
              }
            />
          }
        />
      }>
      {/* Destaque com a cor da matéria */}
      <View style={[styles.hero, { backgroundColor: subject.color }]}>
        <View style={styles.heroTop}>
          <View style={styles.flex}>
            <Text style={styles.heroName}>{subject.name}</Text>
            {!!subject.professor && <Text style={styles.heroSub}>{subject.professor}</Text>}
          </View>
          <Pill
            label={`${meta.emoji} ${meta.label}`}
            color={meta.color}
            background="#FFFFFF"
          />
        </View>

        <View style={styles.heroNumberRow}>
          <Text style={styles.heroNumber}>{Math.max(0, summary.remaining)}</Text>
          <View style={styles.flex}>
            <Text style={styles.heroNumberLabel}>
              {summary.remaining === 1 ? 'falta restante' : 'faltas restantes'}
            </Text>
            <Text style={styles.heroSub}>
              {summary.remaining > 0
                ? summary.remainingMeetings > 0
                  ? `≈ ${plural(summary.remainingMeetings, 'dia')} de aula`
                  : 'Menos de um dia inteiro de aula'
                : summary.remaining === 0
                  ? 'Você está no limite!'
                  : `Passou ${plural(-summary.remaining, 'aula')} do limite`}
            </Text>
          </View>
        </View>

        <ProgressBar
          value={summary.usedRatio}
          color="#FFFFFF"
          trackColor="rgba(255,255,255,0.3)"
          height={10}
        />
        <Text style={styles.heroSub}>
          {summary.absences} de {summary.maxAbsences} faltas permitidas usadas
        </Text>
      </View>

      {/* Números */}
      <View style={styles.stats}>
        <Stat
          label="Presença"
          value={
            summary.currentAttendance === null ? '—' : formatPercent(summary.currentAttendance)
          }
        />
        <Stat label="Mínimo" value={`${subject.minAttendance}%`} />
        <Stat label="Total" value={`${subject.totalClasses}`} caption="aulas" />
      </View>

      <AppText variant="bodyStrong" style={[styles.remaining, { color: meta.color }]}>
        {remainingLabel(summary)}
      </AppText>

      <View style={styles.actions}>
        <Button
          title="Fui à aula"
          icon="check"
          variant="success"
          style={styles.flex}
          onPress={() => openRegister('present')}
        />
        <Button
          title="Faltei"
          icon="close"
          variant="danger"
          style={styles.flex}
          onPress={() => openRegister('absent')}
        />
      </View>

      {/* Notas */}
      <SectionHeader
        title="Notas"
        action={
          <AppText variant="caption" color="textSecondary">
            Média para passar: {formatGrade(subject.passingGrade)}
          </AppText>
        }
      />
      <Card style={styles.gradesCard}>
        {subject.assessments.map((a) => (
          <GradeRow key={a.id} subject={subject} assessment={a} />
        ))}
        <GradeResult subject={subject} summary={grades} />
      </Card>

      {/* Horários */}
      <SectionHeader title="Dias de aula" />
      <View style={styles.schedule}>
        {WEEK_ORDER.filter((wd) => subject.schedule.some((s) => s.weekday === wd)).map((wd) => {
          const slot = subject.schedule.find((s) => s.weekday === wd)!;
          return (
            <View key={wd} style={[styles.slot, { backgroundColor: theme.card, borderColor: theme.border }]}>
              <Icon name="calendar" size={14} color={subject.color} />
              <AppText variant="bodyStrong">{WEEKDAY_SHORT[wd]}</AppText>
              <AppText variant="caption" color="textSecondary">
                {plural(slot.classes, 'aula')}
              </AppText>
            </View>
          );
        })}
      </View>

      {/* Histórico */}
      <SectionHeader
        title={`Histórico (${history.length})`}
        action={
          history.length > HISTORY_PREVIEW ? (
            <Button
              title={showAll ? 'Mostrar menos' : 'Ver tudo'}
              variant="ghost"
              size="sm"
              onPress={() => setShowAll((v) => !v)}
            />
          ) : undefined
        }
      />
      {history.length === 0 ? (
        <Card>
          <AppText color="textSecondary" style={styles.center}>
            Nenhuma presença ou falta registrada ainda.
          </AppText>
        </Card>
      ) : (
        <Card style={styles.historyCard}>
          {visibleHistory.map((r, index) => {
            const absent = r.status === 'absent';
            return (
              <View
                key={r.id}
                style={[
                  styles.historyRow,
                  index > 0 && { borderTopWidth: StyleSheet.hairlineWidth, borderTopColor: theme.border },
                ]}>
                <View
                  style={[
                    styles.historyIcon,
                    { backgroundColor: absent ? theme.dangerSoft : theme.successSoft },
                  ]}>
                  <Icon
                    name={absent ? 'close' : 'check'}
                    size={14}
                    color={absent ? theme.danger : theme.success}
                  />
                </View>
                <View style={styles.flex}>
                  <AppText variant="bodyStrong">{relativeDayLabel(r.date)}</AppText>
                  <AppText variant="caption" color="textSecondary">
                    {absent ? 'Falta' : 'Presença'} · {plural(r.classes, 'aula')}
                  </AppText>
                </View>
                <IconButton
                  icon="trash"
                  size={34}
                  accessibilityLabel="Apagar registro"
                  color={theme.textMuted}
                  background="transparent"
                  onPress={() => deleteRecord(r.id)}
                />
              </View>
            );
          })}
        </Card>
      )}

      <Button
        title="Excluir matéria"
        icon="trash"
        variant="ghost"
        color={theme.danger}
        style={styles.delete}
        onPress={remove}
      />
    </Screen>
  );
}

function Stat({ label, value, caption }: { label: string; value: string; caption?: string }) {
  return (
    <Card style={styles.stat}>
      <AppText variant="label" color="textSecondary">
        {label}
      </AppText>
      <Text style={styles.statRow}>
        <AppText variant="title" style={styles.statValue}>
          {value}
        </AppText>
        {caption && (
          <AppText variant="caption" color="textSecondary">
            {' '}
            {caption}
          </AppText>
        )}
      </Text>
    </Card>
  );
}

/** Campo para digitar a nota de uma avaliação; atualiza o cálculo em tempo real. */
function GradeRow({ subject, assessment }: { subject: Subject; assessment: Assessment }) {
  const theme = useTheme();
  const { setGrade } = useStore();
  const [text, setText] = useState(
    assessment.grade === null ? '' : String(assessment.grade).replace('.', ','),
  );
  const parsed = text.trim() === '' ? null : Number(text.replace(',', '.'));
  const invalid =
    parsed !== null && (Number.isNaN(parsed) || parsed < 0 || parsed > subject.maxGrade);

  const onChange = (value: string) => {
    const clean = value.replace(/[^0-9.,]/g, '').slice(0, 5);
    setText(clean);
    if (clean.trim() === '') return setGrade(subject.id, assessment.id, null);
    const n = Number(clean.replace(',', '.'));
    if (!Number.isNaN(n) && n >= 0 && n <= subject.maxGrade) {
      setGrade(subject.id, assessment.id, n);
    }
  };

  const showWeight = subject.assessments.some((a) => a.weight !== assessment.weight);

  return (
    <View style={styles.gradeRow}>
      <View style={styles.flex}>
        <AppText variant="bodyStrong">{assessment.name}</AppText>
        <AppText variant="caption" color="textSecondary">
          {showWeight ? `Peso ${assessment.weight}` : `Nota de 0 a ${subject.maxGrade}`}
        </AppText>
      </View>
      <TextInput
        value={text}
        onChangeText={onChange}
        placeholder="—"
        placeholderTextColor={theme.textMuted}
        keyboardType="decimal-pad"
        returnKeyType="done"
        accessibilityLabel={`Nota da ${assessment.name}`}
        style={[
          styles.gradeInput,
          {
            color: theme.text,
            backgroundColor: theme.cardAlt,
            borderColor: invalid ? theme.danger : 'transparent',
          },
        ]}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1 },
  center: { textAlign: 'center' },
  hero: {
    borderRadius: Radius.xl,
    padding: Spacing.five,
    gap: Spacing.three,
    marginTop: Spacing.two,
  },
  heroTop: { flexDirection: 'row', alignItems: 'flex-start', gap: Spacing.three },
  heroName: { color: '#fff', fontSize: 24, fontWeight: '800', lineHeight: 30 },
  heroSub: { color: 'rgba(255,255,255,0.85)', fontSize: 14, fontWeight: '600' },
  heroNumberRow: { flexDirection: 'row', alignItems: 'center', gap: Spacing.three },
  heroNumber: {
    color: '#fff',
    fontSize: 64,
    lineHeight: 70,
    fontWeight: '900',
    fontVariant: ['tabular-nums'],
  },
  heroNumberLabel: { color: '#fff', fontSize: 20, fontWeight: '800' },
  stats: { flexDirection: 'row', gap: Spacing.two, marginTop: Spacing.three },
  stat: { flex: 1, gap: 4, padding: Spacing.three },
  statRow: {},
  statValue: { fontSize: 24, lineHeight: 30 },
  remaining: { textAlign: 'center', marginTop: Spacing.four },
  actions: { flexDirection: 'row', gap: Spacing.three, marginTop: Spacing.three },
  gradesCard: { gap: Spacing.three },
  gradeRow: { flexDirection: 'row', alignItems: 'center', gap: Spacing.three },
  gradeInput: {
    width: 84,
    height: 48,
    borderRadius: Radius.md,
    borderWidth: 2,
    textAlign: 'center',
    fontSize: 20,
    fontWeight: '800',
  },
  schedule: { flexDirection: 'row', flexWrap: 'wrap', gap: Spacing.two },
  slot: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: Radius.md,
    borderWidth: StyleSheet.hairlineWidth,
  },
  historyCard: { paddingVertical: Spacing.one },
  historyRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.three,
    paddingVertical: Spacing.two,
  },
  historyIcon: {
    width: 30,
    height: 30,
    borderRadius: 15,
    alignItems: 'center',
    justifyContent: 'center',
  },
  delete: { marginTop: Spacing.six },
});
