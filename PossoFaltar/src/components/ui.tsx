import type { ReactNode } from 'react';
import {
  Pressable,
  StyleSheet,
  Text,
  View,
  type PressableProps,
  type StyleProp,
  type TextProps,
  type TextStyle,
  type ViewStyle,
} from 'react-native';

import { Icon, type IconName } from '@/components/icon';
import { Fonts, Radius, Spacing, type ThemeColor } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';
import type { RiskLevel } from '@/lib/calc';
import { tapFeedback } from '@/lib/feedback';

/* ------------------------------------------------------------------ */
/* Texto                                                               */
/* ------------------------------------------------------------------ */

type Variant = 'hero' | 'title' | 'heading' | 'body' | 'bodyStrong' | 'caption' | 'label';

export function AppText({
  variant = 'body',
  color = 'text',
  style,
  ...rest
}: TextProps & { variant?: Variant; color?: ThemeColor | (string & {}) }) {
  const theme = useTheme();
  const resolved = color in theme ? theme[color as ThemeColor] : color;
  return <Text style={[textStyles[variant], { color: resolved }, style]} {...rest} />;
}

const textStyles = StyleSheet.create({
  hero: { fontSize: 40, lineHeight: 44, fontWeight: '800', fontFamily: Fonts.rounded, letterSpacing: -0.5 },
  title: { fontSize: 30, lineHeight: 36, fontWeight: '800', fontFamily: Fonts.rounded, letterSpacing: -0.3 },
  heading: { fontSize: 18, lineHeight: 24, fontWeight: '700', fontFamily: Fonts.rounded },
  body: { fontSize: 15, lineHeight: 21, fontWeight: '500' },
  bodyStrong: { fontSize: 15, lineHeight: 21, fontWeight: '700' },
  caption: { fontSize: 13, lineHeight: 18, fontWeight: '500' },
  label: {
    fontSize: 12,
    lineHeight: 16,
    fontWeight: '700',
    letterSpacing: 0.6,
    textTransform: 'uppercase',
  },
});

/* ------------------------------------------------------------------ */
/* Containers                                                          */
/* ------------------------------------------------------------------ */

export function Card({
  children,
  style,
  onPress,
}: {
  children: ReactNode;
  style?: StyleProp<ViewStyle>;
  onPress?: () => void;
}) {
  const theme = useTheme();
  const base = [styles.card, { backgroundColor: theme.card, borderColor: theme.border }, style];
  if (!onPress) return <View style={base}>{children}</View>;
  return (
    <Pressable
      onPress={() => {
        tapFeedback();
        onPress();
      }}
      style={({ pressed }) => [base, pressed && styles.pressed]}>
      {children}
    </Pressable>
  );
}

export function SectionHeader({ title, action }: { title: string; action?: ReactNode }) {
  return (
    <View style={styles.sectionHeader}>
      <AppText variant="label" color="textSecondary">
        {title}
      </AppText>
      {action}
    </View>
  );
}

/* ------------------------------------------------------------------ */
/* Botões                                                              */
/* ------------------------------------------------------------------ */

type ButtonVariant = 'primary' | 'secondary' | 'ghost' | 'danger' | 'success';

