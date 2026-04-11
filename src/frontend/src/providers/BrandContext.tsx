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
  name: "Rajeev's Tech",
  slogan: 'Launch every brand from one dashboard',
  description: 'A multi-tenant SaaS command center for Rajeev\'s brand ecosystem.',
  primaryColor: '#0f766e',
  secondaryColor: '#f59e0b',
  logoUrl: '/logo.png',
  email: 'support@rajeevstech.in',
  phone: '+91 00000 00000',
  builtBy: "Rajeev's Tech",
  features: ['dashboard']
};

export const BrandContext = createContext({ brand: defaultBrand, loading: true });
export const useBrand = () => useContext(BrandContext);
