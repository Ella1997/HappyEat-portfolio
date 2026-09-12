<script setup>
const props = defineProps({
  records: {
    type: Array,
    default: () => [],
  },
});

const emit = defineEmits(["edit", "delete"]);

const handleEdit = (record) => {
  emit("edit", record);
};

const handleDelete = (record) => {
  emit("delete", record);
};
</script>

<template>
  <h2>體態紀錄</h2>
  <p v-if="props.records.length === 0">目前沒有體態紀錄</p>
  <div v-else>
    <div v-for="record in props.records" :key="record.bodyRecordId">
      <p>日期：{{ record.recordDate }}</p>

      <p>身高：{{ record.height }} cm</p>

      <p>體重：{{ record.weight }} kg</p>

      <p>
        體脂率：
        {{ record.bodyFat ?? "-" }} %
      </p>
      <button type="button" @click="handleEdit(record)">編輯</button>
      <button type="button" @click="handleDelete(record)">刪除</button>
      <hr />
    </div>
  </div>
</template>
