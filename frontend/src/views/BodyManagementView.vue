<script setup>
import { ref, onMounted } from "vue";
import { useBodyRecord } from "@/composables/useBodyRecord";

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

//定義紀錄表單欄位
const form = ref({
  recordDate: "2026-09-12",
  activityLevel: 1.2,
  height: 165,
  weight: 55,
  waistSize: null,
  neckSize: null,
  hipSize: null,
  bodyFat: null,
  muscleMass: null,
  visceralFat: null,
});

const handleCreate = async () => {
  await createRecord({
    userId,
    ...form.value,
  });
};

const handleUpdate = async () => {
  await updateRecord(304, userId, {
    recordDate: "2026-09-10",
    activityLevel: 1.2,
    height: 166,
    weight: 56,
    waistSize: null,
    neckSize: null,
    hipSize: null,
    bodyFat: null,
    muscleMass: null,
    visceralFat: null,
  });
};

const handleDelete = async () => {
  await deleteRecord(304, userId);
};

onMounted(() => {
  getRecords(userId);
});
</script>

<template>
  <div>
    <h1>Body Management</h1>
    <h2>新增身體紀錄</h2>
    <div>
      <label for="">日期: </label>
      <input type="date" v-model="form.recordDate" />
    </div>
    <div>
      <label for="">活動量: </label>
      <input type="number" v-model="form.activityLevel" />
    </div>
    <div>
      <label for="">身高: </label>
      <input type="number" v-model="form.height" />
    </div>
    <div>
      <label for="">體重: </label>
      <input type="number" v-model="form.weight" />
    </div>

    <button @click="handleCreate">新增紀錄</button>
    <hr />
    <button @click="handleUpdate">測試更新</button>
    <hr />
    <button @click="handleDelete">測試刪除</button>
    <hr />

    <p v-if="loading">載入中...</p>
    <p v-else-if="error">取得資料失敗</p>
    <div v-else>
      <p>共有{{ records.length }}筆身體紀錄</p>
      <ul>
        <li v-for="record in records" :key="record.bodyRecordId">
          {{ record.recordDate }} - 身高:{{ record.height }}cm - 體重:{{
            record.weight
          }}kg
        </li>
      </ul>
    </div>
  </div>
</template>
