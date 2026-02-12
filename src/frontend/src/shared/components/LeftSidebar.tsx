import React, { useEffect } from 'react';
import { Drawer, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Divider, Box, IconButton, useTheme, Button, alpha, Tooltip } from '@mui/material';
import {
    Home as HomeIcon,
    ShoppingCart as ProductsIcon,
    Login as LoginIcon,
    PersonAdd as SignupIcon,
    Logout as LogoutIcon,
    ChevronLeft as ChevronLeftIcon,
    ChevronRight as ChevronRightIcon
} from '@mui/icons-material';
import { useNavigate, useLocation } from 'react-router-dom';
import { useBrand } from '../../providers/BrandContext';
import authService from '../services/auth';
import { useColorMode } from '../../providers/ThemeContext';
import { Brightness4, Brightness7 } from '@mui/icons-material';

const drawerWidth = 260;
const collapsedWidth = 70;

const LeftSidebar: React.FC<{
    open: boolean;
    collapsed: boolean;
    onClose: () => void;
    onToggleCollapse: () => void;
    variant?: 'permanent' | 'persistent' | 'temporary';
}> = ({ open, collapsed, onClose, onToggleCollapse, variant = 'permanent' }) => {
    const { brand } = useBrand();
    const navigate = useNavigate();
    const location = useLocation();
    const theme = useTheme();
    const { mode, toggleColorMode } = useColorMode();
    const [user, setUser] = React.useState(authService.getCurrentUser());

    // Close sidebar on ESC key
    useEffect(() => {
        const handleKeyDown = (event: KeyboardEvent) => {
            if (event.key === 'Escape' && open) {
                onClose();
            }
        };

        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [open, onClose]);

    const currentWidth = collapsed && variant !== 'temporary' ? collapsedWidth : drawerWidth;

    const handleNavigation = (path: string) => {
        navigate(path);
        if (variant === 'temporary') onClose();
    };

    const handleLogout = () => {
        authService.logout();
        setUser(null);
        navigate('/login');
    };

    return (
        <Drawer
            variant={variant}
            open={open}
            onClose={onClose}
            PaperProps={{
                onClick: () => {
                    if (onToggleCollapse) {
                        onToggleCollapse();
                    }
                }
            }}
            sx={{
                width: currentWidth,
                flexShrink: 0,
                whiteSpace: 'nowrap',
                '& .MuiDrawer-paper': {
                    width: currentWidth,
                    boxSizing: 'border-box',
                    backgroundColor: (theme) => alpha(theme.palette.background.default, 0.8),
                    backdropFilter: 'blur(12px)',
                    borderRight: `1px solid ${theme.palette.divider}`,
                    overflowX: 'hidden',
                    transition: theme.transitions.create('width', {
                        easing: theme.transitions.easing.sharp,
                        duration: theme.transitions.duration.enteringScreen,
                    }),
                },
            }}
        >
            <Box sx={{
                p: 2,
                display: 'flex',
                alignItems: 'center',
                justifyContent: collapsed ? 'center' : 'space-between',
                borderBottom: `1px solid ${theme.palette.divider}`,
                minHeight: 64
            }}>
                {variant !== 'temporary' && (
                    <Tooltip title={collapsed ? "Expand Sidebar" : "Collapse Sidebar"}>
                        <IconButton onClick={(e) => {
                            e.stopPropagation();
                            onToggleCollapse();
                        }}>
                            {collapsed ? <ChevronRightIcon /> : <ChevronLeftIcon />}
                        </IconButton>
                    </Tooltip>
                )}
                {variant === 'temporary' && (
                    <IconButton onClick={onClose}>
                        <ChevronLeftIcon />
                    </IconButton>
                )}
            </Box>

            <List sx={{ pt: 2 }}>
                <ListItem disablePadding sx={{ display: 'block' }}>
                    <Tooltip title={collapsed ? "Home" : ""} placement="right">
                        <ListItemButton
                            selected={location.pathname === '/'}
                            onClick={(e) => {
                                e.stopPropagation();
                                handleNavigation('/');
                            }}
                            sx={{
                                minHeight: 48,
                                justifyContent: collapsed ? 'center' : 'initial',
                                px: 2.5,
                                borderRadius: collapsed ? 0 : '0 24px 24px 0',
                                mr: collapsed ? 0 : 2
                            }}
                        >
                            <ListItemIcon sx={{ minWidth: 0, mr: collapsed ? 'auto' : 3, justifyContent: 'center' }}>
                                <HomeIcon color={location.pathname === '/' ? 'primary' : 'inherit'} />
                            </ListItemIcon>
                            <ListItemText primary="Home" sx={{ opacity: collapsed ? 0 : 1 }} />
                        </ListItemButton>
                    </Tooltip>
                </ListItem>

                {brand.features.includes('products') && (
                    <ListItem disablePadding sx={{ display: 'block' }}>
                        <Tooltip title={collapsed ? "Products" : ""} placement="right">
                            <ListItemButton
                                selected={location.pathname === '/products'}
                                onClick={(e) => {
                                    e.stopPropagation();
                                    handleNavigation('/products');
                                }}
                                sx={{
                                    minHeight: 48,
                                    justifyContent: collapsed ? 'center' : 'initial',
                                    px: 2.5,
                                    borderRadius: collapsed ? 0 : '0 24px 24px 0',
                                    mr: collapsed ? 0 : 2
                                }}
                            >
                                <ListItemIcon sx={{ minWidth: 0, mr: collapsed ? 'auto' : 3, justifyContent: 'center' }}>
                                    <ProductsIcon color={location.pathname === '/products' ? 'primary' : 'inherit'} />
                                </ListItemIcon>
                                <ListItemText primary="Products" sx={{ opacity: collapsed ? 0 : 1 }} />
                            </ListItemButton>
                        </Tooltip>
                    </ListItem>
                )}
            </List>

            <Divider sx={{ my: 2, mx: 2 }} />

            <List>
                {!user ? (
                    <>
                        <ListItem disablePadding sx={{ display: 'block' }}>
                            <Tooltip title={collapsed ? "Login" : ""} placement="right">
                                <ListItemButton
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        handleNavigation('/login');
                                    }}
                                    sx={{
                                        minHeight: 48,
                                        justifyContent: collapsed ? 'center' : 'initial',
                                        px: 2.5,
                                        borderRadius: collapsed ? 0 : '0 24px 24px 0',
                                        mr: collapsed ? 0 : 2
                                    }}
                                >
                                    <ListItemIcon sx={{ minWidth: 0, mr: collapsed ? 'auto' : 3, justifyContent: 'center' }}><LoginIcon /></ListItemIcon>
                                    <ListItemText primary="Login" sx={{ opacity: collapsed ? 0 : 1 }} />
                                </ListItemButton>
                            </Tooltip>
                        </ListItem>
                        <ListItem disablePadding sx={{ display: 'block' }}>
                            <Tooltip title={collapsed ? "Sign Up" : ""} placement="right">
                                <ListItemButton
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        handleNavigation('/signup');
                                    }}
                                    sx={{
                                        minHeight: 48,
                                        justifyContent: collapsed ? 'center' : 'initial',
                                        px: 2.5,
                                        borderRadius: collapsed ? 0 : '0 24px 24px 0',
                                        mr: collapsed ? 0 : 2
                                    }}
                                >
                                    <ListItemIcon sx={{ minWidth: 0, mr: collapsed ? 'auto' : 3, justifyContent: 'center' }}><SignupIcon /></ListItemIcon>
                                    <ListItemText primary="Sign Up" sx={{ opacity: collapsed ? 0 : 1 }} />
                                </ListItemButton>
                            </Tooltip>
                        </ListItem>
                    </>
                ) : (
                    <ListItem disablePadding sx={{ display: 'block' }}>
                        <Tooltip title={collapsed ? "Logout" : ""} placement="right">
                            <ListItemButton
                                onClick={(e) => {
                                    e.stopPropagation();
                                    handleLogout();
                                }}
                                sx={{
                                    minHeight: 48,
                                    justifyContent: collapsed ? 'center' : 'initial',
                                    px: 2.5,
                                    borderRadius: collapsed ? 0 : '0 24px 24px 0',
                                    mr: collapsed ? 0 : 2
                                }}
                            >
                                <ListItemIcon sx={{ minWidth: 0, mr: collapsed ? 'auto' : 3, justifyContent: 'center' }}><LogoutIcon /></ListItemIcon>
                                <ListItemText primary="Logout" sx={{ opacity: collapsed ? 0 : 1 }} />
                            </ListItemButton>
                        </Tooltip>
                    </ListItem>
                )}
            </List>

            <Box sx={{ mt: 'auto', p: 2, textAlign: 'center' }}>
                {collapsed ? (
                    <Tooltip title="Toggle Theme" placement="right">
                        <IconButton onClick={(e) => {
                            e.stopPropagation();
                            toggleColorMode();
                        }}>
                            {mode === 'dark' ? <Brightness7 /> : <Brightness4 />}
                        </IconButton>
                    </Tooltip>
                ) : (
                    <Button
                        startIcon={mode === 'dark' ? <Brightness7 /> : <Brightness4 />}
                        onClick={(e) => {
                            e.stopPropagation();
                            toggleColorMode();
                        }}
                        variant="outlined"
                        fullWidth
                        sx={{ borderRadius: 4 }}
                    >
                        {mode === 'dark' ? 'Light Mode' : 'Dark Mode'}
                    </Button>
                )}
            </Box>
        </Drawer>
    );
};

export default LeftSidebar;
