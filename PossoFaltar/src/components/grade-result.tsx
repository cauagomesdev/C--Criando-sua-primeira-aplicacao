import { StyleSheet, Text, View } from 'react-native';

import { AppText } from '@/components/ui';
import { Radius, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import { formatGrade, joinNames, type GradeSummary } from '@/lib/calc';
import type { Subject } from '@/lib/types';

export function gradeMessage(subject: Subject, g: GradeSummary) {
  const pending = joinNames(g.pendingNames);
  const many = g.pendingNames.length > 1;
  switch (g.status) {
    case 'empty':
      return {
        emoji: '📝',
        title: 'Sem avaliações',
        text: 'Edite a matéria para cadastrar as avaliações (N1, N2...).',
        tone: 'neutral' as const,
      };
    case 'approved':
      return g.finalAverage !== null
        ? {
            emoji: '🎉',
            title: `Aprovado com ${formatGrade(g.finalAverage)}`,
            text: `A média para passar era ${formatGrade(subject.passingGrade)}.`,
            tone: 'success' as const,
          }
        : {
            emoji: '🎉',
            title: 'Já passou!',
            text: `Mesmo tirando 0 ${many ? 'em' : 'na'} ${pending}, você fica com a média.`,
            tone: 'success' as const,
          };
    case 'needs':
      return {
        emoji: (g.needed ?? 0) >= 8 ? '😬' : '💪',
        title: `Precisa de ${formatGrade(g.needed ?? 0)} ${many ? 'em' : 'na'} ${pending}`,
        text: many
          ? `Tirando ${formatGrade(g.needed ?? 0)} em cada uma, você fecha com ${formatGrade(subject.passingGrade)}.`
          : `Com isso você fecha com média ${formatGrade(subject.passingGrade)}.`,
        tone: (g.needed ?? 0) >= 8 ? ('warning' as const) : ('primary' as const),
      };
    case 'impossible':
      return {
        emoji: '😱',
        title: `Precisaria de ${formatGrade(g.needed ?? 0)}`,
        text: `Não dá para chegar na média ${formatGrade(subject.passingGrade)} só com ${pending}. Hora de falar com o professor sobre recuperação.`,
        tone: 'danger' as const,
      };
    case 'failed':
      return {
        emoji: '📉',
        title: `Média final ${formatGrade(g.finalAverage ?? 0)}`,
        text: `Ficou abaixo da média ${formatGrade(subject.passingGrade)}. Confira se tem exame/recuperação.`,
        tone: 'danger' as const,
      };
  }
}

export function GradeResult({ subject, summary }: { subject: Subject; summary: GradeSummary }) {
  const theme = useTheme();
  const msg = gradeMessage(subject, summary);
  const palette = {
    neutral: { fg: theme.textSecondary, bg: theme.cardAlt },
    success: { fg: theme.success, bg: theme.successSoft },
    primary: { fg: theme.primary, bg: theme.primarySoft },
    warning: { fg: theme.warning, bg: theme.warningSoft },
    danger: { fg: theme.danger, bg: theme.dangerSoft },
  }[msg.tone];
  return (
    <View style={[styles.box, { backgroundColor: palette.bg }]}>
      <Text style={styles.emoji}>{msg.emoji}</Text>
      <View style={styles.flex}>
        <AppText variant="bodyStrong" style={{ color: palette.fg }}>
          {msg.title}
        </AppText>
        <AppText variant="caption" color="textSecondary">
          {msg.text}
        </AppText>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  box: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.three,
    padding: Spacing.three,
    borderRadius: Radius.md,
  },
  emoji: { fontSize: 28 },
  flex: { flex: 1, gap: 2 },
});
