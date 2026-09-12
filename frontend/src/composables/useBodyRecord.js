import { ref } from "vue";
import { bodyRecordApi } from "@/services/bodyRecordApi";

export function useBodyRecord() {
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
    } finally {
      loading.value = false;
    }
  };

  const getRecords = async (userId) => {
    await execute(async () => {
      const response = await bodyRecordApi.getAll(userId);
      records.value = response.data;
    }, "取得紀錄失敗: ");
  };

  const createRecord = async (data) => {
    await execute(async () => {
      const response = await bodyRecordApi.create(data);
      await getRecords(data.userId); //新增後重新取得所有紀錄
      return response.data;
    }, "新增紀錄失敗: ");
  };

  const updateRecord = async (id, userId, data) => {
    await execute(async () => {
      await bodyRecordApi.update(id, userId, data);
      await getRecords(userId);
    }, "更新紀錄失敗: ");
  };

  const deleteRecord = async (id, userId) => {
    await execute(async () => {
      await bodyRecordApi.delete(id, userId);
      await getRecords(userId);
    }, "刪除紀錄失敗: ");
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
