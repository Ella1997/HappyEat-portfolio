import api from "@/services/api";

export const userGoalApi = {
  getAll(userId) {
    return api.get(`/UserGoal`, {
      params: { userId },
    });
  },

  create(data) {
    return api.post(`/UserGoal`, data);
  },

  update(userId, goalId, data) {
    return api.put(`/UserGoal/${goalId}`, data, {
      params: { userId },
    });
  },

  end(userId, goalId) {
    return api.patch(`/UserGoal/${goalId}/end`, null, {
      params: { userId },
    });
  },
};
