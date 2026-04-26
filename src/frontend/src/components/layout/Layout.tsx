import React, { useState } from 'react';
import { Box, IconButton, Typography, useMediaQuery, useTheme } from '@mui/material';
import { Brightness4 as DarkModeIcon, Brightness7 as LightModeIcon, Home as HomeIcon } from '@mui/icons-material';
import Footer from './Footer';
import LeftSidebar from './LeftSidebar';
import NotificationBell from '../notifications/NotificationBell';
import { useBrand } from '../../providers/BrandContext';
import { useColorMode } from '../../providers/ThemeContext';

export default function Layout({ children }: { children: React.ReactNode }) {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [mobileOpen, setMobileOpen] = useState(false);
  const [collapsed, setCollapsed] = useState(true);
  const { brand } = useBrand();
  const { mode, toggleColorMode } = useColorMode();
  const isCorporate = !brand?.id || brand.id === 'default' || brand.id === 'rajeev-pvt' || brand.name?.toLowerCase().includes('rajeev');
  const title = isCorporate ? "Rajeev's Tech" : brand.name;

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <LeftSidebar open={isMobile ? mobileOpen : true} collapsed={collapsed} onClose={() => setMobileOpen(false)} onToggleCollapse={() => setCollapsed((v) => !v)} variant={isMobile ? 'temporary' : 'permanent'} />
      <Box component="main" sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column', height: '100vh', overflowY: 'auto', width: { md: `calc(100% - ${collapsed ? 70 : 260}px)` } }}>
        <Box sx={{ py: 1, px: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between', borderBottom: '1px solid rgba(0,0,0,.05)', position: 'sticky', top: 0, backgroundColor: 'background.paper', zIndex: theme.zIndex.appBar, backdropFilter: 'blur(8px)' }}>
          <Box />
          <Box onClick={() => { window.location.href = '/'; }} sx={{ cursor: 'pointer', position: isMobile ? 'static' : 'absolute', left: isMobile ? 'auto' : '50%', transform: isMobile ? 'none' : 'translateX(-50%)' }}>
            <Typography variant="h6" sx={{ fontWeight: 800, letterSpacing: '-.02em', color: brand.primaryColor }}>{title}</Typography>
          </Box>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            {isMobile && <IconButton onClick={() => { window.location.href = '/'; }} size="small"><HomeIcon fontSize="small" /></IconButton>}
            {isMobile && <IconButton onClick={toggleColorMode} size="small">{mode === 'dark' ? <LightModeIcon fontSize="small" /> : <DarkModeIcon fontSize="small" />}</IconButton>}
            <NotificationBell />
          </Box>
        </Box>
        <Box sx={{ flexGrow: 1, pb: { xs: 8, md: 10 } }}>{children}</Box>
        <Box sx={{ position: 'fixed', bottom: 0, left: { md: collapsed ? 70 : 260 }, right: 0, zIndex: theme.zIndex.appBar, backgroundColor: 'background.paper', borderTop: '1px solid rgba(0,0,0,.05)' }}><Footer /></Box>
      </Box>
    </Box>
  );
}
