import React from 'react';
import { Box, Typography, Avatar, IconButton, Paper, Button, Grid, alpha } from '@mui/material';
import { Close as CloseIcon, Add as AddIcon } from '@mui/icons-material';
import type { SavedAccount } from '../shared/providers/AuthContext';
import { useBrand } from '../providers/BrandContext';

interface SavedAccountsListProps {
    accounts: SavedAccount[];
    onLogin: (account: SavedAccount) => void;
    onRemove: (email: string) => void;
    onAddAccount: () => void;
}

const SavedAccountsList: React.FC<SavedAccountsListProps> = ({ accounts, onLogin, onRemove, onAddAccount }) => {
    const { brand } = useBrand();

    return (
        <Box sx={{ width: '100%', textAlign: 'center' }}>
            <Box mb={4}>
                <Typography variant="h5" fontWeight="bold" gutterBottom>
                    Welcome Back
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    Choose an account to login
                </Typography>
            </Box>

            <Grid container spacing={2} justifyContent="center" sx={{ mb: 4 }}>
                {accounts.map((account) => (
                    <Grid key={account.email}>
                        <Paper
                            elevation={0}
                            sx={{
                                p: 2,
                                display: 'flex',
                                flexDirection: 'column',
                                alignItems: 'center',
                                border: '1px solid',
                                borderColor: 'divider',
                                borderRadius: 3,
                                width: 140,
                                position: 'relative',
                                cursor: 'pointer',
                                transition: 'all 0.2s',
                                '&:hover': {
                                    borderColor: brand.primaryColor,
                                    boxShadow: `0 4px 12px ${alpha(brand.primaryColor, 0.1)}`,
                                    transform: 'translateY(-2px)'
                                }
                            }}
                            onClick={() => onLogin(account)}
                        >
                            <IconButton
                                size="small"
                                sx={{
                                    position: 'absolute',
                                    top: 4,
                                    right: 4,
                                    opacity: 0.6,
                                    '&:hover': { opacity: 1, color: 'error.main', backgroundColor: alpha('#ff0000', 0.1) }
                                }}
                                onClick={(e) => {
                                    e.stopPropagation();
                                    onRemove(account.email);
                                }}
                            >
                                <CloseIcon fontSize="small" sx={{ fontSize: 16 }} />
                            </IconButton>

                            <Avatar
                                src={account.avatarUrl}
                                alt={account.name}
                                sx={{
                                    width: 64,
                                    height: 64,
                                    mb: 1.5,
                                    bgcolor: brand.primaryColor,
                                    fontSize: '1.5rem'
                                }}
                            >
                                {account.name.charAt(0).toUpperCase()}
                            </Avatar>

                            <Typography variant="subtitle2" fontWeight="bold" noWrap sx={{ width: '100%' }}>
                                {account.name}
                            </Typography>
                            <Typography variant="caption" color="text.secondary" noWrap sx={{ width: '100%' }}>
                                {account.email}
                            </Typography>
                        </Paper>
                    </Grid>
                ))}
            </Grid>

            <Button
                variant="outlined"
                startIcon={<AddIcon />}
                onClick={onAddAccount}
                sx={{
                    borderRadius: 3,
                    textTransform: 'none',
                    borderColor: 'divider',
                    color: 'text.primary',
                    px: 3,
                    '&:hover': {
                        borderColor: brand.primaryColor,
                        backgroundColor: alpha(brand.primaryColor, 0.05)
                    }
                }}
            >
                Log into another account
            </Button>
        </Box>
    );
};

export default SavedAccountsList;
