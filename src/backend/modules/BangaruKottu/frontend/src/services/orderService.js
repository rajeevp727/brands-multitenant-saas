import api from './api';

export const orderService = {
  getAll: () => api.get('/order'),
  getById: (id) => api.get(`/order/${id}`),
  accept: (id) => api.post(`/order/${id}/accept`),
  reject: (id) => api.post(`/order/${id}/reject`),
  prepare: (id) => api.post(`/order/${id}/prepare`),
  dispatch: (id) => api.post(`/order/${id}/dispatch`),
  deliver: (id) => api.post(`/order/${id}/deliver`),
};

