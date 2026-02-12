// Removed authService import as it's no longer needed for token retrieval

interface BrandData {
    tenantId: string;
    portalUrl?: string;
    configJson: string;
    primaryColor: string;
    secondaryColor: string;
    port?: string;
}

interface User {
    id: string;
    email: string;
    name?: string;
    username: string;
    tenantId: string;
    role?: string;
}

/**
 * Calculates the base URL for a brand based on portalUrl, configJson and port.
 */
export const calculateBaseUrl = (brand: BrandData, isProd: boolean): string => {
    let baseUrl = '';
    const config = JSON.parse(brand.configJson || '{}');
    const port = brand.port || config.port;

    // 1. In Local Development, prioritize the current host and port if defined
    // This allows branding to work when accessing via IP (e.g. 192.168.x.x)
    if (!isProd && port) {
        return `${window.location.protocol}//${window.location.hostname}:${port}/`;
    }

    if (brand.portalUrl) {
        try {
            let urlToParse = brand.portalUrl;
            if (!urlToParse.includes('://')) urlToParse = 'https://' + urlToParse;

            const urlObj = new URL(urlToParse);

            if (config.url) {
                baseUrl = config.url.endsWith('/') ? config.url : config.url + '/';
            } else if (urlObj.hostname !== 'localhost' && urlObj.hostname !== '127.0.0.1') {
                baseUrl = `${urlObj.protocol}//${urlObj.hostname}/`;
            } else {
                baseUrl = urlObj.origin + '/';
            }
        } catch (e) {
            baseUrl = config.url || brand.portalUrl || '';
            if (baseUrl && !baseUrl.endsWith('/')) baseUrl += '/';
        }
    }

    if (!baseUrl) {
        if (!isProd) {
            baseUrl = `http://localhost/`;
        } else {
            const subdomain = config.vercel || 'default';
            baseUrl = `https://${subdomain}.vercel.app/`;
        }
    }

    return baseUrl;
};

/**
 * Generates the final URL with SSO parameters.
 * Note: With the move to secure HttpOnly cookies, we no longer pass the JWT in the URL.
 * The destination domain will rely on its own session or a secure handshake.
 */
export const generateFinalUrl = (baseUrl: string, brand: BrandData, user: User | null): string => {
    if (!user) return baseUrl;

    const ssoData = {
        user: {
            id: user.id,
            email: user.email,
            name: user.name,
            username: user.username,
            tenantId: user.tenantId || brand.tenantId,
            role: user.role
        },
        primaryColor: brand.primaryColor,
        secondaryColor: brand.secondaryColor,
        timestamp: Date.now()
    };

    const jsonStr = JSON.stringify(ssoData);
    const ssoParam = btoa(unescape(encodeURIComponent(jsonStr)));

    return baseUrl.includes('?')
        ? `${baseUrl}&sso=${encodeURIComponent(ssoParam)}`
        : `${baseUrl}?sso=${encodeURIComponent(ssoParam)}`;
};
