import { Analytics } from '@vercel/analytics/react';
import { CssBaseline } from '@mui/material';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { useEffect, useState } from 'react';
import SplashScreen from './components/common/SplashScreen';
import Layout from './components/layout/Layout';
import Dashboard from './pages/Dashboard';
import MarkdownPage from './pages/MarkdownPage';
import { BrandProvider } from './providers/BrandProvider';
import { useBrand } from './providers/BrandContext';
import { ThemeProvider } from './providers/ThemeProvider';
import { AppConstantsProvider } from './shared/providers/AppConstantsProvider';
import { AuthProvider } from './shared/providers/AuthProvider';

const AppContent = () => {
  const { loading: brandLoading } = useBrand();
  const [minLoadingTimePassed, setMinLoadingTimePassed] = useState(false);

  useEffect(() => {
    const timer = window.setTimeout(() => setMinLoadingTimePassed(true), 1800);
    return () => window.clearTimeout(timer);
  }, []);

  if (brandLoading || !minLoadingTimePassed) return <SplashScreen />;

  return (
    <ThemeProvider>
      <CssBaseline />
      <BrowserRouter>
        <Layout>
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/privacy" element={<MarkdownPage title="Privacy Policy" type="privacy" />} />
            <Route path="/terms" element={<MarkdownPage title="Terms of Service" type="terms" />} />
            <Route path="/support" element={<MarkdownPage title="Help & Support" type="support" />} />
          </Routes>
        </Layout>
        <Analytics />
      </BrowserRouter>
    </ThemeProvider>
  );
};

export default function App() {
  return (
    <AuthProvider>
      <BrandProvider>
        <AppConstantsProvider>
          <AppContent />
        </AppConstantsProvider>
      </BrandProvider>
    </AuthProvider>
  );
}
