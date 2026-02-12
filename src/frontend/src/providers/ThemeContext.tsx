import { createContext, useContext } from 'react';
import type { PaletteMode } from '@mui/material';

export interface ThemeContextType {
    mode: PaletteMode;
    toggleColorMode: () => void;
}

export const ThemeContext = createContext<ThemeContextType>({
    mode: 'light',
    toggleColorMode: () => { },
});

export const useColorMode = () => useContext(ThemeContext);
