import React, { useState, useEffect, useMemo } from 'react';
import { motion } from 'framer-motion';
import { ChevronRight } from 'lucide-react';
import { Snackbar, Alert } from '@mui/material';
import api from '../shared/services/api';
import { useAuth } from '../shared/providers/AuthContext';
import { useAppConstants } from '../shared/providers/AppConstantsContext';
import { calculateBaseUrl, generateFinalUrl } from '../shared/utils/urlHelper';
import './Dashboard.css';

interface BrandData {
    id: string;
    tenantId: string;
    name: string;
    description: string;
    logoUrl: string;
    primaryColor: string;
    secondaryColor: string;
    port?: string;
    configJson: string;
    slogan?: string;
    portalUrl?: string;
    sortOrder?: number;
    isVisible?: boolean;
}

interface RawBrandData {
    [key: string]: unknown;
    id?: string;
    Id?: string;
    tenantId?: string;
    TenantId?: string;
    name?: string;
    Name?: string;
    description?: string;
    Description?: string;
    slogan?: string;
    Slogan?: string;
    logoUrl?: string;
    LogoUrl?: string;
    primaryColor?: string;
    PrimaryColor?: string;
    secondaryColor?: string;
    SecondaryColor?: string;
    configJson?: string;
    ConfigJson?: string;
    portalUrl?: string;
    PortalUrl?: string;
    sortOrder?: number;
    SortOrder?: number;
    isVisible?: boolean;
    IsVisible?: boolean;
    isActive?: boolean;
    IsActive?: boolean;
}

/** Resolve logo URL: relative paths (e.g. /images/...) to current origin so all brands show images. */
function resolveLogoUrl(logoUrl: string | null | undefined): string {
    if (!logoUrl || !logoUrl.trim()) return '';
    const u = logoUrl.trim();
    if (u.startsWith('http://') || u.startsWith('https://')) return u;
    const origin = typeof window !== 'undefined' ? window.location.origin : '';
    return u.startsWith('/') ? `${origin}${u}` : `${origin}/${u}`;
}

