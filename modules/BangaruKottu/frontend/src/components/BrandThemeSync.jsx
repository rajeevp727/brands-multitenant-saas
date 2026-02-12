import { useEffect } from 'react';

export default function BrandThemeSync() {
    useEffect(() => {
        const fetchBrand = async () => {
            try {
                // Check URL params first (SSO)
                const params = new URLSearchParams(window.location.search);
                const ssoParam = params.get('sso');
                let pColor = null;
                let sColor = null;

                if (ssoParam) {
                    try {
                        const ssoData = JSON.parse(atob(decodeURIComponent(ssoParam)));
                        pColor = ssoData.primaryColor;
                        sColor = ssoData.secondaryColor;
                    } catch (e) {
                        console.error('Failed to parse SSO data in ThemeSync:', e);
                    }
                }

                if (pColor) {
                    const decodedP = pColor; // No need to decode twice if coming from ssoData
                    const root = document.documentElement;
                    root.style.setProperty('--color-primary-50', `${decodedP}10`);
                    root.style.setProperty('--color-primary-500', decodedP);
                    root.style.setProperty('--color-primary-600', decodedP);
                    root.style.setProperty('--color-primary-700', decodedP);

                    if (sColor) {
                        // Set secondary if app supports it
                    }
                }

                const hostname = window.location.hostname;
                const tenantId = hostname === 'localhost' ? 'rajeev-pvt' : 'bangaru';

                const response = await fetch('http://localhost:5114/api/brands/current', {
                    headers: {
                        'X-Tenant-Id': tenantId
                    }
                });
                if (response.ok) {
                    const data = await response.json();

                    if (data.primaryColor && !pColor) {
                        const root = document.documentElement;
                        root.style.setProperty('--color-primary-50', `${data.primaryColor}10`);
                        root.style.setProperty('--color-primary-500', data.primaryColor);
                        root.style.setProperty('--color-primary-600', data.primaryColor);
                        root.style.setProperty('--color-primary-700', data.primaryColor);
                    }

                    if (data.name) {
                        document.title = `${data.name} | BangaruKottu`;
                    }
                }
            } catch (error) {
                console.error('Failed to sync brand theme:', error);
            }
        };

        fetchBrand();
    }, []);

    return null;
}
