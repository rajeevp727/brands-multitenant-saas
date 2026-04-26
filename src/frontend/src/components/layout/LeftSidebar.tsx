import { useEffect } from 'react';
import { Drawer, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Divider, Box, IconButton, Tooltip, useTheme, alpha } from '@mui/material';
import { HomeRounded as HomeIcon, Login as LoginIcon, Logout as LogoutIcon, ChevronLeft as ChevronLeftIcon, ChevronRight as ChevronRightIcon, Brightness4, Brightness7 } from '@mui/icons-material';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../shared/providers/AuthContext';
import { useColorMode } from '../../providers/ThemeContext';

const drawerWidth = 260;
const collapsedWidth = 70;

export default function LeftSidebar({ open, collapsed, onClose, onToggleCollapse, variant = 'permanent' }: { open: boolean; collapsed: boolean; onClose: () => void; onToggleCollapse: () => void; variant?: 'permanent' | 'persistent' | 'temporary'; }) {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const theme = useTheme();
  const { mode, toggleColorMode } = useColorMode();
  const currentWidth = collapsed && variant !== 'temporary' ? collapsedWidth : drawerWidth;

  useEffect(() => {
    const onKey = (event: KeyboardEvent) => { if (event.key === 'Escape' && open) onClose(); };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [open, onClose]);

  const nav = (path: string) => { navigate(path); if (variant === 'temporary') onClose(); };

  return (
    <Drawer variant={variant} open={open} onClose={onClose} PaperProps={{ sx: { pointerEvents: 'auto' } }} sx={{ width: currentWidth, flexShrink: 0, whiteSpace: 'nowrap', '& .MuiDrawer-paper': { width: currentWidth, boxSizing: 'border-box', backgroundColor: mode === 'dark' ? alpha(theme.palette.background.default, .9) : alpha('#ffffff', .8), backdropFilter: 'blur(16px)', borderRight: `1px solid ${theme.palette.divider}`, overflowX: 'hidden' } }}>
      <Box sx={{ p: 2, display: 'flex', alignItems: 'center', justifyContent: collapsed ? 'center' : 'flex-end', borderBottom: `1px solid ${alpha(theme.palette.divider, .5)}`, minHeight: 64, mb: 1 }}>
        {variant !== 'temporary' && <Tooltip title={collapsed ? 'Expand Sidebar' : 'Collapse'}><IconButton onClick={(e) => { e.stopPropagation(); onToggleCollapse(); }} size="small">{collapsed ? <ChevronRightIcon fontSize="small" /> : <ChevronLeftIcon fontSize="small" />}</IconButton></Tooltip>}
      </Box>
      <List sx={{ px: 1.5, display: 'flex', flexDirection: 'column', gap: .5 }}>
        <ListItem disablePadding sx={{ display: 'block' }}>
          <Tooltip title={collapsed ? 'Dashboard' : ''} placement="right">
            <ListItemButton selected={location.pathname === '/'} onClick={(e) => { e.stopPropagation(); nav('/'); }} sx={{ minHeight: 48, justifyContent: collapsed ? 'center' : 'initial', px: 2.5, borderRadius: 3, color: location.pathname === '/' ? theme.palette.primary.main : theme.palette.text.secondary, bgcolor: location.pathname === '/' ? alpha(theme.palette.primary.main, .1) : 'transparent' }}>
              <ListItemIcon sx={{ minWidth: 0, mr: collapsed && variant !== 'temporary' ? 'auto' : 2, justifyContent: 'center', color: 'inherit' }}><HomeIcon /></ListItemIcon>
              <ListItemText primary="Dashboard" sx={{ opacity: collapsed && variant !== 'temporary' ? 0 : 1 }} />
            </ListItemButton>
          </Tooltip>
        </ListItem>
      </List>
      <Divider sx={{ my: 1, mx: 3, opacity: .5 }} />
      <List sx={{ px: 1.5, mt: 'auto', mb: 2 }}>
        <Box sx={{ mt: 2, display: 'flex', justifyContent: 'center' }}><Tooltip title={collapsed ? 'Toggle Theme' : ''} placement="right"><IconButton onClick={(e) => { e.stopPropagation(); toggleColorMode(); }} sx={{ bgcolor: alpha(theme.palette.text.primary, .05), width: collapsed ? 40 : '100%', borderRadius: 3 }}>{mode === 'dark' ? <Brightness7 fontSize="small" /> : <Brightness4 fontSize="small" />}{!collapsed && <Box component="span" sx={{ ml: 1.5, fontSize: '.9rem', fontWeight: 600 }}>{mode === 'dark' ? 'Light Mode' : 'Dark Mode'}</Box>}</IconButton></Tooltip></Box>
      </List>
    </Drawer>
  );
}
