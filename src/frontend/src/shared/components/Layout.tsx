import React, { useState } from 'react';
import { Box, useMediaQuery, useTheme, IconButton } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import Footer from './Footer';
import LeftSidebar from './LeftSidebar';

const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const [mobileOpen, setMobileOpen] = useState(false);
    const [collapsed, setCollapsed] = useState(true);

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

            <Box component="main" sx={{
                flexGrow: 1,
                display: 'flex',
                flexDirection: 'column',
                width: { md: `calc(100% - ${collapsed ? 70 : 260}px)` },
                transition: theme.transitions.create(['width', 'margin'], {
                    easing: theme.transitions.easing.sharp,
                    duration: theme.transitions.duration.enteringScreen,
                }),
            }}>
                {isMobile && (
                    <Box sx={{ p: 2, display: 'flex', alignItems: 'center' }}>
                        <IconButton
                            color="inherit"
                            aria-label="open drawer"
                            edge="start"
                            onClick={handleDrawerToggle}
                            sx={{ mr: 2 }}
                        >
                            <MenuIcon />
                        </IconButton>
                    </Box>
                )}

                <Box sx={{ flexGrow: 1 }}>
                    {children}
                </Box>
                <Footer />
            </Box>
        </Box>
    );
};

export default Layout;
