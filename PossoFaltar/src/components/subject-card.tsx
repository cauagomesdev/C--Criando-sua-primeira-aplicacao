import { router } from 'expo-router';
import { StyleSheet, View } from 'react-native';

import { Icon } from '@/components/icon';
import { AppText, Card, Pill, ProgressBar, useLevelMeta } from '@/components/ui';
import { Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import { plural, type AttendanceSummary } from '@/lib/calc';
import { WEEKDAY_SHORT, WEEK_ORDER } from '@/lib/dates';
import type { Subject } from '@/lib/types';

export function scheduleLabel(subject: Subject): string {
  return WEEK_ORDER.filter((wd) => subject.schedule.some((s) => s.weekday === wd))
    .map((wd) => WEEKDAY_SHORT[wd])
    .join(' · ');
}

export function remainingLabel(summary: AttendanceSummary): string {
  if (summary.remaining < 0) return `Passou ${plural(-summary.remaining, 'falta')} do limite`;
  if (summary.remaining === 0) return 'Não pode faltar mais nenhuma aula';
  const days =
    summary.remainingMeetings > 0 ? ` (≈ ${plural(summary.remainingMeetings, 'dia')})` : '';
  return `Pode faltar mais ${plural(summary.remaining, 'aula')}${days}`;
}

export function SubjectCard({ subject, summary }: { subject: Subject; summary: AttendanceSummary }) {
  const theme = useTheme();
  const meta = useLevelMeta()(summary.level);
  return (
    <Card
      style={styles.card}
      onPress={() => router.push({ pathname: '/materia/[id]', params: { id: subject.id } })}>
      <View style={styles.top}>
        <View style={[styles.avatar, { backgroundColor: subject.color }]}>
          <AppText variant="heading" style={styles.avatarText}>
            {subject.name.trim().charAt(0).toUpperCase()}
          </AppText>
        </View>
        <View style={styles.flex}>
          <AppText variant="heading" numberOfLines={1}>
            {subject.name}
          </AppText>
          <AppText variant="caption" color="textSecondary" numberOfLines={1}>
            {[subject.professor, scheduleLabel(subject)].filter(Boolean).join('  ·  ')}
          </AppText>
        </View>
        <Icon name="chevron" size={16} color={theme.textMuted} />
      </View>

      <View style={styles.statsRow}>
        <Pill label={`${meta.emoji} ${meta.label}`} color={meta.color} background={meta.soft} />
        <AppText variant="caption" color="textSecondary">
          <AppText variant="bodyStrong">{summary.absences}</AppText> de {summary.maxAbsences}{' '}
          faltas
        </AppText>
      </View>

      <ProgressBar value={summary.usedRatio} color={meta.color} />

      <AppText variant="bodyStrong" style={{ color: meta.color }}>
        {remainingLabel(summary)}
      </AppText>
    </Card>
  );
}

const styles = StyleSheet.create({
  card: { gap: Spacing.three },
  top: { flexDirection: 'row', alignItems: 'center', gap: Spacing.three },
  avatar: {
    width: 44,
    height: 44,
    borderRadius: 14,
    alignItems: 'center',
    justifyContent: 'center',
  },
  avatarText: { color: '#fff', fontSize: 20 },
  flex: { flex: 1 },
  statsRow: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' },
});
