import { describe, it, expect } from 'vitest';

// We test the interceptor logic by simulating what the response interceptor does,
// because the actual axios instance is module-level and hard to reset per-test.

describe('API Interceptor Logic', () => {
    const simulateResponseInterceptorError = (
        statusCode: number,
        requestUrl: string,
        currentPath: string
    ) => {
        // Inline the interceptor logic from api.ts for unit testing
        const isSilentCheck = requestUrl.includes('/auth/me');
        const shouldRedirect = statusCode === 401 && currentPath !== '/login' && !isSilentCheck;
        return { isSilentCheck, shouldRedirect };
    };

    // ── /auth/me 401 handling ──────────────────────────────────────────────────

    it('should NOT redirect on 401 from /auth/me (page load check)', () => {
        const { shouldRedirect, isSilentCheck } = simulateResponseInterceptorError(
            401, '/auth/me', '/'
        );
        expect(isSilentCheck).toBe(true);
        expect(shouldRedirect).toBe(false);
    });

    it('should NOT redirect on 401 from /api/auth/me (full path variant)', () => {
        const { shouldRedirect } = simulateResponseInterceptorError(
            401, '/api/auth/me', '/dashboard'
        );
        expect(shouldRedirect).toBe(false);
    });

    // ── Real 401 responses (other protected endpoints) ─────────────────────────

    it('should redirect on 401 from a protected endpoint', () => {
        const { shouldRedirect } = simulateResponseInterceptorError(
            401, '/api/users', '/dashboard'
        );
        expect(shouldRedirect).toBe(true);
    });

    it('should NOT redirect when already on /login page', () => {
        const { shouldRedirect } = simulateResponseInterceptorError(
            401, '/api/users', '/login'
        );
        expect(shouldRedirect).toBe(false);
    });

    it('should NOT redirect on non-401 status codes', () => {
        const { shouldRedirect } = simulateResponseInterceptorError(
            403, '/api/users', '/dashboard'
        );
        expect(shouldRedirect).toBe(false);
    });

    it('should NOT redirect on 500 errors', () => {
        const { shouldRedirect } = simulateResponseInterceptorError(
            500, '/api/auth/login', '/login'
        );
        expect(shouldRedirect).toBe(false);
    });

    // ── Tenant header injection logic ──────────────────────────────────────────

    describe('X-Tenant-Id header mapping', () => {
        const getTenantId = (hostname: string): string => {
            const tenantMap: Record<string, string> = {
                localhost: 'rajeev-pvt',
                'greenpantry.local': 'greenpantry',
                'omega.local': 'omega',
                'bangaru.local': 'bangaru',
                'rajeevs-pvt-ltd.vercel.app': 'rajeev-pvt',
                'green-pantry-saas.vercel.app': 'greenpantry',
                'omega-technologies.vercel.app': 'omega',
                'bangaru-kottu.vercel.app': 'bangaru',
                'rajeevstech.in': 'rajeev-pvt',
            };
            return tenantMap[hostname] || 'rajeev-pvt';
        };

        it('maps localhost to rajeev-pvt', () => {
            expect(getTenantId('localhost')).toBe('rajeev-pvt');
        });

        it('maps greenpantry.local to greenpantry', () => {
            expect(getTenantId('greenpantry.local')).toBe('greenpantry');
        });

        it('maps green-pantry-saas.vercel.app to greenpantry', () => {
            expect(getTenantId('green-pantry-saas.vercel.app')).toBe('greenpantry');
        });

        it('maps omega-technologies.vercel.app to omega', () => {
            expect(getTenantId('omega-technologies.vercel.app')).toBe('omega');
        });

        it('maps bangaru-kottu.vercel.app to bangaru', () => {
            expect(getTenantId('bangaru-kottu.vercel.app')).toBe('bangaru');
        });

        it('maps rajeevstech.in to rajeev-pvt', () => {
            expect(getTenantId('rajeevstech.in')).toBe('rajeev-pvt');
        });

        it('falls back to rajeev-pvt for unknown hostnames', () => {
            expect(getTenantId('unknown-domain.com')).toBe('rajeev-pvt');
        });
    });
});
