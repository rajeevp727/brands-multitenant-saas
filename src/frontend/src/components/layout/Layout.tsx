import React, { useState } from 'react';
import { Box, useMediaQuery, useTheme, IconButton, Typography } from '@mui/material';

import Footer from './Footer';
import LeftSidebar from './LeftSidebar';
import NotificationBell from '../notifications/NotificationBell';
import { useBrand } from '../../providers/BrandContext';
import {
    ShoppingCart as ShoppingCartIcon,
    Brightness4 as DarkModeIcon,
    Brightness7 as LightModeIcon,
    ExitToApp as LogoutIcon
} from '@mui/icons-material';
import { useAuth } from '../../shared/providers/AuthContext';
import { useColorMode } from '../../providers/ThemeContext';

const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    // ... existing hooks
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const [mobileOpen, setMobileOpen] = useState(false);
    const [collapsed, setCollapsed] = useState(true);
    const { brand } = useBrand();
    const { user, logout } = useAuth();
    const { mode, toggleColorMode } = useColorMode();

    const handleDrawerToggle = () => {
        setMobileOpen(!mobileOpen);
    };

    const handleToggleCollapse = () => {
        setCollapsed(!collapsed);
    };

    return (
        <Box sx={{ display: 'flex', minHeight: '100vh' }}>
            <LeftSidebar
                open={isMobile ? mobileOpen : true}
                collapsed={collapsed}
                onClose={handleDrawerToggle}
                onToggleCollapse={handleToggleCollapse}
                variant={isMobile ? 'temporary' : 'permanent'}
            />

            <Box
                component="main"
                onClick={() => {
                    if (!collapsed && !isMobile) {
                        setCollapsed(true);
                    }
                }}
                sx={{
                    flexGrow: 1,
                    display: 'flex',
                    flexDirection: 'column',
                    height: '100vh',
                    overflowY: 'auto',
                    overflowX: 'hidden',
                    width: { md: `calc(100% - ${collapsed ? 70 : 260}px)` },
                    cursor: (!collapsed && !isMobile) ? 'pointer' : 'default',
                    transition: theme.transitions.create(['width', 'margin'], {
                        easing: theme.transitions.easing.sharp,
                        duration: theme.transitions.duration.enteringScreen,
                    }),
                }}
            >
                <Box
                    sx={{
                        py: 1,
                        px: 2,
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'space-between',
                        borderBottom: '1px solid rgba(0,0,0,0.05)',
                        position: 'sticky',
                        top: 0,
                        backgroundColor: 'background.paper',
                        zIndex: theme.zIndex.appBar,
                        backdropFilter: 'blur(8px)',
                    }}
                >
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    </Box>

                    {/* Brand Name / Logo */}
                    <Box
                        onClick={() => window.location.href = '/'}
                        sx={{
                            cursor: 'pointer',
                            display: 'flex',
                            alignItems: 'center',
                            flexGrow: isMobile ? 1 : 0,
                            position: isMobile ? 'static' : 'absolute',
                            left: isMobile ? 'auto' : '50%',
                            transform: isMobile ? 'none' : 'translateX(-50%)',
                            maxWidth: isMobile ? 'calc(100% - 140px)' : { sm: '300px' },
                            '&:hover': { opacity: 0.8 }
                        }}
                    >
                        <Typography
                            variant="h6"
                            component="div"
                            sx={{
                                fontWeight: 800,
                                letterSpacing: '-0.02em',
                                color: brand.primaryColor,
                                whiteSpace: 'nowrap',
                                overflow: 'hidden',
                                textOverflow: 'ellipsis',
                                fontSize: { xs: '1rem', md: '1.25rem' }
                            }}
                        >
                            {brand?.name ?? "Rajeev's Pvt. Ltd."}
                        </Typography>
                    </Box>

                    <Box sx={{ display: 'flex', alignItems: 'center', gap: { xs: 0.5, sm: 1.5 } }}>
                        {/* Desktop Username (Hidden on XS) */}
                        {user && !isMobile && (
                            <Typography
                                variant="body2"
                                sx={{
                                    fontWeight: 600,
                                    color: 'text.primary',
                                    mr: 1,
                                    display: { xs: 'none', sm: 'block' }
                                }}
                            >
                                {user.username || user.email?.split('@')[0]}
                            </Typography>
                        )}

                        {/* Theme Toggle (Mobile Only) */}
                        {isMobile && (
                            <IconButton
                                onClick={toggleColorMode}
                                sx={{ color: 'text.secondary' }}
                                size="small"
                            >
                                {mode === 'dark' ? <LightModeIcon fontSize="small" /> : <DarkModeIcon fontSize="small" />}
                            </IconButton>
                        )}

                        {/* Shopping Cart (Mobile Only) */}
                        {isMobile && (
                            <IconButton
                                onClick={() => window.location.href = '/products'}
                                sx={{ color: 'text.secondary' }}
                                size="small"
                            >
                                <ShoppingCartIcon fontSize="small" />
                            </IconButton>
                        )}

                        <NotificationBell />

                        {/* Logout Button (Mobile Only) */}
                        {user && isMobile && (
                            <IconButton
                                onClick={logout}
                                sx={{ color: 'error.light', ml: 0.5 }}
                                size="small"
                            >
                                <LogoutIcon sx={{ fontSize: 20 }} />
                            </IconButton>
                        )}
                    </Box>
                </Box>

                <Box sx={{
                    flexGrow: 1,
                    pb: { xs: 8, md: 10 } // Add padding for fixed footer
                }}>
                    {children}
                </Box>

                <Box sx={{
                    position: 'fixed',
                    bottom: 0,
                    left: { md: collapsed ? 70 : 260 },
                    right: 0,
                    zIndex: theme.zIndex.appBar,
                    backgroundColor: 'background.paper',
                    borderTop: '1px solid rgba(0,0,0,0.05)',
                    transition: theme.transitions.create(['left'], {
                        easing: theme.transitions.easing.sharp,
                        duration: theme.transitions.duration.enteringScreen,
                    }),
                }}>
                    <Footer />
                </Box>
            </Box>
        </Box>
    );
};

export default Layout;
