import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../shared/providers/AuthContext';
import { Box, CircularProgress, Typography, Alert } from '@mui/material';
import authService from '../shared/services/auth';

const AuthCallback: React.FC = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const { setSession } = useAuth();
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const handleCallback = async () => {
            const errorParam = searchParams.get('error');

            if (errorParam) {
                console.error('Auth Error:', errorParam);
                setError(errorParam);
                setTimeout(() => navigate('/login', { replace: true }), 3000);
                return;
            }

            // After SSO callback, the backend has set an HttpOnly cookie
            // We don't need the token from URL params anymore
            // Just fetch the current user using the cookie
            try {
                const user = await authService.checkAuth();
                
                if (user) {
                    setSession(user);
                    // Use replace: true to avoid back-button loops
                    navigate('/', { replace: true });
                } else {
                    throw new Error('Failed to authenticate user');
                }
            } catch (err) {
                console.error('Failed to complete SSO authentication:', err);
                setError('Authentication failed. Please try again.');
                setTimeout(() => navigate('/login', { replace: true }), 3000);
            }
        };

        handleCallback();
    }, [searchParams, navigate, setSession]);

    if (error) {
        return (
            <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" height="100vh" p={3}>
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
                <Typography variant="body2" color="text.secondary">
                    Redirecting to login...
                </Typography>
            </Box>
        );
    }

    return (
        <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" height="100vh">
            <CircularProgress size={60} thickness={4} />
            <Typography variant="h6" sx={{ mt: 3, color: 'text.secondary' }}>
                Completing secure authentication...
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                Please wait while we verify your account
            </Typography>
        </Box>
    );
};

export default AuthCallback;