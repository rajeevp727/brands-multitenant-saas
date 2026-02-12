import { createContext, useContext } from 'react';

export interface BrandConfig {
    id: string;
    name: string;
    slogan: string;
    description: string;
    primaryColor: string;
    secondaryColor: string;
    logoUrl: string;
    email: string;
    phone: string;
    privacyPolicy?: string;
    termsOfService?: string;
    builtBy: string;
    features: string[];
}

export const defaultBrand: BrandConfig = {
    id: 'default',
    name: 'Rajeev\'s Pvt. Ltd.',
    slogan: 'Empowering your business',
    description: 'A multi-tenant white-label solution.',
    primaryColor: '#1976d2',
    secondaryColor: '#dc004e',
    logoUrl: '/logo.png',
    email: 'support@saas.com',
    phone: '+1 (000) 000-0000',
    builtBy: 'OmegaTechnologies Pvt. Ltd.',
    features: ['products', 'orders']
};

export const BrandContext = createContext<{
    brand: BrandConfig;
    loading: boolean;
}>({ brand: defaultBrand, loading: true });

export const useBrand = () => useContext(BrandContext);
