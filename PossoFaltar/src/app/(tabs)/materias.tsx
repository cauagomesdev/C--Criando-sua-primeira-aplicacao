import { router } from 'expo-router';
import { StyleSheet, Text, View } from 'react-native';

import { LargeTitle, Screen } from '@/components/screen';
import { SubjectCard } from '@/components/subject-card';
import {
  AppText,
  Button,
  Card,
  EmptyState,
  IconButton,
  SectionHeader,
  useLevelMeta,
} from '@/components/ui';
import { Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import { summarizeAttendance, type RiskLevel } from '@/lib/calc';
import { confirmAction } from '@/lib/feedback';
import { useStore } from '@/lib/store';

const LEVEL_ORDER: RiskLevel[] = ['failed', 'danger', 'warning', 'safe'];

export default function SubjectsScreen() {
  const { data, loadSample, resetAll } = useStore();
  const theme = useTheme();
  const levelMeta = useLevelMeta();

  const items = data.subjects
    .map((subject) => ({ subject, summary: summarizeAttendance(subject, data.records) }))
    .sort(
      (a, b) =>
        LEVEL_ORDER.indexOf(a.summary.level) - LEVEL_ORDER.indexOf(b.summary.level) ||
        a.summary.remaining - b.summary.remaining,
    );

  const addButton = (
    <IconButton
      icon="plus"
      accessibilityLabel="Nova matéria"
      color={theme.primary}
      background={theme.primarySoft}
      size={44}
      onPress={() => router.push('/materia/form')}
    />
  );

  if (items.length === 0) {
    return (
      <Screen>
        <LargeTitle title="Matérias" right={addButton} />
        <EmptyState
          emoji="📚"
          title="Nenhuma matéria ainda"
          message="Adicione as matérias do semestre com a carga horária e os dias de aula.">
          <Button
            title="Adicionar matéria"
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

  const counts = LEVEL_ORDER.map((level) => ({
    level,
    count: items.filter((i) => i.summary.level === level).length,
  })).filter((c) => c.count > 0);

  return (
    <Screen>
      <LargeTitle
        title="Matérias"
        subtitle={`${items.length} ${items.length === 1 ? 'matéria' : 'matérias'} neste semestre`}
        right={addButton}
      />

      <Card style={styles.overview}>
        {counts.map(({ level, count }) => {
          const meta = levelMeta(level);
          return (
            <View key={level} style={[styles.overviewItem, { backgroundColor: meta.soft }]}>
              <Text style={styles.overviewEmoji}>{meta.emoji}</Text>
              <Text style={[styles.overviewCount, { color: meta.color }]}>{count}</Text>
              <AppText variant="caption" style={{ color: meta.color }} numberOfLines={1}>
                {meta.label}
              </AppText>
            </View>
          );
        })}
      </Card>

      <SectionHeader title="Situação das faltas" />
      <View style={styles.list}>
        {items.map(({ subject, summary }) => (
          <SubjectCard key={subject.id} subject={subject} summary={summary} />
        ))}
      </View>

      <Button
        title="Apagar tudo e recomeçar"
        icon="trash"
        variant="ghost"
        size="sm"
        color={theme.textMuted}
        style={styles.reset}
        onPress={() =>
          confirmAction({
            title: 'Apagar todos os dados?',
            message: 'Todas as matérias, presenças, faltas e notas serão apagadas deste aparelho.',
            confirmText: 'Apagar tudo',
            onConfirm: resetAll,
          })
        }
      />
    </Screen>
  );
}

const styles = StyleSheet.create({
  overview: { flexDirection: 'row', gap: Spacing.two, padding: Spacing.three },
  overviewItem: {
    flex: 1,
    alignItems: 'center',
    paddingVertical: Spacing.three,
    borderRadius: 14,
    gap: 2,
  },
  overviewEmoji: { fontSize: 20 },
  overviewCount: { fontSize: 22, fontWeight: '800' },
  list: { gap: Spacing.three },
  reset: { marginTop: Spacing.eight, alignSelf: 'center' },
});
