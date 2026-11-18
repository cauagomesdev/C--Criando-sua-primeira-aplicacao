/**
 * Paleta de cores do app, com suporte a modo claro e escuro.
 * https://docs.expo.dev/guides/color-schemes/
 */

import { Colors, type ThemeColors } from '@/constants/theme';
import { useColorScheme } from '@/hooks/use-color-scheme';

export function useTheme(): ThemeColors {
  const scheme = useColorScheme();
  return Colors[scheme === 'dark' ? 'dark' : 'light'];
}

export function useIsDark() {
  return useColorScheme() === 'dark';
}
