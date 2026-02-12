import React from 'react';
import { Box, Container, Typography, Link } from '@mui/material';
import { useBrand } from '../../providers/BrandContext';
import { useAppConstants } from '../../shared/providers/AppConstantsContext';

const Footer: React.FC = () => {
    const { brand } = useBrand();
    const { get } = useAppConstants();

    return (
        <Box sx={{
            mt: 'auto',
            py: 1.5,
            backgroundColor: brand.primaryColor,
            color: '#fff'
        }}>
            <Container maxWidth="sm">
                <Box textAlign="center">
                    <Typography variant="caption" sx={{ opacity: 0.7 }}>
                        © {new Date().getFullYear()}. {get('FOOTER_RIGHTS_RESERVED', 'All rights reserved.')} | {get('FOOTER_BUILT_BY', 'Built by')} <Link href="https://omega-technologies.in/" color="inherit" target="_blank" sx={{ textDecoration: 'underline', opacity: 0.9, '&:hover': { opacity: 1 } }}>{brand.builtBy}</Link>
                    </Typography>
                </Box>

            </Container>
        </Box>
    );
};

export default Footer;
