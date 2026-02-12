import React from 'react';
import { Box, Typography, Container, Paper, Divider } from '@mui/material';
import { useBrand } from '../providers/BrandContext';

interface GenericPageProps {
    title: string;
    type: 'privacy' | 'terms' | 'support';
}

const MarkdownPage: React.FC<GenericPageProps> = ({ title, type }) => {
    const { brand } = useBrand();

    const getContent = () => {
        switch (type) {
            case 'privacy': return brand.privacyPolicy || '# Privacy Policy\n\nContact support for details.';
            case 'terms': return brand.termsOfService || '# Terms of Service\n\nStandard terms apply.';
            case 'support': return `# Support\n\nNeed help? Contact us at:\n\n- **Email**: ${brand.email}\n- **Phone**: ${brand.phone}`;
            default: return '';
        }
    };

    return (
        <Container maxWidth="md">
            <Box sx={{ py: 8 }}>
                <Paper elevation={0} sx={{ p: { xs: 3, md: 6 }, border: '1px solid #eee', borderRadius: 4 }}>
                    <Typography variant="h3" gutterBottom sx={{ color: brand.primaryColor, fontWeight: 'bold' }}>
                        {title}
                    </Typography>
                    <Divider sx={{ my: 4 }} />
                    <Box sx={{
                        '& h1': { color: brand.primaryColor, mb: 2 },
                        '& h2': { mt: 4, mb: 1 },
                        '& p': { mb: 2, lineHeight: 1.7, color: 'text.secondary' },
                        '& ul': { mb: 2, pl: 4 },
                        whiteSpace: 'pre-wrap'
                    }}>
                        {getContent()}
                    </Box>
                </Paper>
            </Box>
        </Container>
    );
};

export default MarkdownPage;
