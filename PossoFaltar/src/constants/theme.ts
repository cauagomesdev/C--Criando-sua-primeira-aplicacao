import '@/global.css';

import { Platform } from 'react-native';

export const Colors = {
  light: {
    text: '#14142B',
    textSecondary: '#6E6E87',
    textMuted: '#A0A0B8',
    background: '#F4F4FA',
    card: '#FFFFFF',
    cardAlt: '#ECECF6',
    border: '#E3E3EE',
    primary: '#5B4BF5',
    primarySoft: '#ECEAFF',
    onPrimary: '#FFFFFF',
    success: '#12A150',
    successSoft: '#DDF6E7',
    warning: '#D98200',
    warningSoft: '#FFF0D6',
    danger: '#E5374B',
    dangerSoft: '#FFE1E5',
    failed: '#8A1C2B',
    failedSoft: '#F6D9DE',
    tabBar: '#FFFFFF',
  },
  dark: {
    text: '#F4F4FB',
    textSecondary: '#A3A3BD',
    textMuted: '#6B6B85',
    background: '#0D0D16',
    card: '#181826',
    cardAlt: '#232335',
    border: '#2A2A3E',
    primary: '#8B7FFF',
    primarySoft: '#26224A',
    onPrimary: '#FFFFFF',
    success: '#3DDC84',
    successSoft: '#143323',
    warning: '#FFB23F',
    warningSoft: '#3A2A10',
    danger: '#FF5C70',
    dangerSoft: '#3D1820',
    failed: '#FF8A99',
    failedSoft: '#4A1621',
    tabBar: '#141420',
  },
} as const;

export type ThemeColors = { [K in keyof typeof Colors.light]: string };
export type ThemeColor = keyof ThemeColors;

/** Cores disponíveis para identificar as matérias. */
export const SubjectColors = [
  '#5B4BF5',
  '#00A3FF',
  '#12B886',
  '#F59F00',
  '#FF6B6B',
  '#E64980',
  '#7950F2',
  '#15AABF',
] as const;

export const Fonts = Platform.select({
  ios: { rounded: 'ui-rounded', sans: 'system-ui' },
  web: { rounded: 'var(--font-rounded)', sans: 'var(--font-display)' },
  default: { rounded: 'normal', sans: 'normal' },
});

export const Spacing = {
  half: 2,
  one: 4,
  two: 8,
  three: 12,
  four: 16,
  five: 20,
  six: 24,
  eight: 32,
} as const;

export const Radius = {
  sm: 10,
  md: 14,
  lg: 20,
  xl: 28,
  pill: 999,
} as const;

export const MaxContentWidth = 640;
