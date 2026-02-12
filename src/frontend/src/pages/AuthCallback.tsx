import React, { useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../shared/providers/AuthContext';
import { Box, CircularProgress, Typography } from '@mui/material';

const AuthCallback: React.FC = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const { setSession } = useAuth();

    useEffect(() => {
        const token = searchParams.get('token');
        const email = searchParams.get('email');
        const error = searchParams.get('error');

        if (error) {
            console.error('Auth Error:', error);
            navigate('/login');
            return;
        }

        if (token && email) {
            // Construct a more robust user object from the token info
            const nameParts = email.split('@')[0].split('.');
            const firstName = nameParts[0].charAt(0).toUpperCase() + nameParts[0].slice(1);
            const lastName = nameParts.length > 1
                ? nameParts[1].charAt(0).toUpperCase() + nameParts[1].slice(1)
                : '';

            const user = {
                username: email.split('@')[0],
                id: email, // Use email as ID for external login fallback
                name: `${firstName} ${lastName}`.trim(),
                firstName: firstName,
                lastName: lastName,
                email: email,
                role: 'User',
                token: token,
                tenantId: 'default', // SaaS Dashboard tenant
                avatarUrl: ''
            };

            setSession(user);
            // Use replace: true to avoid back-button loops
            navigate('/', { replace: true });
        } else {
            console.error('Missing token or email');
            navigate('/login', { replace: true });
        }
    }, [searchParams, navigate, setSession]);

    return (
        <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" height="100vh">
            <CircularProgress size={60} thickness={4} />
            <Typography variant="h6" sx={{ mt: 3, color: 'text.secondary' }}>
                Completing secure authentication...
            </Typography>
        </Box>
    );
};

export default AuthCallback;