export function Button({
  title,
  icon,
  variant = 'primary',
  size = 'md',
  color,
  style,
  onPress,
  ...rest
}: Omit<PressableProps, 'style' | 'children'> & {
  title: string;
  icon?: IconName;
  variant?: ButtonVariant;
  size?: 'sm' | 'md' | 'lg';
  /** Cor de destaque customizada (ex.: cor da matéria). */
  color?: string;
  style?: StyleProp<ViewStyle>;
}) {
  const theme = useTheme();
  const palette: Record<ButtonVariant, { bg: string; fg: string; border?: string }> = {
    primary: { bg: color ?? theme.primary, fg: theme.onPrimary },
    secondary: { bg: theme.cardAlt, fg: theme.text },
    ghost: { bg: 'transparent', fg: color ?? theme.primary },
    danger: { bg: theme.dangerSoft, fg: theme.danger },
    success: { bg: theme.successSoft, fg: theme.success },
  };
  const p = palette[variant];
  const height = size === 'sm' ? 36 : size === 'lg' ? 56 : 48;
  return (
    <Pressable
      accessibilityRole="button"
      onPress={(e) => {
        tapFeedback();
        onPress?.(e);
      }}
      style={({ pressed }) => [
        styles.button,
        { backgroundColor: p.bg, height, paddingHorizontal: size === 'sm' ? 14 : 20 },
        pressed && styles.pressed,
        rest.disabled && styles.disabled,
        style,
      ]}
      {...rest}>
      {icon && <Icon name={icon} size={size === 'sm' ? 15 : 18} color={p.fg} />}
      <Text
        style={[
          styles.buttonText,
          { color: p.fg, fontSize: size === 'sm' ? 14 : size === 'lg' ? 17 : 16 },
        ]}>
        {title}
      </Text>
    </Pressable>
  );
}

export function IconButton({
  icon,
  onPress,
  color,
  background,
  accessibilityLabel,
  size = 40,
}: {
  icon: IconName;
  onPress: () => void;
  color?: string;
  background?: string;
  accessibilityLabel: string;
  size?: number;
}) {
  const theme = useTheme();
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      hitSlop={8}
      onPress={() => {
        tapFeedback();
        onPress();
      }}
      style={({ pressed }) => [
        styles.iconButton,
        { width: size, height: size, backgroundColor: background ?? theme.card },
        pressed && styles.pressed,
      ]}>
      <Icon name={icon} size={size * 0.45} color={color ?? theme.text} />
    </Pressable>
  );
}

/* ------------------------------------------------------------------ */
/* Indicadores                                                         */
/* ------------------------------------------------------------------ */

export function ProgressBar({
  value,
  color,
  height = 8,
  trackColor,
}: {
  value: number;
  color: string;
  height?: number;
  trackColor?: string;
}) {
  const theme = useTheme();
  const pct = Math.max(0, Math.min(1, value));
  return (
    <View
      style={[
        styles.track,
        { height, borderRadius: height / 2, backgroundColor: trackColor ?? theme.cardAlt },
      ]}>
      <View
        style={{
          width: `${pct * 100}%`,
          minWidth: pct > 0 ? height : 0,
          height,
          borderRadius: height / 2,
          backgroundColor: color,
        }}
      />
    </View>
  );
}

export function Pill({
  label,
  color,
  background,
  style,
}: {
  label: string;
  color: string;
  background: string;
  style?: StyleProp<ViewStyle>;
}) {
  return (
    <View style={[styles.pill, { backgroundColor: background }, style]}>
      <Text style={[styles.pillText, { color }]}>{label}</Text>
    </View>
  );
}

export function Chip({
  label,
  selected,
  onPress,
  color,
  style,
  textStyle,
}: {
  label: string;
  selected: boolean;
  onPress: () => void;
  color?: string;
  style?: StyleProp<ViewStyle>;
  textStyle?: StyleProp<TextStyle>;
}) {
  const theme = useTheme();
  const accent = color ?? theme.primary;
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityState={{ selected }}
      onPress={() => {
        tapFeedback();
        onPress();
      }}
      style={({ pressed }) => [
        styles.chip,
        {
          backgroundColor: selected ? accent : theme.card,
          borderColor: selected ? accent : theme.border,
        },
        pressed && styles.pressed,
        style,
      ]}>
      <Text
        style={[styles.chipText, { color: selected ? theme.onPrimary : theme.text }, textStyle]}>
        {label}
      </Text>
    </Pressable>
  );
}

