import { SymbolView } from 'expo-symbols';
import type { ComponentProps } from 'react';
import type { ColorValue, StyleProp, ViewStyle } from 'react-native';

type SymbolName = ComponentProps<typeof SymbolView>['name'];
type NameMap = Exclude<SymbolName, string>;

/**
 * Ícones do app: SF Symbols no iOS e Material Symbols no Android/web.
 */
const ICONS = {
  today: { ios: 'sun.max.fill', android: 'wb_sunny', web: 'wb_sunny' },
  subjects: { ios: 'books.vertical.fill', android: 'menu_book', web: 'menu_book' },
  grades: { ios: 'graduationcap.fill', android: 'school', web: 'school' },
  plus: { ios: 'plus', android: 'add', web: 'add' },
  check: { ios: 'checkmark', android: 'check', web: 'check' },
  close: { ios: 'xmark', android: 'close', web: 'close' },
  back: { ios: 'chevron.left', android: 'arrow_back', web: 'arrow_back' },
  chevron: { ios: 'chevron.right', android: 'chevron_right', web: 'chevron_right' },
  edit: { ios: 'pencil', android: 'edit', web: 'edit' },
  trash: { ios: 'trash', android: 'delete', web: 'delete' },
  calendar: { ios: 'calendar', android: 'calendar_month', web: 'calendar_month' },
  clock: { ios: 'clock', android: 'schedule', web: 'schedule' },
  person: { ios: 'person.fill', android: 'person', web: 'person' },
  sparkles: { ios: 'sparkles', android: 'auto_awesome', web: 'auto_awesome' },
  minus: { ios: 'minus', android: 'remove', web: 'remove' },
  warning: {
    ios: 'exclamationmark.triangle.fill',
    android: 'warning',
    web: 'warning',
  },
  history: {
    ios: 'clock.arrow.circlepath',
    android: 'history',
    web: 'history',
  },
  undo: { ios: 'arrow.uturn.backward', android: 'undo', web: 'undo' },
} satisfies Record<string, NameMap>;

export type IconName = keyof typeof ICONS;

export function Icon({
  name,
  size = 20,
  color,
  style,
}: {
  name: IconName;
  size?: number;
  color: ColorValue;
  style?: StyleProp<ViewStyle>;
}) {
  return (
    <SymbolView
      name={ICONS[name]}
      size={size}
      tintColor={color}
      weight="semibold"
      style={style}
    />
  );
}
