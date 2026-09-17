import api from "@/services/api";

export const userApi = {
  getById(userId) {
    return api.get(`/User/${userId}`);
  },
};
