import { ref } from "vue";
import { userApi } from "@/services/userApi";

export function useUser() {
  const user = ref(null);
  const loading = ref(false);
  const error = ref(null);

  const getUser = async (userId) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await userApi.getById(userId);
      user.value = response.data;
    } catch (err) {
      error.value = err;
      console.error("取得使用者資料失敗:", err);
      throw err; // 將後端回傳的錯誤傳至 View
    } finally {
      loading.value = false;
    }
  };
  return {
    user,
    loading,
    error,
    getUser,
  };
}
