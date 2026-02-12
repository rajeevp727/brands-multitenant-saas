import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import { User } from '../types'

interface AuthState {
  user: User | null
  token: string | null
  refreshToken: string | null
  isAuthenticated: boolean
  isLoading: boolean
  isInitializing: boolean
  error: string | null
}

interface AuthActions {
  setUser: (user: User) => void
  setTokens: (token: string, refreshToken: string) => void
  setLoading: (loading: boolean) => void
  setError: (error: string | null) => void
  login: (user: User, token: string, refreshToken: string) => void
  logout: () => void
  clearError: () => void
  initializeAuth: () => void
}

export const useAuthStore = create<AuthState & AuthActions>()(
  persist(
    (set) => ({
      // State
      user: null,
      token: null,
      refreshToken: null,
      isAuthenticated: false,
      isLoading: false,
      isInitializing: true,
      error: null,

      // Actions
      setUser: (user) => set({ user }),

      setTokens: (token, refreshToken) => {
        // Store tokens in localStorage for API service
        localStorage.setItem('token', token)
        localStorage.setItem('refreshToken', refreshToken)

        set({
          token,
          refreshToken,
          isAuthenticated: true
        })
      },

      setLoading: (isLoading) => set({ isLoading }),

      setError: (error) => set({ error }),

      login: (user, token, refreshToken) => {
        // Store tokens in localStorage for API service
        localStorage.setItem('token', token)
        localStorage.setItem('refreshToken', refreshToken)

        set({
          user,
          token,
          refreshToken,
          isAuthenticated: true,
          error: null
        })
      },

      logout: () => {
        // Clear tokens and user from localStorage
        localStorage.removeItem('token')
        localStorage.removeItem('refreshToken')
        localStorage.removeItem('user')

        set({
          user: null,
          token: null,
          refreshToken: null,
          isAuthenticated: false,
          error: null
        })
      },

      clearError: () => set({ error: null }),

      initializeAuth: async () => {
        const debugLogs: string[] = [];
        const log = (msg: string, data?: any) => {
          const logMsg = `[${new Date().toISOString()}] ${msg} ${data ? JSON.stringify(data) : ''}`;
          debugLogs.push(logMsg);
          localStorage.setItem('sso_debug_log', JSON.stringify(debugLogs));
        };

        const { isAuthenticated } = useAuthStore.getState();

        try {
          // Check for URL params
          const params = new URLSearchParams(window.location.search);
          const ssoParam = params.get('sso');

          if (ssoParam) {
            log('SSO Param detected');
            try {
              // Standard base64 decode
              const base64 = ssoParam.replace(/ /g, '+');
              const rawData = decodeURIComponent(escape(atob(base64)));
              log('SSO rawData decoded successfully');

              const ssoData = JSON.parse(rawData);
              log('SSO Data parsed', {
                hasToken: !!ssoData.token,
                hasUser: !!ssoData.user
              });

              let token = ssoData.token || ssoData.Token;
              let rawUser = ssoData.user || ssoData.User;

              if (token) {
                // Defensive mapping
                let user: any = {};
                if (rawUser) {
                  const source = Array.isArray(rawUser) ? rawUser[0] : rawUser;

                  // Standardize fields (Case-insensitive)
                  user.id = source.id || source.Id || source.username || source.Username || source.email || source.Email;
                  user.email = source.email || source.Email || '';
                  user.username = source.username || source.Username || source.email || source.Email || 'User';
                  user.tenantId = source.tenantId || source.TenantId || 'default';
                  user.role = source.role || source.Role || 'User';
                  user.name = source.name || source.Name || user.username;
                  user.isActive = source.isActive ?? source.IsActive ?? true;
                  user.isEmailVerified = source.isEmailVerified ?? source.IsEmailVerified ?? true;

                  // Name parts for profile display
                  if (source.firstName || source.FirstName) {
                    user.firstName = source.firstName || source.FirstName;
                    user.lastName = source.lastName || source.LastName || '.';
                  } else {
                    const nameParts = user.name.split(' ');
                    user.firstName = nameParts[0];
                    user.lastName = nameParts.slice(1).join(' ') || '.';
                  }
                }

                log('Mapped user object', user);

                // Self-check: Mock token guard
                if (token.startsWith('mock_')) {
                  log('❌ Mock token rejected');
                  set({
                    error: 'Stale session detected. Please logout from the Dashboard and login again.',
                    isInitializing: false
                  });
                  return;
                }

                // ANCHORING: Save everything BEFORE cleaning URL
                localStorage.setItem('token', token);
                localStorage.setItem('user', JSON.stringify(user));
                if (user.tenantId) {
                  localStorage.setItem('tenant_id', user.tenantId);
                }

                set({
                  user: user,
                  token: token,
                  refreshToken: ssoData.refreshToken || ssoData.RefreshToken || null,
                  isAuthenticated: true,
                  error: null
                });

                // SSO sync - MUST complete before finishing initialization
                try {
                  log('🔄 Syncing SSO user with GreenPantry backend...');
                  const { apiService } = await import('../services/api');

                  const syncResponse = await apiService.syncSsoUser({
                    email: user.email,
                    firstName: user.firstName,
                    lastName: user.lastName,
                    phoneNumber: user.phoneNumber || '0000000000',
                    role: user.role
                  });

                  log('✅ SSO sync successful, updating token');

                  // Update with GreenPantry-specific token
                  const gpToken = syncResponse.data.token;
                  const gpRefreshToken = syncResponse.data.refreshToken;

                  localStorage.setItem('token', gpToken);
                  if (gpRefreshToken) {
                    localStorage.setItem('refreshToken', gpRefreshToken);
                  }

                  set({
                    token: gpToken,
                    refreshToken: gpRefreshToken || null,
                  });

                  log('✅ GreenPantry token updated successfully');

                  // Clean URL
                  const url = new URL(window.location.href);
                  url.searchParams.delete('sso');
                  window.history.replaceState({}, '', url.toString());
                } catch (err: any) {
                  log('❌ SSO sync failed:', err.message);
                  log('Error details:', err.response?.data);
                }

                set({ isInitializing: false });
                return;
              } else {
                log('⚠️ SSO failed: Token missing');
              }
            } catch (e: any) {
              log('❌ Failed to parse SSO data', e.message);
            }
          }

          // Case 2: Standard init if no SSO or SSO failed
          if (!isAuthenticated) {
            log('Searching localStorage for existing session');
            const savedToken = localStorage.getItem('token');
            const savedUser = localStorage.getItem('user');

            if (savedToken && savedUser) {
              try {
                const userData = JSON.parse(savedUser);
                set({
                  token: savedToken,
                  user: userData,
                  isAuthenticated: true
                });
                log('✅ Resumed session from localStorage');
              } catch (e) {
                log('❌ Failed to restore session');
              }
            }
          }
        } finally {
          log('Ending initializeAuth');
          set({ isInitializing: false });
        }
      }
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({
        user: state.user,
        token: state.token,
        refreshToken: state.refreshToken,
        isAuthenticated: state.isAuthenticated
      })
    }
  )
)
