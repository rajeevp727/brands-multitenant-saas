import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { authService } from '../shared/services/auth';

// Mock the api module
vi.mock('../services/api', () => {
    const mockApi = {
        get: vi.fn(),
        post: vi.fn(),
        interceptors: {
            request: { use: vi.fn() },
            response: { use: vi.fn() },
        },
    };
    return { default: mockApi };
});

import api from '../services/api';

describe('AuthService', () => {
    beforeEach(() => {
        vi.clearAllMocks();
        localStorage.clear();
    });

    afterEach(() => {
        localStorage.clear();
    });

    // ── checkAuth ───────────────────────────────────────────────────────────────

    describe('checkAuth()', () => {
        it('returns user when /auth/me succeeds', async () => {
            const mockUser = {
                userId: 'u-1',
                email: 'admin@rajeev.com',
                username: 'Admin',
                tenantId: 'rajeev-pvt',
                role: 'Admin',
            };
            vi.mocked(api.get).mockResolvedValueOnce({ data: mockUser });

            const user = await authService.checkAuth();

            expect(user).not.toBeNull();
            expect(user?.email).toBe('admin@rajeev.com');
            expect(user?.role).toBe('Admin');
        });

        it('returns null silently when /auth/me returns 401 (not logged in)', async () => {
            // 401 is expected on page load when unauthenticated — must not throw
            vi.mocked(api.get).mockRejectedValueOnce({ response: { status: 401 } });

            const user = await authService.checkAuth();

            expect(user).toBeNull();
        });

        it('clears localStorage on 401 during checkAuth', async () => {
            localStorage.setItem('saas_user_data', JSON.stringify({ email: 'old@user.com' }));
            vi.mocked(api.get).mockRejectedValueOnce({ response: { status: 401 } });

            await authService.checkAuth();

            expect(localStorage.getItem('saas_user_data')).toBeNull();
        });
    });

    // ── login ────────────────────────────────────────────────────────────────────

    describe('login()', () => {
        it('stores user data in localStorage on successful login', async () => {
            const mockResponse = {
                data: {
                    user: { id: 'u-1', email: 'admin@rajeev.com', username: 'Admin', tenantId: 'rajeev-pvt' },
                    token: 'jwt-token-abc',
                    refreshToken: 'refresh-xyz',
                },
            };
            vi.mocked(api.post).mockResolvedValueOnce(mockResponse);

            const result = await authService.login({ email: 'admin@rajeev.com', password: 'Pass123' });

            expect(result.token).toBe('jwt-token-abc');
            expect(result.user.email).toBe('admin@rajeev.com');
            expect(localStorage.getItem('saas_user_data')).not.toBeNull();
            expect(localStorage.getItem('access_token')).toBe('jwt-token-abc');
            expect(localStorage.getItem('refresh_token')).toBe('refresh-xyz');
        });

        it('throws on login failure (401)', async () => {
            vi.mocked(api.post).mockRejectedValueOnce({
                response: { status: 401, data: { message: 'Invalid credentials' } }
            });

            await expect(
                authService.login({ email: 'wrong@email.com', password: 'WrongPass' })
            ).rejects.toBeDefined();
        });

        it('does NOT store anything on login failure', async () => {
            vi.mocked(api.post).mockRejectedValueOnce({ response: { status: 401 } });

            try {
                await authService.login({ email: 'bad@email.com', password: 'bad' });
            } catch {
                // expected
            }

            expect(localStorage.getItem('saas_user_data')).toBeNull();
            expect(localStorage.getItem('access_token')).toBeNull();
        });
    });

    // ── logout ───────────────────────────────────────────────────────────────────

    describe('logout()', () => {
        it('clears all localStorage keys on logout', async () => {
            localStorage.setItem('saas_user_data', JSON.stringify({ email: 'admin@rajeev.com' }));
            localStorage.setItem('access_token', 'jwt-token');
            localStorage.setItem('user', JSON.stringify({ token: 'jwt-token' }));

            vi.mocked(api.post).mockResolvedValueOnce({ data: {} });

            await authService.logout();

            expect(localStorage.getItem('saas_user_data')).toBeNull();
            expect(localStorage.getItem('access_token')).toBeNull();
            expect(localStorage.getItem('user')).toBeNull();
        });

        it('clears localStorage even if API logout call fails', async () => {
            localStorage.setItem('saas_user_data', JSON.stringify({ email: 'admin@rajeev.com' }));
            vi.mocked(api.post).mockRejectedValueOnce(new Error('Network Error'));

            await authService.logout(); // should not throw

            expect(localStorage.getItem('saas_user_data')).toBeNull();
        });
    });

    // ── getCurrentUser ────────────────────────────────────────────────────────────

    describe('getCurrentUser()', () => {
        it('returns user from localStorage', () => {
            const user = { id: 'u-1', email: 'admin@rajeev.com', username: 'Admin', tenantId: 'rajeev-pvt' };
            localStorage.setItem('saas_user_data', JSON.stringify(user));

            const result = authService.getCurrentUser();

            expect(result?.email).toBe('admin@rajeev.com');
        });

        it('returns null when no user in localStorage', () => {
            const result = authService.getCurrentUser();
            expect(result).toBeNull();
        });

        it('returns null on corrupted localStorage data', () => {
            localStorage.setItem('saas_user_data', '{broken-json}');
            const result = authService.getCurrentUser();
            expect(result).toBeNull();
        });
    });

    // ── register ─────────────────────────────────────────────────────────────────

    describe('register()', () => {
        it('stores user and token on successful registration', async () => {
            const mockResponse = {
                data: {
                    user: { id: 'u-2', email: 'new@user.com', username: 'NewUser', tenantId: 'rajeev-pvt' },
                    token: 'new-token',
                    refreshToken: 'new-refresh',
                },
            };
            vi.mocked(api.post).mockResolvedValueOnce(mockResponse);

            const result = await authService.register({
                username: 'NewUser',
                email: 'new@user.com',
                password: 'SecurePass123',
                confirmPassword: 'SecurePass123',
            });

            expect(result.user.email).toBe('new@user.com');
            expect(localStorage.getItem('access_token')).toBe('new-token');
        });

        it('throws when email is already registered', async () => {
            vi.mocked(api.post).mockRejectedValueOnce({
                response: { status: 400, data: { message: 'Email already registered' } }
            });

            await expect(
                authService.register({ username: 'x', email: 'dup@email.com', password: 'p', confirmPassword: 'p' })
            ).rejects.toBeDefined();
        });
    });
});
