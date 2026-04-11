import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { BrandContext, defaultBrand } from './BrandContext';
import type { BrandConfig } from './BrandContext';

export const BrandProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [brand, setBrand] = useState<BrandConfig>(defaultBrand);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchBrand = async () => {
      try {
        const { data } = await api.get('/brands/current');
        const isCorporate = !data?.tenantId || data.tenantId === 'rajeev-pvt' || String(data?.name ?? '').toLowerCase().includes('rajeev');
        const nextBrand: BrandConfig = {
          id: data.tenantId || defaultBrand.id,
          name: isCorporate ? defaultBrand.name : (data.name || defaultBrand.name),
          slogan: isCorporate ? defaultBrand.slogan : (data.slogan || defaultBrand.slogan),
          description: data.description || defaultBrand.description,
          primaryColor: data.primaryColor || defaultBrand.primaryColor,
          secondaryColor: data.secondaryColor || defaultBrand.secondaryColor,
          logoUrl: data.logoUrl || defaultBrand.logoUrl,
          email: data.email || defaultBrand.email,
          phone: data.phone || defaultBrand.phone,
          privacyPolicy: data.privacyPolicy,
          termsOfService: data.termsOfService,
          builtBy: data.builtBy || defaultBrand.builtBy,
          features: ['dashboard']
        };
        setBrand(nextBrand);
        document.documentElement.style.setProperty('--primary-color', nextBrand.primaryColor);
        document.documentElement.style.setProperty('--secondary-color', nextBrand.secondaryColor);
        document.title = nextBrand.name;
      } catch (error) {
        console.error('Failed to fetch brand config, using defaults', error);
      } finally {
        setLoading(false);
      }
    };

    fetchBrand();
  }, []);

  return <BrandContext.Provider value={{ brand, loading }}>{children}</BrandContext.Provider>;
};
