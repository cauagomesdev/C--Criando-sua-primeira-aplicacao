import type { Weekday } from './types';

export const WEEKDAY_SHORT = ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'] as const;
export const WEEKDAY_LONG = [
  'domingo',
  'segunda',
  'terça',
  'quarta',
  'quinta',
  'sexta',
  'sábado',
] as const;
const MONTHS = [
  'janeiro',
  'fevereiro',
  'março',
  'abril',
  'maio',
  'junho',
  'julho',
  'agosto',
  'setembro',
  'outubro',
  'novembro',
  'dezembro',
] as const;

/** Ordem dos dias exibida no app: segunda primeiro. */
export const WEEK_ORDER: Weekday[] = [1, 2, 3, 4, 5, 6, 0];

const pad = (n: number) => String(n).padStart(2, '0');

/** Data local no formato YYYY-MM-DD. */
export function toISODate(date: Date): string {
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
}

/** Converte YYYY-MM-DD em Date local (meio-dia, para evitar problemas de horário de verão). */
export function parseISODate(value: string): Date {
  const [y, m, d] = value.split('-').map(Number);
  return new Date(y, m - 1, d, 12);
}

export function todayISO(): string {
  return toISODate(new Date());
}

export function addDays(date: Date, days: number): Date {
  const copy = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 12);
  copy.setDate(copy.getDate() + days);
  return copy;
}

export function weekdayOf(value: string): Weekday {
  return parseISODate(value).getDay() as Weekday;
}

/** Os 7 dias (seg → dom) da semana que contém `date`. */
export function weekDates(date: Date): string[] {
  const offset = (date.getDay() + 6) % 7; // dias desde segunda
  const monday = addDays(date, -offset);
  return Array.from({ length: 7 }, (_, i) => toISODate(addDays(monday, i)));
}

/** "Hoje", "Ontem", "Amanhã" ou "qua, 12 nov". */
export function relativeDayLabel(value: string, today = todayISO()): string {
  const diff = Math.round(
    (parseISODate(value).getTime() - parseISODate(today).getTime()) / 86_400_000,
  );
  if (diff === 0) return 'Hoje';
  if (diff === -1) return 'Ontem';
  if (diff === 1) return 'Amanhã';
  const d = parseISODate(value);
  return `${WEEKDAY_SHORT[d.getDay()].toLowerCase()}, ${d.getDate()} ${MONTHS[d.getMonth()].slice(0, 3)}`;
}

/** "quarta, 18 de novembro" */
export function longDateLabel(value: string): string {
  const d = parseISODate(value);
  return `${WEEKDAY_LONG[d.getDay()]}, ${d.getDate()} de ${MONTHS[d.getMonth()]}`;
}

export function greeting(now = new Date()): string {
  const h = now.getHours();
  if (h < 12) return 'Bom dia';
  if (h < 18) return 'Boa tarde';
  return 'Boa noite';
}
