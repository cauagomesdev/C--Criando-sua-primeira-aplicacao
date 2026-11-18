import {
  TabList,
  TabSlot,
  TabTrigger,
  Tabs,
  type TabListProps,
  type TabTriggerSlotProps,
} from 'expo-router/ui';
import { Pressable, StyleSheet, Text, View } from 'react-native';

import { Icon, type IconName } from '@/components/icon';
import { MaxContentWidth } from '@/constants/theme';
import { usePendingToday } from '@/hooks/use-pending-today';
import { useTheme } from '@/hooks/use-theme';

/** Barra de abas para a versão web, imitando a barra inferior do celular. */
export default function AppTabs() {
  const pending = usePendingToday();
  return (
    <Tabs style={styles.root}>
      <TabSlot style={styles.slot} />
      <TabList asChild>
        <CustomTabList>
          <TabTrigger name="index" href="/" asChild>
            <TabButton icon="today" label="Hoje" badge={pending} />
          </TabTrigger>
          <TabTrigger name="materias" href="/materias" asChild>
            <TabButton icon="subjects" label="Matérias" />
          </TabTrigger>
          <TabTrigger name="notas" href="/notas" asChild>
            <TabButton icon="grades" label="Notas" />
          </TabTrigger>
        </CustomTabList>
      </TabList>
    </Tabs>
  );
}

function CustomTabList({ children, ...props }: TabListProps) {
  const theme = useTheme();
  return (
    <View
      {...props}
      style={[props.style, styles.bar, { backgroundColor: theme.tabBar, borderTopColor: theme.border }]}>
      <View style={styles.barInner}>{children}</View>
    </View>
  );
}

function TabButton({
  icon,
  label,
  badge,
  isFocused,
  ...props
}: Omit<TabTriggerSlotProps, 'ref'> & { icon: IconName; label: string; badge?: number }) {
  const theme = useTheme();
  const color = isFocused ? theme.primary : theme.textSecondary;
  return (
    <Pressable {...props} style={styles.tab}>
      <View
        style={[styles.iconWrap, isFocused && { backgroundColor: theme.primarySoft }]}>
        <Icon name={icon} size={22} color={color} />
        {!!badge && (
          <View style={[styles.badge, { backgroundColor: theme.danger }]}>
            <Text style={styles.badgeText}>{badge}</Text>
          </View>
        )}
      </View>
      <Text style={[styles.label, { color }]}>{label}</Text>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  root: { flex: 1 },
  slot: { flex: 1 },
  bar: { borderTopWidth: StyleSheet.hairlineWidth, paddingBottom: 6 },
  barInner: {
    flexDirection: 'row',
    width: '100%',
    maxWidth: MaxContentWidth,
    alignSelf: 'center',
  },
  tab: { flex: 1, alignItems: 'center', paddingTop: 8, gap: 2 },
  iconWrap: {
    width: 60,
    height: 32,
    borderRadius: 16,
    alignItems: 'center',
    justifyContent: 'center',
  },
  label: { fontSize: 12, fontWeight: '700' },
  badge: {
    position: 'absolute',
    top: -2,
    right: 10,
    minWidth: 18,
    height: 18,
    borderRadius: 9,
    paddingHorizontal: 4,
    alignItems: 'center',
    justifyContent: 'center',
  },
  badgeText: { color: '#fff', fontSize: 11, fontWeight: '800' },
});
