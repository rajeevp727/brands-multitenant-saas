import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { authService } from './services/authService';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Orders from './pages/Orders';
import Products from './pages/Products';
import Profile from './pages/Profile';

import BrandThemeSync from './components/BrandThemeSync';

import { useEffect, useState } from 'react';


function App() {
  const [isInitializing, setIsInitializing] = useState(true);


  // Sync theme with dashboard cookie (Heartbeat)
  useEffect(() => {
    const getCookie = (name) => {
      const value = `; ${document.cookie}`;
      const parts = value.split(`; ${name}=`);
      if (parts.length === 2) return parts.pop().split(';').shift();
      return null;
    };

    const syncTheme = () => {
      const mode = getCookie('saas-theme-mode');
      if (mode === 'dark') {
        document.body.classList.add('dark');
      } else {
        document.body.classList.remove('dark');
      }
    };

    // Initial sync
    syncTheme();

    // Heartbeat poll
    const interval = setInterval(syncTheme, 1000);
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    const initialize = async () => {
      try {
        // Check URL params first (SSO)
        const params = new URLSearchParams(window.location.search);
        const ssoParam = params.get('sso');

        if (ssoParam) {
          try {
            // Robust Base64 decode (Unicode safe)
            const base64 = decodeURIComponent(ssoParam).replace(/ /g, '+');
            const rawData = decodeURIComponent(escape(atob(base64)));
            const ssoData = JSON.parse(rawData);
            const { token } = ssoData;
            let user = ssoData.user;

            if (Array.isArray(user)) {
              user = user[0];
            }

            if (token) {
              localStorage.setItem('token', token);
              if (user) {
                localStorage.setItem('user', JSON.stringify(user));

                // SSO sync - call backend to ensure user exists
                try {
                  console.log('🔄 Syncing SSO user with BangaruKottu backend...');
                  authService.syncSsoUser(user).then(response => {
                    console.log('✅ SSO sync successful');
                  }).catch(err => {
                    console.error('❌ SSO sync failed:', err);
                  });
                } catch (err) {
                  console.error('❌ Sync call failed:', err);
                }
              }
            }

            // Clean URL
            const url = new URL(window.location.href);
            url.searchParams.delete('sso');
            window.history.replaceState({}, '', url.toString());
          } catch (e) {
            console.error('Failed to parse SSO data:', e);
          }
        }
      } finally {
        setIsInitializing(false);
      }
    };

    initialize();
  }, []);

  if (isInitializing) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500"></div>
      </div>
    );
  }

  return (
    <Router>
      <BrandThemeSync />
      <Routes>
        <Route
          path="/login"
          element={
            authService.isAuthenticated() ? <Navigate to="/dashboard" replace /> : <Login />
          }
        />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/orders" element={<Orders />} />
        <Route path="/products" element={<Products />} />
        <Route path="/profile" element={<Profile />} />
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </Router>
  );
}

export default App;

