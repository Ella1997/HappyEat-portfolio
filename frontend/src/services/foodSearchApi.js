import api from "@/services/api";

export const foodSearchApi = {
  search(keyword) {
    return api.get("/FoodSearch", {
      params: { keyword },
    });
  },
};