export function Stepper({
  value,
  onChange,
  min = 0,
  max = 999,
  step = 1,
  format = (v: number) => String(v),
  compact,
}: {
  value: number;
  onChange: (value: number) => void;
  min?: number;
  max?: number;
  step?: number;
  format?: (value: number) => string;
  compact?: boolean;
}) {
  const theme = useTheme();
  const set = (v: number) => onChange(Math.min(max, Math.max(min, Math.round(v * 100) / 100)));
  const size = compact ? 30 : 38;
  return (
    <View style={[styles.stepper, { backgroundColor: theme.cardAlt }]}>
      <IconButton
        icon="minus"
        accessibilityLabel="Diminuir"
        size={size}
        background={theme.card}
        onPress={() => set(value - step)}
      />
      <Text
        style={[
          styles.stepperValue,
          { color: theme.text, minWidth: compact ? 28 : 48, fontSize: compact ? 15 : 18 },
        ]}>
        {format(value)}
      </Text>
      <IconButton
        icon="plus"
        accessibilityLabel="Aumentar"
        size={size}
        background={theme.card}
        onPress={() => set(value + step)}
      />
    </View>
  );
}

/* ------------------------------------------------------------------ */
/* Estado de risco                                                     */
/* ------------------------------------------------------------------ */

export function useLevelMeta() {
  const theme = useTheme();
  return (level: RiskLevel) =>
    ({
      safe: { label: 'Tranquilo', emoji: '😎', color: theme.success, soft: theme.successSoft },
      warning: { label: 'Atenção', emoji: '👀', color: theme.warning, soft: theme.warningSoft },
      danger: { label: 'No limite', emoji: '😬', color: theme.danger, soft: theme.dangerSoft },
      failed: { label: 'Reprovado', emoji: '💀', color: theme.failed, soft: theme.failedSoft },
    })[level];
}

export function EmptyState({
  emoji,
  title,
  message,
  children,
}: {
  emoji: string;
  title: string;
  message: string;
  children?: ReactNode;
}) {
  return (
    <View style={styles.empty}>
      <Text style={styles.emptyEmoji}>{emoji}</Text>
      <AppText variant="heading" style={styles.center}>
        {title}
      </AppText>
      <AppText color="textSecondary" style={styles.center}>
        {message}
      </AppText>
      {children && <View style={styles.emptyActions}>{children}</View>}
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    borderRadius: Radius.lg,
    padding: Spacing.four,
    borderWidth: StyleSheet.hairlineWidth,
    boxShadow: '0 2px 10px rgba(20, 20, 43, 0.05)',
  },
  pressed: { opacity: 0.7, transform: [{ scale: 0.98 }] },
  disabled: { opacity: 0.4 },
  sectionHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginTop: Spacing.six,
    marginBottom: Spacing.three,
    paddingHorizontal: Spacing.one,
  },
  button: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    gap: Spacing.two,
    borderRadius: Radius.md,
  },
  buttonText: { fontWeight: '700' },
  iconButton: {
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: Radius.pill,
  },
  track: { width: '100%', overflow: 'hidden' },
  pill: {
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: Radius.pill,
    alignSelf: 'flex-start',
  },
  pillText: { fontSize: 12, fontWeight: '800' },
  chip: {
    paddingHorizontal: 14,
    height: 38,
    borderRadius: Radius.pill,
    borderWidth: 1,
    alignItems: 'center',
    justifyContent: 'center',
  },
  chipText: { fontSize: 14, fontWeight: '700' },
  stepper: {
    flexDirection: 'row',
    alignItems: 'center',
    borderRadius: Radius.pill,
    padding: 4,
    gap: 4,
    alignSelf: 'flex-start',
  },
  stepperValue: { textAlign: 'center', fontWeight: '800', fontVariant: ['tabular-nums'] },
  empty: {
    alignItems: 'center',
    paddingVertical: Spacing.eight,
    paddingHorizontal: Spacing.four,
    gap: Spacing.two,
  },
  emptyEmoji: { fontSize: 56, marginBottom: Spacing.two },
  emptyActions: { alignSelf: 'stretch', gap: Spacing.three, marginTop: Spacing.five },
  center: { textAlign: 'center' },
});
