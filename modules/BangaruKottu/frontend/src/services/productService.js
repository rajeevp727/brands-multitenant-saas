import api from './api';

export const productService = {
  getAll: () => api.get('/product'),
  getById: (id) => api.get(`/product/${id}`),
  create: (data) => api.post('/product', data),
  update: (id, data) => api.put(`/product/${id}`, data),
  delete: (id) => api.delete(`/product/${id}`),
  toggleStatus: (id) => api.patch(`/product/${id}/toggle-status`),
};

