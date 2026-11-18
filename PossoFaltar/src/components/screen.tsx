import type { ReactNode } from 'react';
import { Platform, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

import { AppText } from '@/components/ui';
import { MaxContentWidth, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';

/** Espaço extra no fim das telas com abas, para o conteúdo não ficar atrás da barra. */
const TAB_BAR_SPACE = Platform.select({ ios: 70, android: 96, default: 24 });

/** Tela rolável com margens seguras e largura máxima (fica bonito no celular e na web). */
export function Screen({
  children,
  withTabBar = true,
  header,
  topInset = true,
}: {
  children: ReactNode;
  withTabBar?: boolean;
  header?: ReactNode;
  topInset?: boolean;
}) {
  const theme = useTheme();
  const insets = useSafeAreaInsets();
  return (
    <View style={[styles.root, { backgroundColor: theme.background }]}>
      {header}
      <ScrollView
        contentInsetAdjustmentBehavior="never"
        keyboardShouldPersistTaps="handled"
        keyboardDismissMode="on-drag"
        automaticallyAdjustKeyboardInsets
        contentContainerStyle={[
          styles.content,
          {
            paddingTop: (topInset && !header ? insets.top : 0) + Spacing.four,
            paddingBottom: insets.bottom + (withTabBar ? TAB_BAR_SPACE : Spacing.eight),
          },
        ]}>
        <View style={styles.inner}>{children}</View>
      </ScrollView>
    </View>
  );
}

/** Título grande das telas principais. */
export function LargeTitle({
  eyebrow,
  title,
  subtitle,
  right,
}: {
  eyebrow?: string;
  title: string;
  subtitle?: string;
  right?: ReactNode;
}) {
  return (
    <View style={styles.titleRow}>
      <View style={styles.titleText}>
        {eyebrow && (
          <AppText variant="caption" color="textSecondary">
            {eyebrow}
          </AppText>
        )}
        <AppText variant="title">{title}</AppText>
        {subtitle && (
          <AppText color="textSecondary" style={styles.subtitle}>
            {subtitle}
          </AppText>
        )}
      </View>
      {right}
    </View>
  );
}

const styles = StyleSheet.create({
  root: { flex: 1 },
  content: { paddingHorizontal: Spacing.five, alignItems: 'center' },
  inner: { width: '100%', maxWidth: MaxContentWidth },
  titleRow: {
    flexDirection: 'row',
    alignItems: 'flex-end',
    justifyContent: 'space-between',
    gap: Spacing.three,
    marginBottom: Spacing.five,
  },
  titleText: { flex: 1, gap: 2 },
  subtitle: { marginTop: 2 },
});
