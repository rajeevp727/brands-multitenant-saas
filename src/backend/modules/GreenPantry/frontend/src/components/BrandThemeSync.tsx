import { useEffect } from 'react';

const BrandThemeSync = () => {
    useEffect(() => {
        const fetchBrand = async () => {
            try {
                // Check URL params first (SSO)
                const params = new URLSearchParams(window.location.search);
                const pColor = params.get('primaryColor');
                const sColor = params.get('secondaryColor');

                if (pColor) {
                    const decodedP = decodeURIComponent(pColor);
                    const root = document.documentElement;
                    root.style.setProperty('--color-primary-50', `${decodedP}10`);
                    root.style.setProperty('--color-primary-100', `${decodedP}20`);
                    root.style.setProperty('--color-primary-200', `${decodedP}40`);
                    root.style.setProperty('--color-primary-300', `${decodedP}60`);
                    root.style.setProperty('--color-primary-400', `${decodedP}90`);
                    root.style.setProperty('--color-primary-500', decodedP);
                    root.style.setProperty('--color-primary-600', decodedP);
                    root.style.setProperty('--color-primary-700', decodedP);
                    root.style.setProperty('--color-primary-800', decodedP);
                    root.style.setProperty('--color-primary-900', decodedP);

                    if (sColor) {
                        const decodedS = decodeURIComponent(sColor);
                        root.style.setProperty('--color-secondary-50', `${decodedS}10`);
                        root.style.setProperty('--color-secondary-500', decodedS);
                        root.style.setProperty('--color-secondary-600', decodedS);
                    }
                    // We can return here if we trust the dashboard params completely, 
                    // or let the API call overwrite it later for consistency.
                    // For now, let's allow the API call to run as a backup/verification.
                }

                const hostname = window.location.hostname;
                const tenantId = hostname === 'localhost' ? 'rajeev-pvt' : 'greenpantry';

                const response = await fetch('http://localhost:5114/api/brands/current', {
                    headers: {
                        'X-Tenant-Id': tenantId
                    }
                });
                if (response.ok) {
                    const data = await response.json();

                    if (data.primaryColor && !pColor) { // Only apply if not already set by URL
                        const root = document.documentElement;
                        root.style.setProperty('--color-primary-50', `${data.primaryColor}10`);
                        root.style.setProperty('--color-primary-100', `${data.primaryColor}20`);
                        root.style.setProperty('--color-primary-200', `${data.primaryColor}40`);
                        root.style.setProperty('--color-primary-300', `${data.primaryColor}60`);
                        root.style.setProperty('--color-primary-400', `${data.primaryColor}90`);
                        root.style.setProperty('--color-primary-500', data.primaryColor);
                        root.style.setProperty('--color-primary-600', data.primaryColor);
                        root.style.setProperty('--color-primary-700', data.primaryColor);
                        root.style.setProperty('--color-primary-800', data.primaryColor);
                        root.style.setProperty('--color-primary-900', data.primaryColor);
                    }

                    if (data.secondaryColor && !sColor) {
                        const root = document.documentElement;
                        root.style.setProperty('--color-secondary-50', `${data.secondaryColor}10`);
                        root.style.setProperty('--color-secondary-500', data.secondaryColor);
                        root.style.setProperty('--color-secondary-600', data.secondaryColor);
                    }

                    if (data.name) {
                        document.title = `${data.name} | GreenPantry`;
                    }
                }
            } catch (error) {
                console.error('Failed to sync brand theme:', error);
            }
        };

        fetchBrand();
    }, []);

    return null;
};

export default BrandThemeSync;
