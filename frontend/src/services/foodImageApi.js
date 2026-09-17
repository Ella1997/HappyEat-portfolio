import api from "@/services/api";

export const foodImageApi = {
  getById(imageId) {
    return api.get(`/FoodImage/${imageId}`);
  },

  getByUserId(userId) {
    return api.get(`/FoodImage/user/${userId}`);
  },

  create(data) {
    return api.post("/FoodImage", data, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },

  delete(imageId) {
    return api.delete(`/FoodImage/${imageId}`);
  },
};
