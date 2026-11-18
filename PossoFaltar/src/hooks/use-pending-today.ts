import { classesOn } from '@/lib/calc';
import { todayISO, weekdayOf } from '@/lib/dates';
import { useStore } from '@/lib/store';

/** Quantas aulas de hoje ainda não tiveram presença/falta marcada. */
export function usePendingToday(): number {
  const { data } = useStore();
  const today = todayISO();
  const weekday = weekdayOf(today);
  return data.subjects.filter(
    (s) =>
      classesOn(s, weekday) > 0 &&
      !data.records.some((r) => r.subjectId === s.id && r.date === today),
  ).length;
}
