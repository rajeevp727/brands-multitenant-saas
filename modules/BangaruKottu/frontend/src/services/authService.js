import api from './api';

export const authService = {
  login: async (email, password) => {
    const response = await api.post('/auth/login', { email, password });
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(response.data));
    }
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  getCurrentUser: () => {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  isAuthenticated: () => {
    return !!localStorage.getItem('token');
  },

  syncSsoUser: async (ssoData) => {
    const response = await api.post('/sso/sync', {
      email: ssoData.email,
      firstName: ssoData.name?.split(' ')[0] || 'User',
      lastName: ssoData.name?.split(' ')[1] || 'SSO',
      phoneNumber: ssoData.phoneNumber || '0000000000',
      role: ssoData.role || 'User'
    });

    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.user || response.data));
    }
    return response.data;
  }
};

