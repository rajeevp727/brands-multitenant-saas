
import React from 'react';
import { Box, Typography, keyframes, alpha } from '@mui/material';
import { useBrand } from '../../providers/BrandContext';
import { useTheme } from '@mui/material/styles';
import CorporateLogo from '../../images/RajeevsTech.png';

const pulse = keyframes`
  0% { transform: scale(0.95); opacity: 0.5; }
  50% { transform: scale(1.05); opacity: 1; }
  100% { transform: scale(0.95); opacity: 0.5; }
`;

const fadeIn = keyframes`
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
`;

const SplashScreen: React.FC = () => {
    const { brand } = useBrand();
    const theme = useTheme();

    return (
        <Box
            sx={{
                position: 'fixed',
                top: 0,
                left: 0,
                width: '100vw',
                height: '100dvh', // Dynamic viewport height for mobile
                minHeight: '-webkit-fill-available', // Fallback for some mobile browsers
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center',
                bgcolor: theme.palette.background.default,
                // Add full background image support for corporate branding
                ...(brand?.id === 'rajeev-pvt' || brand?.name?.includes('Rajeev') || !brand?.id ? {
                    backgroundImage: `linear-gradient(rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.4)), url(${CorporateLogo})`,
                    backgroundSize: 'cover',
                    backgroundPosition: 'center',
                    backgroundRepeat: 'no-repeat',
                } : {}),
                zIndex: 9999,
                transition: 'opacity 0.5s ease-out',
            }}
        >
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    gap: 3,
                    animation: `${fadeIn} 0.8s ease-out`
                }}
            >
                {/* Logo Area - Hide if using full background image for corporate brand */}
                {!(brand?.id === 'rajeev-pvt' || brand?.name?.includes('Rajeev') || !brand?.id) && (
                    <Box
                        sx={{
                            width: 120,
                            height: 120,
                            borderRadius: '24px',
                            bgcolor: alpha(brand?.primaryColor || theme.palette.primary.main, 0.1),
                            display: 'flex',
                            justifyContent: 'center',
                            alignItems: 'center',
                            boxShadow: `0 8px 32px ${alpha(brand?.primaryColor || theme.palette.primary.main, 0.2)}`,
                            animation: `${pulse} 2s infinite ease-in-out`
                        }}
                    >
                        {brand?.logoUrl ? (
                            <img
                                src={brand.logoUrl}
                                alt={brand.name}
                                style={{ width: '80%', height: '80%', objectFit: 'contain' }}
                            />
                        ) : (
                            <Typography
                                variant="h3"
                                sx={{
                                    fontWeight: 800,
                                    color: brand?.primaryColor || theme.palette.primary.main,
                                    background: `linear-gradient(135deg, ${brand?.primaryColor || theme.palette.primary.main}, ${alpha(brand?.primaryColor || theme.palette.primary.main, 0.5)})`,
                                    WebkitBackgroundClip: 'text',
                                    WebkitTextFillColor: 'transparent',
                                }}
                            >
                                {brand?.name?.charAt(0) || 'R'}
                            </Typography>
                        )}
                    </Box>
                )}
            </Box>
        </Box>
    );
};

export default SplashScreen;
