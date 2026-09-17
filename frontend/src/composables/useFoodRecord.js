import { ref } from "vue";
import { foodRecordApi } from "@/services/foodRecordApi";

export function useFoodRecord() {
  const records = ref([]);
  const loading = ref(false);
  const error = ref(null);

  const execute = async (action, errorMessage) => {
    loading.value = true;
    error.value = null;

    try {
      return await action();
    } catch (err) {
      error.value = err;
      console.error(errorMessage, err);

      throw err; // 將後端回傳的錯誤傳至 View
    } finally {
      loading.value = false;
    }
  };

  const getRecords = async (userId) => {
    await execute(async () => {
      const response = await foodRecordApi.getAll(userId);
      records.value = response.data;
    }, "取得飲食紀錄失敗: ");
  };

  const createRecord = async (data) => {
    await execute(async () => {
      const response = await foodRecordApi.create(data);

      // 新增後重新取得所有飲食紀錄
      await getRecords(data.userId);

      return response.data;
    }, "新增飲食紀錄失敗: ");
  };

  const updateRecord = async (id, userId, data) => {
    await execute(async () => {
      await foodRecordApi.update(id, userId, data);

      // 更新後重新取得所有飲食紀錄
      await getRecords(userId);
    }, "更新飲食紀錄失敗: ");
  };

  const deleteRecord = async (id, userId) => {
    await execute(async () => {
      await foodRecordApi.delete(id, userId);

      // 刪除後重新取得所有飲食紀錄
      await getRecords(userId);
    }, "刪除飲食紀錄失敗: ");
  };

  return {
    records,
    loading,
    error,
    getRecords,
    createRecord,
    updateRecord,
    deleteRecord,
  };
}
