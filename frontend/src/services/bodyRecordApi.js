import api from "@/services/api";

export const bodyRecordApi = {
  getAll(userId) {
    return api.get(`/BodyRecord`, {
      params: { userId },
    });
  },

  getById(id, userId) {
    return api.get(`/BodyRecord/${id}`, {
      params: { userId },
    });
  },

  create(data) {
    return api.post(`/BodyRecord`, data);
  },

  update(id, userId, data) {
    return api.put(`/BodyRecord/${id}`, data, {
      params: { userId },
    });
  },

  delete(id, userId) {
    return api.delete(`BodyRecord/${id}`, {
      params: { userId },
    });
  },
};
