import React, { useState, useMemo } from 'react';
import { createTheme, ThemeProvider as MUIThemeProvider } from '@mui/material';
import type { PaletteMode } from '@mui/material';
import { useBrand } from './BrandContext';
import { ThemeContext } from './ThemeContext';

const getCookie = (name: string) => {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop()?.split(';').shift();
    return null;
};

const setCookie = (name: string, value: string) => {
    document.cookie = `${name}=${value}; path=/; max-age=31536000`; // 1 year
};

export const ThemeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { brand } = useBrand();
    // Persistent theme preference
    const [mode, setMode] = useState<PaletteMode>(() => {
        const cookieTheme = getCookie('saas-theme-mode');
        if (cookieTheme === 'light' || cookieTheme === 'dark') return cookieTheme;

        const saved = localStorage.getItem('themeMode');
        return (saved as PaletteMode) || 'light';
    });

    const toggleColorMode = () => {
        setMode((prevMode) => {
            const newMode = prevMode === 'light' ? 'dark' : 'light';
            localStorage.setItem('themeMode', newMode);
            setCookie('saas-theme-mode', newMode);

            // Toggle global dark class for CSS support
            if (newMode === 'dark') {
                document.body.classList.add('dark');
            } else {
                document.body.classList.remove('dark');
            }

            return newMode;
        });
    };

    // Ensure class is set on initial load
    React.useEffect(() => {
        if (mode === 'dark') {
            document.body.classList.add('dark');
        } else {
            document.body.classList.remove('dark');
        }
    }, [mode]);

    const theme = useMemo(
        () =>
            createTheme({
                palette: {
                    mode,
                    primary: {
                        main: brand?.primaryColor || '#1976d2',
                    },
                    secondary: {
                        main: brand?.secondaryColor || '#dc004e',
                    },
                    background: {
                        default: mode === 'light' ? '#f8f9fa' : '#121212',
                        paper: mode === 'light' ? '#ffffff' : '#1e1e1e',
                    },
                },
                typography: {
                    fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
                    h3: { fontWeight: 700 },
                    h5: { fontWeight: 600 },
                },
                shape: {
                    borderRadius: 8,
                },
                components: {
                    MuiButton: {
                        styleOverrides: {
                            root: {
                                textTransform: 'none',
                                fontWeight: 600,
                            },
                        },
                    },
                    MuiPaper: {
                        styleOverrides: {
                            root: {
                                backgroundImage: 'none',
                            },
                        },
                    },
                },
            }),
        [mode, brand]
    );

    return (
        <ThemeContext.Provider value={{ mode, toggleColorMode }}>
            <MUIThemeProvider theme={theme}>
                {children}
            </MUIThemeProvider>
        </ThemeContext.Provider>
    );
};
