import React, { useEffect } from 'react';
import { Drawer, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Divider, Box, IconButton, useTheme, alpha, Tooltip } from '@mui/material';
import {
    ShoppingCart as ProductsIcon,
    Login as LoginIcon,
    Logout as LogoutIcon,
    ChevronLeft as ChevronLeftIcon,
    ChevronRight as ChevronRightIcon
} from '@mui/icons-material';
import { useNavigate, useLocation } from 'react-router-dom';
import { useBrand } from '../../providers/BrandContext';
import { useAuth } from '../../shared/providers/AuthContext';
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
    const { user, logout } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();
    const theme = useTheme();
    const { mode, toggleColorMode } = useColorMode();

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
        logout();
        navigate('/login');
    };

    const handlePaperClick = () => {
        if (variant !== 'temporary' && onToggleCollapse) {
            onToggleCollapse();
        }
    };

    return (
        <Drawer
            variant={variant}
            open={open}
            onClose={onClose}
            PaperProps={{
                onClick: handlePaperClick,
                sx: { pointerEvents: 'auto' } // Ensure clicks are captured
            }}
            sx={{
                width: currentWidth,
                flexShrink: 0,
                whiteSpace: 'nowrap',
                '& .MuiDrawer-paper': {
                    width: currentWidth,
                    boxSizing: 'border-box',
                    backgroundColor: mode === 'dark'
                        ? alpha(theme.palette.background.default, 0.9)
                        : alpha('#ffffff', 0.8),
                    backdropFilter: 'blur(16px)',
                    borderRight: `1px solid ${theme.palette.divider}`,
                    overflowX: 'hidden',
                    transition: theme.transitions.create('width', {
                        easing: theme.transitions.easing.sharp,
                        duration: theme.transitions.duration.enteringScreen,
                    }),
                    boxShadow: mode === 'light' ? '4px 0 24px rgba(0,0,0,0.02)' : 'none'
                },
            }}
        >
            <Box sx={{
                p: 2,
                display: 'flex',
                alignItems: 'center',
                justifyContent: collapsed ? 'center' : 'space-between',
                borderBottom: `1px solid ${alpha(theme.palette.divider, 0.5)}`,
                minHeight: 64,
                mb: 1
            }}>
                {!collapsed && user && (
                    <Box sx={{ overflow: 'hidden', mr: 1 }}>
                        <div style={{
                            fontWeight: 700,
                            fontSize: '0.95rem',
                            whiteSpace: 'nowrap',
                            overflow: 'hidden',
                            textOverflow: 'ellipsis',
                            color: mode === 'dark' ? '#fff' : '#000'
                        }}>
                            {user.username || user.email?.split('@')[0]}
                        </div>
                        <div style={{
                            fontSize: '0.75rem',
                            color: theme.palette.text.secondary,
                            whiteSpace: 'nowrap',
                            overflow: 'hidden',
                            textOverflow: 'ellipsis'
                        }}>
                            {user.email}
                        </div>
                    </Box>
                )}

                {/* Logo Area - Stop propagation to prevent toggle if user clicks logo logic later */}
                {variant !== 'temporary' && (
                    <Tooltip title={collapsed ? "Expand Sidebar" : "Collapse"}>
                        <IconButton
                            onClick={(e) => {
                                e.stopPropagation();
                                onToggleCollapse();
                            }}
                            size="small"
                            sx={{
                                color: theme.palette.text.secondary,
                                bgcolor: alpha(theme.palette.action.hover, 0.1),
                                '&:hover': { bgcolor: alpha(theme.palette.action.hover, 0.2) }
                            }}
                        >
                            {collapsed ? <ChevronRightIcon fontSize="small" /> : <ChevronLeftIcon fontSize="small" />}
                        </IconButton>
                    </Tooltip>
                )}
            </Box>

            <List sx={{ px: 1.5, gap: 0.5, display: 'flex', flexDirection: 'column' }}>
                {(brand.features.includes('products') && (variant as string) !== 'temporary') && (
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
                                    borderRadius: 3,
                                    mb: 0.5,
                                    color: location.pathname === '/products' ? brand.primaryColor : theme.palette.text.secondary,
                                    bgcolor: location.pathname === '/products' ? alpha(brand.primaryColor, 0.1) : 'transparent',
                                    '&:hover': {
                                        bgcolor: alpha(brand.primaryColor, 0.08),
                                        color: brand.primaryColor
                                    }
                                }}
                            >
                                <ListItemIcon sx={{ minWidth: 0, mr: (collapsed && (variant as string) !== 'temporary') ? 'auto' : 2, justifyContent: 'center', color: 'inherit' }}>
                                    <ProductsIcon />
                                </ListItemIcon>
                                <ListItemText primary="Products" sx={{ opacity: (collapsed && (variant as string) !== 'temporary') ? 0 : 1 }} primaryTypographyProps={{ fontWeight: 600, fontSize: '0.95rem' }} />
                            </ListItemButton>
                        </Tooltip>
                    </ListItem>
                )}
            </List>

            <Divider sx={{ my: 1, mx: 3, opacity: 0.5 }} />

            <List sx={{ px: 1.5, mt: 'auto', mb: 2 }}>
                {(true) && (
                    !user ? (
                        <ListItem disablePadding sx={{ display: 'block' }}>
                            <Tooltip title={collapsed ? "Join / Sign In" : ""} placement="right">
                                <ListItemButton
                                    onClick={(e) => {
                                        e.stopPropagation();
                                        handleNavigation('/login');
                                    }}
                                    sx={{
                                        minHeight: 48,
                                        justifyContent: collapsed ? 'center' : 'initial',
                                        px: 2.5,
                                        borderRadius: 3,
                                        bgcolor: alpha(theme.palette.primary.main, 0.1),
                                        color: theme.palette.primary.main,
                                        '&:hover': {
                                            bgcolor: alpha(theme.palette.primary.main, 0.2),
                                        },
                                    }}
                                >
                                    <ListItemIcon sx={{ minWidth: 0, mr: (collapsed && (variant as string) !== 'temporary') ? 'auto' : 2, justifyContent: 'center', color: 'inherit' }}>
                                        <LoginIcon />
                                    </ListItemIcon>
                                    <ListItemText primary="Join / Sign In" sx={{ opacity: (collapsed && (variant as string) !== 'temporary') ? 0 : 1, fontWeight: 700 }} />
                                </ListItemButton>
                            </Tooltip>
                        </ListItem>
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
                                        borderRadius: 3,
                                        color: theme.palette.error.main,
                                        '&:hover': {
                                            bgcolor: alpha(theme.palette.error.main, 0.08)
                                        }
                                    }}
                                >
                                    <ListItemIcon sx={{ minWidth: 0, mr: (collapsed && (variant as string) !== 'temporary') ? 'auto' : 2, justifyContent: 'center', color: 'inherit' }}><LogoutIcon /></ListItemIcon>
                                    <ListItemText primary="Logout" sx={{ opacity: (collapsed && (variant as string) !== 'temporary') ? 0 : 1, fontWeight: 600 }} />
                                </ListItemButton>
                            </Tooltip>
                        </ListItem>
                    )
                )}

                {(true) && (
                    <Box sx={{ mt: 2, display: 'flex', justifyContent: 'center' }}>
                        <Tooltip title={collapsed ? "Toggle Theme" : ""} placement="right">
                            <IconButton
                                onClick={(e) => {
                                    e.stopPropagation();
                                    toggleColorMode();
                                }}
                                sx={{
                                    bgcolor: alpha(theme.palette.text.primary, 0.05),
                                    width: collapsed ? 40 : '100%',
                                    borderRadius: 3,
                                    '&:hover': { bgcolor: alpha(theme.palette.text.primary, 0.1) }
                                }}
                            >
                                {mode === 'dark' ? <Brightness7 fontSize="small" /> : <Brightness4 fontSize="small" />}
                                {!collapsed && <Box component="span" sx={{ ml: 1.5, fontSize: '0.9rem', fontWeight: 600 }}>{mode === 'dark' ? 'Light Mode' : 'Dark Mode'}</Box>}
                            </IconButton>
                        </Tooltip>
                    </Box>
                )}
            </List>
        </Drawer>
    );
};

export default LeftSidebar;
