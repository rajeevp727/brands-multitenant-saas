import api from './api';

export const vendorService = {
  getProfile: () => api.get('/vendor/profile'),
  updateProfile: (data) => api.put('/vendor/profile', data),
};

