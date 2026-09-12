<script setup>
import { ref, onMounted } from "vue";
import { useBodyRecord } from "@/composables/useBodyRecord";
import BodyRecordForm from "@/components/body/BodyRecordForm.vue";
import BodyRecordList from "@/components/body/BodyRecordList.vue";

const {
  records,
  loading,
  error,
  getRecords,
  createRecord,
  updateRecord,
  deleteRecord,
} = useBodyRecord();

//先寫死userId = 2測試
const userId = 2;

const editingRecord = ref(null);

const handleEdit = (record) => {
  //   console.log("編輯紀錄", record);
  editingRecord.value = record;
};

const handleDelete = async (record) => {
  //   console.log("刪除紀錄", record);
  const confirmed = window.confirm(
    `確定要刪除${record.recordDate}的體態紀錄嗎?`,
  );
  if (!confirmed) return;
  await deleteRecord(record.bodyRecordId, userId);
};

const handleSubmit = async (data) => {
  if (editingRecord.value) {
    await updateRecord(editingRecord.value.bodyRecordId, userId, data);
    editingRecord.value = null;
    return;
  }
  await createRecord({
    userId,
    ...data,
  });
};

const handleCancel = () => {
  editingRecord.value = null;
};

onMounted(() => {
  getRecords(userId);
});
</script>

<template>
  <div>
    <h1>Body Management</h1>
    <p v-if="loading">載入中...</p>
    <p v-else-if="error">取得資料失敗</p>
    <div v-else>
      <p>共有{{ records.length }}筆身體紀錄</p>

      <BodyRecordList
        :records="records"
        @edit="handleEdit"
        @delete="handleDelete"
      />

      <BodyRecordForm
        :key="editingRecord?.bodyRecordId ?? 'new'"
        :record="editingRecord"
        @submit="handleSubmit"
        @cancel="handleCancel"
      />
    </div>
  </div>
</template>
