import { Pressable, StyleSheet, Text, View } from 'react-native';

import { Radius } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import { WEEKDAY_SHORT, parseISODate } from '@/lib/dates';
import { tapFeedback } from '@/lib/feedback';

/** Faixa com os 7 dias da semana para escolher qual dia simular. */
export function WeekStrip({
  dates,
  selected,
  today,
  hasClasses,
  onSelect,
}: {
  dates: string[];
  selected: string;
  today: string;
  hasClasses: (date: string) => boolean;
  onSelect: (date: string) => void;
}) {
  const theme = useTheme();
  return (
    <View style={styles.row}>
      {dates.map((date) => {
        const d = parseISODate(date);
        const isSelected = date === selected;
        const isToday = date === today;
        const fg = isSelected ? theme.onPrimary : theme.text;
        return (
          <Pressable
            key={date}
            accessibilityRole="button"
            accessibilityLabel={`${WEEKDAY_SHORT[d.getDay()]} ${d.getDate()}`}
            accessibilityState={{ selected: isSelected }}
            onPress={() => {
              tapFeedback();
              onSelect(date);
            }}
            style={({ pressed }) => [
              styles.day,
              {
                backgroundColor: isSelected ? theme.primary : theme.card,
                borderColor: isToday && !isSelected ? theme.primary : theme.border,
              },
              pressed && styles.pressed,
            ]}>
            <Text
              style={[
                styles.weekday,
                { color: isSelected ? theme.onPrimary : theme.textSecondary },
              ]}>
              {WEEKDAY_SHORT[d.getDay()]}
            </Text>
            <Text style={[styles.date, { color: fg }]}>{d.getDate()}</Text>
            <View
              style={[
                styles.dot,
                {
                  backgroundColor: hasClasses(date)
                    ? isSelected
                      ? theme.onPrimary
                      : theme.primary
                    : 'transparent',
                },
              ]}
            />
          </Pressable>
        );
      })}
    </View>
  );
}

const styles = StyleSheet.create({
  row: { flexDirection: 'row', gap: 6 },
  day: {
    flex: 1,
    alignItems: 'center',
    paddingVertical: 10,
    borderRadius: Radius.md,
    borderWidth: 1.5,
    gap: 2,
  },
  pressed: { opacity: 0.7 },
  weekday: { fontSize: 11, fontWeight: '700' },
  date: { fontSize: 18, fontWeight: '800', fontVariant: ['tabular-nums'] },
  dot: { width: 5, height: 5, borderRadius: 3, marginTop: 2 },
});
