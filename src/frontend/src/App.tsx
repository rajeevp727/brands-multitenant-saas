import { CssBaseline, Box, List, ListItem, ListItemText, Paper, Typography, Divider } from '@mui/material';
import SplashScreen from './components/common/SplashScreen';
import { BrandProvider } from './providers/BrandProvider';
import { useBrand } from './providers/BrandContext';
import { AppConstantsProvider } from './shared/providers/AppConstantsProvider';
import { ThemeProvider } from './providers/ThemeProvider';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { useEffect, useState } from 'react';
import api from './services/api';
import Layout from './components/layout/Layout';
import LoginPage from './pages/LoginPage';
import AuthCallback from './pages/AuthCallback';
import SignupPage from './pages/SignupPage';
import Dashboard from './pages/Dashboard';
import { AuthProvider } from './shared/providers/AuthProvider';
import MarkdownPage from './pages/MarkdownPage';

const AppContent = () => {
  const { brand, loading: brandLoading } = useBrand();
  const [products, setProducts] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [minLoadingTimePassed, setMinLoadingTimePassed] = useState(false);

  useEffect(() => {
    const timer = setTimeout(() => {
      setMinLoadingTimePassed(true);
    }, 2000);
    return () => clearTimeout(timer);
  }, []);

  useEffect(() => {
    if (!brandLoading) {
      api.get('/products')
        .then(res => {
          if (Array.isArray(res.data)) {
            setProducts(res.data);
          } else if (res.data && Array.isArray(res.data.items)) {
            setProducts(res.data.items);
          } else {
            setProducts([]);
          }
        })
        .catch(err => {
          console.error('Failed to fetch products', err);
          setProducts([]);
        })
        .finally(() => setLoading(false));
    }
  }, [brandLoading]);

  if (brandLoading || (loading && !brandLoading) || !minLoadingTimePassed) {
    return <SplashScreen />;
  }

  const isCorporate = brand?.id === 'rajeev-pvt' || brand?.name?.includes('Rajeev');

  return (
    <ThemeProvider>
      <CssBaseline />
      <BrowserRouter>
        <Layout>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/auth/callback" element={<AuthCallback />} />
            <Route path="/signup" element={<SignupPage />} />
            <Route path="/privacy" element={<MarkdownPage title="Privacy Policy" type="privacy" />} />
            <Route path="/terms" element={<MarkdownPage title="Terms of Service" type="terms" />} />
            <Route path="/support" element={<MarkdownPage title="Help & Support" type="support" />} />

            <Route path="/" element={
              isCorporate ? <Dashboard /> : (
                <Box p={{ xs: 2, md: 4 }} maxWidth={1000} mx="auto">
                  <Box mb={6} textAlign="center">
                    <Typography variant="h3" gutterBottom sx={{ color: 'primary.main', fontSize: { xs: '2rem', md: '3rem' } }}>
                      {brand.name}
                    </Typography>
                    <Typography variant="h6" color="textSecondary" gutterBottom sx={{ fontSize: { xs: '1rem', md: '1.25rem' } }}>
                      {brand.slogan}
                    </Typography>
                    <Divider sx={{ my: 3, width: '100px', mx: 'auto', borderBottomWidth: 3, borderColor: 'primary.main' }} />
                    <Typography variant="body1" sx={{ maxWidth: 700, mx: 'auto', fontSize: { xs: '1rem', md: '1.1rem' }, color: 'text.secondary' }}>
                      {brand.description}
                    </Typography>
                  </Box>

                  <Paper elevation={0} sx={{ mt: 4, p: { xs: 2, md: 4 }, borderRadius: 4, border: '1px solid', borderColor: 'divider' }}>
                    <Typography variant="h5" gutterBottom sx={{ mb: 4, color: 'primary.main' }}>
                      Available Products
                    </Typography>
                    {(!Array.isArray(products) || products.length === 0) ? (
                      <Typography color="textSecondary">No products found for this tenant. Visit our store soon!</Typography>
                    ) : (
                      <List>
                        {products.map((p: any) => (
                          <ListItem key={p.id} divider sx={{ py: 3, px: 0 }}>
                            <ListItemText
                              primary={<Typography variant="h6" fontWeight="bold">{p.name}</Typography>}
                              secondary={
                                <Box sx={{ mt: 1 }}>
                                  <Typography variant="body2" color="textSecondary">{p.description}</Typography>
                                  <Typography variant="h6" color="primary" sx={{ mt: 1 }}>${p.price.toFixed(2)}</Typography>
                                </Box>
                              }
                            />
                          </ListItem>
                        ))}
                      </List>
                    )}
                  </Paper>
                </Box>
              )
            } />
          </Routes>
        </Layout>
      </BrowserRouter>
    </ThemeProvider>
  );
};

function App() {
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

export default App;
