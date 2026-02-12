import React, { useState } from 'react';
import { Box, Paper, TextField, Button, Typography, Container, Alert, CircularProgress, Link as MuiLink, IconButton, InputAdornment, alpha } from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useBrand } from '../providers/BrandContext';
import { useAuth } from '../shared/providers/AuthContext';
import { Link, useNavigate } from 'react-router-dom';
import SavedAccountsList from '../components/SavedAccountsList';

const LoginPage: React.FC = () => {
    const { brand } = useBrand();
    const { login, savedAccounts, removeAccount, setSession } = useAuth();
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    // Show stored accounts if available
    const [showStoredAccounts, setShowStoredAccounts] = useState(savedAccounts.length > 0);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            await login(email, password);
            navigate('/');
        } catch (err: unknown) {
            const error = err as { response?: { data?: { message?: string } } };
            setError(error.response?.data?.message || 'Login failed. Please check your credentials.');
        } finally {
            setLoading(false);
        }
    };

    const handleAccountSelect = (account: any) => {
        // Direct Login: Use stored token to restore session
        if (account.token) {
            // Mock Token Guard: Do not allow using mock tokens from saved accounts
            if (account.token.startsWith('mock_')) {
                console.warn('⚠️ Saved account has mock token. Purging...');
                removeAccount(account.email);
                setEmail(account.email);
                setShowStoredAccounts(false);
                return;
            }

            const user = {
                id: account.id,
                email: account.email,
                name: account.name,
                username: account.email.split('@')[0], // derived if missing
                token: account.token,
                avatarUrl: account.avatarUrl,
                tenantId: account.tenantId || 'default',
                role: account.role || 'User'
            };
            // Restore session
            setSession(user);
            navigate('/', { replace: true });
        } else {
            // Fallback if no token (shouldn't happen for saved accounts)
            setEmail(account.email);
            setShowStoredAccounts(false);
        }
    };

    return (
        <Box sx={{
            minHeight: '100vh',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            background: () => `linear-gradient(135deg, ${alpha(brand.primaryColor, 0.05)} 0%, ${alpha(brand.secondaryColor, 0.05)} 100%)`,
            pt: { xs: 4, md: 0 },
            pb: { xs: 4, md: 0 }
        }}>
            <Container maxWidth="sm">
                <Paper elevation={0} sx={{
                    p: { xs: 4, md: 6 },
                    width: '100%',
                    border: '1px solid',
                    borderColor: 'divider',
                    borderRadius: 6,
                    backgroundColor: 'background.paper',
                    boxShadow: '0 20px 40px rgba(0,0,0,0.05)',
                    position: 'relative',
                    overflow: 'hidden',
                    '&::before': {
                        content: '""',
                        position: 'absolute',
                        top: 0,
                        left: 0,
                        width: '100%',
                        height: 4,
                        backgroundColor: brand.primaryColor
                    }
                }}>
                    {error && <Alert severity="error" sx={{ mb: 4, borderRadius: 2 }}>{error}</Alert>}

                    {showStoredAccounts && savedAccounts.length > 0 ? (
                        <SavedAccountsList
                            accounts={savedAccounts}
                            onLogin={handleAccountSelect}
                            onRemove={removeAccount}
                            onAddAccount={() => setShowStoredAccounts(false)}
                        />
                    ) : (
                        <>
                            <Box textAlign="center" mb={4}>
                                <Typography variant="h4" fontWeight="bold" gutterBottom sx={{ fontSize: { xs: '1.75rem', md: '2.125rem' } }}>
                                    Welcome Back
                                </Typography>
                                <Typography variant="body1" color="text.secondary" sx={{ fontSize: { xs: '0.9rem', md: '1rem' } }}>
                                    Sign in to your account
                                </Typography>
                            </Box>

                            {/* DEV HELPER: Show Credentials */}
                            {import.meta.env.DEV && (
                                <Alert severity="info" sx={{ mb: 1 }}>
                                    <Box display="flex">
                                        <Typography variant="body2">
                                            <b>Admin Dev Credentials:</b> admin@rajeev.com / Pass123
                                        </Typography>
                                    </Box>
                                </Alert>
                            )}

                            <form onSubmit={handleSubmit}>
                                <Box sx={{ mb: 3 }}>
                                    <Typography variant="caption" sx={{ fontWeight: 700, mb: 1, display: 'block', color: 'text.secondary', textTransform: 'uppercase' }}>
                                        Email Address
                                    </Typography>
                                    <TextField
                                        fullWidth
                                        variant="outlined"
                                        required
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                        disabled={loading}
                                        placeholder="name@company.com"
                                        sx={{
                                            '& .MuiOutlinedInput-root': {
                                                borderRadius: 3,
                                                backgroundColor: alpha(brand.primaryColor, 0.02),
                                                '&:hover': {
                                                    backgroundColor: alpha(brand.primaryColor, 0.04),
                                                }
                                            }
                                        }}
                                    />
                                </Box>

                                <Box sx={{ mb: 1 }}>
                                    <Typography variant="caption" sx={{ fontWeight: 700, mb: 1, display: 'block', color: 'text.secondary', textTransform: 'uppercase' }}>
                                        Password
                                    </Typography>
                                    <TextField
                                        fullWidth
                                        type={showPassword ? 'text' : 'password'}
                                        variant="outlined"
                                        required
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                        disabled={loading}
                                        placeholder="••••••••"
                                        sx={{
                                            '& .MuiOutlinedInput-root': {
                                                borderRadius: 3,
                                                backgroundColor: alpha(brand.primaryColor, 0.02),
                                                '&:hover': {
                                                    backgroundColor: alpha(brand.primaryColor, 0.04),
                                                }
                                            }
                                        }}
                                        InputProps={{
                                            endAdornment: (
                                                <InputAdornment position="end">
                                                    <IconButton
                                                        onClick={() => setShowPassword(!showPassword)}
                                                        edge="end"
                                                        size="small"
                                                    >
                                                        {showPassword ? <VisibilityOff fontSize="small" /> : <Visibility fontSize="small" />}
                                                    </IconButton>
                                                </InputAdornment>
                                            ),
                                        }}
                                    />
                                </Box>

                                <Box sx={{ textAlign: 'right', mb: 4 }}>
                                    <MuiLink component={Link} to="/forgot-password" sx={{ fontSize: '0.85rem', color: 'text.secondary', textDecoration: 'none', '&:hover': { color: brand.primaryColor } }}>
                                        Forgot password?
                                    </MuiLink>
                                </Box>

                                <Button
                                    fullWidth
                                    type="submit"
                                    variant="contained"
                                    size="large"
                                    disabled={loading}
                                    sx={{
                                        py: 1.8,
                                        borderRadius: 3,
                                        backgroundColor: brand.primaryColor,
                                        boxShadow: `0 8px 20px ${alpha(brand.primaryColor, 0.3)}`,
                                        fontSize: '1rem',
                                        '&:hover': {
                                            backgroundColor: brand.primaryColor,
                                            transform: 'translateY(-2px)',
                                            boxShadow: `0 12px 24px ${alpha(brand.primaryColor, 0.4)}`,
                                        },
                                        transition: 'all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275)'
                                    }}
                                >
                                    {loading ? <CircularProgress size={24} color="inherit" /> : 'Continue'}
                                </Button>
                            </form>

                            {/* Social Login Section */}
                            <Box sx={{ mt: 4, mb: 2 }}>
                                <Typography variant="caption" display="block" textAlign="center" color="text.secondary" sx={{ mb: 2 }}>
                                    OR CONTINUE WITH
                                </Typography>
                                <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center' }}>
                                    <Button
                                        fullWidth
                                        variant="outlined"
                                        startIcon={
                                            <Box component="svg" viewBox="0 0 24 24" sx={{ width: 20, height: 20 }}>
                                                <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4" />
                                                <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853" />
                                                <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05" />
                                                <path d="M12 5.38c1.62 0 3.09.56 4.23 1.65l3.18-3.18C17.46 2.02 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335" />
                                            </Box>
                                        }
                                        sx={{
                                            py: 1.2,
                                            borderColor: 'divider',
                                            color: 'text.primary',
                                            borderRadius: 3,
                                            textTransform: 'none',
                                            maxWidth: '300px'
                                        }}
                                        onClick={() => window.location.href = `${import.meta.env.VITE_API_URL || '/api'}/auth/login/google`}
                                    >
                                        Sign in with Google
                                    </Button>
                                </Box>
                            </Box>

                            <Box mt={3} textAlign="center">
                                <Typography variant="body2" color="textSecondary" sx={{ fontWeight: 500 }}>
                                    New here? <MuiLink component={Link} to="/signup" sx={{ color: brand.primaryColor, fontWeight: 700, textDecoration: 'none', ml: 0.5 }}>Create Account</MuiLink>
                                </Typography>
                                {savedAccounts.length > 0 && (
                                    <Button
                                        onClick={() => setShowStoredAccounts(true)}
                                        sx={{ mt: 2, textTransform: 'none', color: 'text.secondary', fontSize: '0.85rem' }}
                                    >
                                        Switch to list view
                                    </Button>
                                )}
                            </Box>
                        </>
                    )}
                </Paper>
            </Container>
        </Box>
    );
};

export default LoginPage;
