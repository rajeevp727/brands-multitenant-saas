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
export const calculateBaseUrl = (brand: BrandData): string => {
    const isLocal = window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1';
    const config = JSON.parse(brand.configJson || '{}');

    // 🚀 LOCAL DEVELOPMENT OVERRIDES
    if (isLocal) {
        // If a specific port is provided in brand data or config
        const port = brand.port || config.port;
        if (port) return `http://localhost:${port}/`;

        // Hardcoded development mappings based on tenantId
        const localPorts: Record<string, string> = {
            'greenpantry': '5174',
            'omega': '5175',
            'bangaru': '5176'
        };

        if (localPorts[brand.tenantId]) {
            return `http://localhost:${localPorts[brand.tenantId]}/`;
        }

        // Fallback to localhost URL if it already exists in config
        if (config.url?.includes('localhost')) {
            return config.url.endsWith('/') ? config.url : config.url + '/';
        }
    }

    // 🌍 PRODUCTION RESOLUTION
    // ✅ 1. ALWAYS DB URL FIRST (if it's not a localhost URL in a production environment)
    if (config.url && (!isLocal || !config.url.includes('localhost'))) {
        return config.url.endsWith('/') ? config.url : config.url + '/';
    }

    // ✅ 2. domain fallback from config
    if (config.domain) {
        return `https://${config.domain}/`;
    }

    // ✅ 3. portalUrl fallback
    if (brand.portalUrl) {
        let url = brand.portalUrl;
        if (!url.startsWith('http')) {
            url = 'https://' + url;
        }
        return url.endsWith('/') ? url : url + '/';
    }

    // ✅ 3. LAST fallback (only if nothing exists)
    const subdomain = config.vercel || brand.tenantId || 'default';
    return `https://${subdomain}.vercel.app/`;
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
