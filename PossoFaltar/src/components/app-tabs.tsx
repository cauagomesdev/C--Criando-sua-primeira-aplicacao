import { NativeTabs } from 'expo-router/native-tabs';

import { useTheme } from '@/hooks/use-theme';
import { usePendingToday } from '@/hooks/use-pending-today';

/** Abas nativas (iOS/Android). A versão web fica em app-tabs.web.tsx. */
export default function AppTabs() {
  const theme = useTheme();
  const pending = usePendingToday();

  return (
    <NativeTabs
      tintColor={theme.primary}
      backgroundColor={theme.tabBar}
      indicatorColor={theme.primarySoft}
      iconColor={{ default: theme.textSecondary, selected: theme.primary }}
      labelStyle={{
        default: { color: theme.textSecondary },
        selected: { color: theme.primary },
      }}
      badgeBackgroundColor={theme.danger}>
      <NativeTabs.Trigger name="index">
        <NativeTabs.Trigger.Label>Hoje</NativeTabs.Trigger.Label>
        <NativeTabs.Trigger.Icon sf="sun.max.fill" md="wb_sunny" />
        <NativeTabs.Trigger.Badge hidden={pending === 0}>{String(pending)}</NativeTabs.Trigger.Badge>
      </NativeTabs.Trigger>

      <NativeTabs.Trigger name="materias">
        <NativeTabs.Trigger.Label>Matérias</NativeTabs.Trigger.Label>
        <NativeTabs.Trigger.Icon sf="books.vertical.fill" md="menu_book" />
      </NativeTabs.Trigger>

      <NativeTabs.Trigger name="notas">
        <NativeTabs.Trigger.Label>Notas</NativeTabs.Trigger.Label>
        <NativeTabs.Trigger.Icon sf="graduationcap.fill" md="school" />
      </NativeTabs.Trigger>
    </NativeTabs>
  );
}
