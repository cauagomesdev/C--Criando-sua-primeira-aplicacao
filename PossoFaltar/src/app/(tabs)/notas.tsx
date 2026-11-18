import { router } from 'expo-router';
import { StyleSheet, View } from 'react-native';

import { GradeResult } from '@/components/grade-result';
import { Icon } from '@/components/icon';
import { LargeTitle, Screen } from '@/components/screen';
import { AppText, Button, Card, EmptyState } from '@/components/ui';
import { Radius, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import { formatGrade, summarizeGrades, type GradeStatus } from '@/lib/calc';
import { useStore } from '@/lib/store';

const ORDER: GradeStatus[] = ['impossible', 'needs', 'failed', 'empty', 'approved'];

export default function GradesScreen() {
  const { data, loadSample } = useStore();
  const theme = useTheme();

  if (data.subjects.length === 0) {
    return (
      <Screen>
        <LargeTitle title="Notas" />
        <EmptyState
          emoji="🎓"
          title="Calcule quanto precisa tirar"
          message="Lance suas notas e o app mostra quanto falta para passar em cada matéria.">
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

  const items = data.subjects
    .map((subject) => ({ subject, grades: summarizeGrades(subject) }))
    .sort(
      (a, b) =>
        ORDER.indexOf(a.grades.status) - ORDER.indexOf(b.grades.status) ||
        (b.grades.needed ?? 0) - (a.grades.needed ?? 0),
    );
  const approved = items.filter((i) => i.grades.status === 'approved').length;

  return (
    <Screen>
      <LargeTitle
        title="Notas"
        subtitle={
          approved > 0
            ? `${approved} de ${items.length} matérias já garantidas 🎉`
            : 'Quanto falta pra passar em cada matéria'
        }
      />

      <View style={styles.list}>
        {items.map(({ subject, grades }) => (
          <Card
            key={subject.id}
            style={styles.card}
            onPress={() =>
              router.push({ pathname: '/materia/[id]', params: { id: subject.id } })
            }>
            <View style={styles.header}>
              <View style={[styles.dot, { backgroundColor: subject.color }]} />
              <AppText variant="heading" numberOfLines={1} style={styles.flex}>
                {subject.name}
              </AppText>
              <Icon name="chevron" size={16} color={theme.textMuted} />
            </View>

            {subject.assessments.length > 0 && (
              <View style={styles.assessments}>
                {subject.assessments.map((a) => (
                  <View
                    key={a.id}
                    style={[styles.assessment, { backgroundColor: theme.cardAlt }]}>
                    <AppText variant="caption" color="textSecondary">
                      {a.name}
                      {subject.assessments.some((x) => x.weight !== a.weight)
                        ? ` (peso ${a.weight})`
                        : ''}
                    </AppText>
                    <AppText
                      variant="bodyStrong"
                      color={a.grade === null ? 'textMuted' : 'text'}>
                      {a.grade === null ? '—' : formatGrade(a.grade)}
                    </AppText>
                  </View>
                ))}
              </View>
            )}

            <GradeResult subject={subject} summary={grades} />
          </Card>
        ))}
      </View>

      <AppText variant="caption" color="textMuted" style={styles.footnote}>
        Toque em uma matéria para lançar ou editar as notas.
      </AppText>
    </Screen>
  );
}

const styles = StyleSheet.create({
  list: { gap: Spacing.three },
  card: { gap: Spacing.three },
  header: { flexDirection: 'row', alignItems: 'center', gap: Spacing.two },
  dot: { width: 12, height: 12, borderRadius: 6 },
  flex: { flex: 1 },
  assessments: { flexDirection: 'row', flexWrap: 'wrap', gap: Spacing.two },
  assessment: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: Radius.sm,
  },
  footnote: { textAlign: 'center', marginTop: Spacing.five },
});