const Dashboard: React.FC = () => {
    const { user } = useAuth();
    const { get: getConstant } = useAppConstants();
    const [brands, setBrands] = useState<BrandData[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [showWarning, setShowWarning] = useState(false);
    const [systemStatus, setSystemStatus] = useState<{ isDeploying: boolean }>({ isDeploying: false });


    const checkSystemStatus = async () => {
        try {
            const response = await api.get('/system/status');
            setSystemStatus(response.data);
        } catch (err) {
            console.error('Failed to check system status:', err);
            // If API fails, we assume it's deploying or offline (Render spin down/Vercel build)
            setSystemStatus({ isDeploying: true });
        }
    };

    useEffect(() => {
        const fetchBrands = async () => {
            try {
                setLoading(true);
                await checkSystemStatus();
                const response = await api.get('/brands');
                const rawData = response.data;
                const brandsArray = Array.isArray(rawData) ? rawData : (rawData?.items && Array.isArray(rawData.items) ? rawData.items : []);

                // Map the API response to our format
                const mappedBrands = brandsArray.map((brand: RawBrandData) => {
                    return {
                        id: brand.id || brand.Id || '',
                        tenantId: brand.tenantId || brand.TenantId || '',
                        name: brand.name || brand.Name || '',
                        description: brand.description || brand.Description || brand.slogan || brand.Slogan || '',
                        logoUrl: brand.logoUrl || brand.LogoUrl || '',
                        primaryColor: brand.primaryColor || brand.PrimaryColor || '#000000',
                        secondaryColor: brand.secondaryColor || brand.SecondaryColor || '#ffffff',
                        configJson: brand.configJson || brand.ConfigJson || '{}',
                        portalUrl: brand.portalUrl || brand.PortalUrl || '',
                        sortOrder: brand.sortOrder ?? brand.SortOrder ?? 999,
                        isVisible: brand.isActive ?? brand.IsActive ?? brand.isVisible ?? brand.IsVisible ?? true
                    };
                });

                setBrands(mappedBrands);
                setError(null);
            } catch (err) {
                console.error('Failed to fetch brands:', err);
                setError('Failed to load brands. Please try again later.');
            } finally {
                setLoading(false);
            }
        };

        fetchBrands();

        // Start polling every 30 seconds for system status
        const intervalId = setInterval(checkSystemStatus, 30000);
        return () => clearInterval(intervalId);
    }, []);

    // Optimization: Memoize filtered and sorted brands
    const displayBrands = useMemo(() => {
        // Filter out Rajeev's Pvt Ltd and any invisible brands
        const filtered = brands.filter(brand =>
            brand.isVisible !== false &&
            brand.tenantId !== 'rajeev-pvt'
        );
        return [...filtered].sort((a, b) => {
            const orderDiff = (a.sortOrder ?? 999) - (b.sortOrder ?? 999);
            if (orderDiff !== 0) return orderDiff;
            return a.name.localeCompare(b.name);
        });
    }, [brands]);

    const handleLaunch = (e: React.MouseEvent, url: string, brand?: BrandData) => {
        e.preventDefault();

        if (systemStatus.isDeploying || !user) {
            if (!user) setShowWarning(true);
            return;
        }

        // Handle "Coming Soon" brands
        const config = JSON.parse(brand?.configJson || '{}');
        if (config.status === 'coming_soon' || brand?.tenantId === 'morebrands') {
            return;
        }

        const target = "saas_portal_brand";
        const features = "noopener=0";

        window.open(url, target, features);
    };

    return (
        <div className="dashboard-container">
            <motion.div
                initial={{ opacity: 0, y: -20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.8, ease: "easeOut" }}
                className="content-wrapper mb-12"
            >
                {systemStatus.isDeploying && (
                    <motion.div
                        initial={{ opacity: 0, scale: 0.95 }}
                        animate={{ opacity: 1, scale: 1 }}
                        className="mb-8 p-4 bg-amber-500/10 border border-amber-500/20 rounded-2xl flex items-center justify-between backdrop-blur-sm"
                    >
                        <div className="flex items-center gap-3">
                            <div className="w-2 h-2 bg-amber-500 rounded-full animate-ping"></div>
                            <p className="text-amber-600 dark:text-amber-400 font-semibold">
                                System Update in Progress - Limited Access
                            </p>
                        </div>
                        <p className="text-sm text-amber-600/80 dark:text-amber-400/80 hidden md:block">
                            Services are being optimized. Portal launching is temporarily disabled.
                        </p>
                    </motion.div>
                )}
                {user ? (
                    <div className="text-center">
                        <motion.h1
                            initial={{ opacity: 0, scale: 0.9 }}
                            animate={{ opacity: 1, scale: 1 }}
                            transition={{ delay: 0.3, duration: 0.5 }}
                            className="text-lg md:text-xl font-black tracking-tighter text-gray-900 dark:text-white mb-0"
                        >
                            Welcome back, <span className="text-blue-600 dark:text-transparent dark:bg-clip-text dark:bg-gradient-to-r dark:from-blue-400 dark:to-emerald-400 font-bold">
                                {user?.username || (Array.isArray(user) ? user[0]?.name : user?.name) || user?.email?.split('@')[0] || 'Valued User'}
                            </span>!
                        </motion.h1>
                        <motion.p
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1 }}
                            transition={{ delay: 0.5, duration: 0.5 }}
                            className="text-gray-600 dark:text-gray-400 text-[10px] max-w-2xl mx-auto font-bold opacity-70"
                        >
                            Your personalized business ecosystem is ready.
                        </motion.p>
                    </div>
                ) : (
                    <div className="text-center">
                        <h1 className="text-3xl md:text-4xl font-black tracking-tight text-gray-900 dark:text-white mb-2">
                            Explore our Brands
                        </h1>
                        <p className="text-gray-600 dark:text-gray-400 text-base max-w-2xl mx-auto font-medium">
                            Sign in to access your business portals and manage your multi-tenant ecosystem.
                        </p>
                    </div>
                )}
            </motion.div>

            <motion.div
                initial={{ opacity: 0, scale: 0.95 }}
                animate={{ opacity: 1, scale: 1 }}
                transition={{ delay: 0.2, duration: 0.6 }}
                className="content-wrapper"
            >
                <div className="brand-grid">
                    {displayBrands.map((brand, index) => {
                        const isProd = import.meta.env.PROD;
                        const baseUrl = calculateBaseUrl(brand, isProd);
                        const finalUrl = generateFinalUrl(baseUrl, brand, user as any);
                        const resolvedLogo = resolveLogoUrl(brand.logoUrl);
                        const isMoreBrands = brand.tenantId === 'morebrands';

                        return (
                            <motion.div
                                key={brand.tenantId || index}
                                initial={{ opacity: 0, scale: 0.95 }}
                                animate={{ opacity: 1, scale: 1 }}
                                transition={{
                                    duration: 0.5,
                                    delay: index * 0.1,
                                    ease: [0.34, 1.56, 0.64, 1]
                                }}
                                className={`brand-card group ${!user || systemStatus.isDeploying ? 'cursor-not-allowed' : (isMoreBrands ? 'brand-card-placeholder' : 'cursor-pointer')}`}
                                onClick={(e) => handleLaunch(e, finalUrl, brand)}
                                role="button"
                                tabIndex={0}
                            >
                                {/* Logo Container: Centered and compact */}
                                <div className="absolute inset-0 pb-16 z-0 overflow-hidden flex items-center justify-center">
                                    {resolvedLogo ? (
                                        <img
                                            src={resolvedLogo}
                                            alt={brand.name}
                                            className={`max-w-[80%] max-h-[80%] object-contain transition-transform duration-700 ${user ? 'group-hover:scale-110' : ''} ${!user ? 'filter grayscale brightness-[0.5]' : ''}`}
                                            onError={(e) => {
                                                e.currentTarget.style.display = 'none';
                                                const parent = e.currentTarget.parentElement;
                                                if (parent) {
                                                    parent.innerHTML = `<div class="w-full h-full flex items-center justify-center bg-gray-900 text-white/10 font-black text-8xl">${(brand.name || '?').slice(0, 2).toUpperCase()}</div>`;
                                                }
                                            }}
                                        />
                                    ) : (
                                        <div
                                            className="w-full h-full flex items-center justify-center text-white/10 font-black text-8xl"
                                            style={{ backgroundColor: brand.primaryColor || '#1e293b' }}
                                        >
                                            {(brand.name || '?').slice(0, 2).toUpperCase()}
                                        </div>
                                    )}
                                </div>

                                {/* Dynamic Overlay for Readability */}
                                <div className={`brand-card-overlay ${!user ? 'bg-black/80' : 'bg-gradient-to-t from-black/90 via-transparent to-transparent group-hover:via-black/10'}`}></div>

                                {/* Content Layer */}
                                <div className="absolute inset-0 z-10 p-2 flex flex-col h-full">
                                    {/* Brand Info at bottom */}
                                    <div className="mt-auto">
                                        <div className="brand-description backdrop-blur-md bg-black/40 border border-white/10 p-2 rounded-lg">
                                            <p className="text-white/90 text-sm md:text-base font-medium leading-snug">
                                                {brand.description}
                                            </p>

                                            {!isMoreBrands && user && (
                                                <div className="mt-5 flex justify-center opacity-0 group-hover:opacity-100 transition-all duration-500 transform translate-y-3 group-hover:translate-y-0">
                                                    <button className="launch-portal-btn">
                                                        Launch Portal
                                                        <ChevronRight size={14} className="stroke-[3]" />
                                                    </button>
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                </div>
                            </motion.div>
                        );
                    })}
                </div>
            </motion.div>

            {showWarning && (
                <Snackbar
                    open={showWarning}
                    autoHideDuration={4000}
                    onClose={() => setShowWarning(false)}
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
                >
                    <Alert
                        onClose={() => setShowWarning(false)}
                        severity="warning"
                        variant="filled"
                        sx={{
                            width: '100%',
                            borderRadius: '1rem',
                            fontWeight: 600,
                            backgroundColor: '#f59e0b', // Amber-500
                            boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)'
                        }}
                    >
                        {getConstant('LOGIN_REQUIRED_MESSAGE', 'Please login to access our premium brands!')}
                    </Alert>
                </Snackbar>
            )}

            {error && (
                <Snackbar open autoHideDuration={6000} onClose={() => setError(null)}>
                    <Alert onClose={() => setError(null)} severity="error">
                        {error}
                    </Alert>
                </Snackbar>
            )}

            {loading && brands.length === 0 && (
                <div className="flex flex-col items-center justify-center p-12 gap-4">
                    <div className="w-12 h-12 border-4 border-blue-500/20 border-t-blue-500 rounded-full animate-spin"></div>
                    <p className="text-gray-500 font-medium animate-pulse">Loading business ecosystem...</p>
                </div>
            )}
        </div>
    );
};

export default Dashboard;
