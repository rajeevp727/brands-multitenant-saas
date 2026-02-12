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
        const response = await api.get('/brands/current');
        const data = response.data;

        const config: BrandConfig = {
          id: data.tenantId || 'default',
          name: data.name || 'Rajeev\'s Pvt. Ltd.',
          slogan: data.slogan || 'Empowering your business',
          description: data.description || 'A multi-tenant white-label solution.',
          primaryColor: data.primaryColor || '#1976d2',
          secondaryColor: data.secondaryColor || '#dc004e',
          logoUrl: data.logoUrl || '/logo.png',
          email: data.email || 'support@saas.com',
          phone: data.phone || '+1 (000) 000-0000',
          privacyPolicy: data.privacyPolicy,
          termsOfService: data.termsOfService,
          builtBy: data.builtBy || 'OmegaTechnologies Pvt Ltd.',
          // Feature mapping from configJson or categories
          features: data.configJson ? (JSON.parse(data.configJson).features || ['products']) : ['products']
        };

        setBrand(config);

        // Dynamically set CSS variables for theme and document title
        document.documentElement.style.setProperty('--primary-color', config.primaryColor);
        document.documentElement.style.setProperty('--secondary-color', config.secondaryColor);
        document.title = config.name;

        // Dynamically update favicon
        const link = document.querySelector("link[rel~='icon']") as HTMLLinkElement;
        if (link) {
          link.href = config.logoUrl;
        } else {
          const newLink = document.createElement('link');
          newLink.rel = 'icon';
          newLink.href = config.logoUrl;
          document.head.appendChild(newLink);
        }
      } catch (error) {
        console.error('Failed to fetch brand config, using defaults', error);
      } finally {
        setLoading(false);
      }
    };

    fetchBrand();
  }, []);

  return (
    <BrandContext.Provider value={{ brand, loading }}>
      {children}
    </BrandContext.Provider>
  );
};
