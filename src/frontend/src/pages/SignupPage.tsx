import React, { useState } from 'react';
import { Box, Paper, TextField, Button, Typography, Container, Alert, CircularProgress, Link as MuiLink, IconButton, InputAdornment } from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useBrand } from '../providers/BrandContext';
import authService from '../shared/services/auth';
import { useNavigate, Link } from 'react-router-dom';

const SignupPage: React.FC = () => {
    const { brand } = useBrand();
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (password !== confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        setError('');
        setLoading(true);

        try {
            await authService.register({ username, email, password, confirmPassword });
            navigate('/');
        } catch (err: unknown) {
            const error = err as { response?: { data?: { message?: string } } };
            setError(error.response?.data?.message || 'Registration failed. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Container maxWidth="sm">
            <Box sx={{ mt: 8, mb: 8, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                <Paper elevation={0} sx={{ p: 5, width: '100%', border: '1px solid #eee', borderRadius: 4 }}>
                    <Box textAlign="center" mb={4}>
                        <Typography variant="h4" fontWeight="bold" sx={{ color: brand.primaryColor }}>
                            Join {brand.name}
                        </Typography>
                        <Typography variant="body2" color="textSecondary" sx={{ mt: 1 }}>
                            Create your account to get started
                        </Typography>
                    </Box>

                    {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

                    <form onSubmit={handleSubmit}>
                        <TextField
                            fullWidth
                            label="Username"
                            variant="outlined"
                            margin="normal"
                            required
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            disabled={loading}
                            placeholder="e.g. janesmith"
                        />
                        <TextField
                            fullWidth
                            label="Email Address"
                            variant="outlined"
                            margin="normal"
                            required
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            disabled={loading}
                            placeholder="e.g. jane@example.com"
                        />
                        <TextField
                            fullWidth
                            label="Password"
                            type={showPassword ? 'text' : 'password'}
                            variant="outlined"
                            margin="normal"
                            required
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            disabled={loading}
                            InputProps={{
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton
                                            aria-label="toggle password visibility"
                                            onClick={() => setShowPassword(!showPassword)}
                                            onMouseDown={(e) => e.preventDefault()}
                                            edge="end"
                                        >
                                            {showPassword ? <VisibilityOff /> : <Visibility />}
                                        </IconButton>
                                    </InputAdornment>
                                ),
                            }}
                        />
                        <TextField
                            fullWidth
                            label="Confirm Password"
                            type={showConfirmPassword ? 'text' : 'password'}
                            variant="outlined"
                            margin="normal"
                            required
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            disabled={loading}
                            InputProps={{
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton
                                            aria-label="toggle password visibility"
                                            onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                            onMouseDown={(e) => e.preventDefault()}
                                            edge="end"
                                        >
                                            {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
                                        </IconButton>
                                    </InputAdornment>
                                ),
                            }}
                        />
                        <Button
                            fullWidth
                            type="submit"
                            variant="contained"
                            size="large"
                            disabled={loading}
                            sx={{
                                mt: 4,
                                py: 1.5,
                                backgroundColor: brand.primaryColor,
                                '&:hover': { backgroundColor: brand.secondaryColor }
                            }}
                        >
                            {loading ? <CircularProgress size={24} color="inherit" /> : 'Create Account'}
                        </Button>
                    </form>

                    <Box mt={4} textAlign="center">
                        <Typography variant="body2" color="textSecondary">
                            Already have an account? <MuiLink component={Link} to="/login" sx={{ color: brand.primaryColor, fontWeight: 'bold' }}>Sign In</MuiLink>
                        </Typography>
                    </Box>
                </Paper>
            </Box>
        </Container>
    );
};

export default SignupPage;
