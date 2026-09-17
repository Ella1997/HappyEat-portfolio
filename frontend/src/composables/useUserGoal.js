import { computed, ref } from "vue";
import { userGoalApi } from "@/services/userGoalApi";

export function useUserGoal() {
  const goals = ref([]);
  const loading = ref(false);
  const error = ref(null);

  const activeGoal = computed(() => {
    return goals.value.find((goal) => goal.isActive) ?? null;
  });

  const execute = async (action, errorMessage) => {
    loading.value = true;
    error.value = null;
    try {
      return await action();
    } catch (err) {
      error.value = err;
      console.error(errorMessage, err);
      throw err; //將後端回傳的錯誤傳至View
    } finally {
      loading.value = false;
    }
  };

  const getGoals = async (userId) => {
    await execute(async () => {
      const response = await userGoalApi.getAll(userId);
      goals.value = response.data;
    }, "取得使用者目標失敗：");
  };

  const createGoal = async (data) => {
    return await execute(async () => {
      const response = await userGoalApi.create(data);
      await getGoals(data.userId);
      return response.data;
    }, "建立體態目標失敗: ");
  };

  const updateGoal = async (userId, goalId, data) => {
    await execute(async () => {
      await userGoalApi.update(userId, goalId, data);
      await getGoals(userId);
    }, "更新體態目標失敗: ");
  };

  const endGoal = async (userId, goalId) => {
    await execute(async () => {
      await userGoalApi.end(userId, goalId);
      await getGoals(userId);
    }, "停用目標失敗");
  };

  return {
    goals,
    activeGoal,
    loading,
    error,
    getGoals,
    createGoal,
    updateGoal,
    endGoal,
  };
}
