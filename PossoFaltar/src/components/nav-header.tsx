import { router } from 'expo-router';
import type { ReactNode } from 'react';
import { StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';

import { AppText, IconButton } from '@/components/ui';
import { MaxContentWidth, Spacing } from '@/constants/theme';
import { useTheme } from '@/hooks/use-theme';

/** Cabeçalho simples com botão de voltar, título e ação à direita. */
export function NavHeader({
  title,
  right,
  onBack = () => (router.canGoBack() ? router.back() : router.replace('/')),
  backIcon = 'back',
  safeTop = true,
}: {
  title?: string;
  right?: ReactNode;
  onBack?: () => void;
  backIcon?: 'back' | 'close';
  safeTop?: boolean;
}) {
  const theme = useTheme();
  const insets = useSafeAreaInsets();
  return (
    <View
      style={[
        styles.wrap,
        { paddingTop: (safeTop ? insets.top : 0) + Spacing.two, backgroundColor: theme.background },
      ]}>
      <View style={styles.row}>
        <IconButton
          icon={backIcon}
          accessibilityLabel={backIcon === 'back' ? 'Voltar' : 'Fechar'}
          onPress={onBack}
        />
        <AppText variant="heading" numberOfLines={1} style={styles.title}>
          {title ?? ''}
        </AppText>
        <View style={styles.right}>{right}</View>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  wrap: { paddingHorizontal: Spacing.five, paddingBottom: Spacing.two },
  row: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: Spacing.three,
    width: '100%',
    maxWidth: MaxContentWidth,
    alignSelf: 'center',
  },
  title: { flex: 1, textAlign: 'center' },
  right: { minWidth: 40, alignItems: 'flex-end' },
});
