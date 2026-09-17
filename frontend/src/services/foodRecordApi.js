import api from "@/services/api";

export const foodRecordApi = {
  getAll(userId) {
    return api.get("/FoodRecord", {
      params: { userId },
    });
  },

  getById(recordId, userId) {
    return api.get(`/FoodRecord/${recordId}`, {
      params: { userId },
    });
  },

  create(data) {
    return api.post("/FoodRecord", data);
  },

  update(recordId, userId, data) {
    return api.put(`/FoodRecord/${recordId}`, data, {
      params: { userId },
    });
  },

  delete(recordId, userId) {
    return api.delete(`/FoodRecord/${recordId}`, {
      params: { userId },
    });
  },
};
