<script setup>
import { reactive } from "vue";

const props = defineProps({
  record: {
    type: Object,
    default: null,
  },
}); //父傳子(接收父元件給的資料)

const emit = defineEmits(["submit", "cancel"]); //子傳父(把事件/結果通知父元件)

const form = reactive({
  recordDate: props.record?.recordDate ?? "",
  activityLevel: props.record?.activityLevel ?? "",
  height: props.record?.height ?? "",
  weight: props.record?.weight ?? "",
  waistSize: props.record?.waistSize ?? "",
  neckSize: props.record?.neckSize ?? "",
  hipSize: props.record?.hipSize ?? "",
  bodyFat: props.record?.bodyFat ?? "",
  muscleMass: props.record?.muscleMass ?? "",
  visceralFat: props.record?.visceralFat ?? "",
});

const handleSubmit = () => {
  emit("submit", {
    ...form,
    height: Number(form.height),
    weight: Number(form.weight),
    activityLevel: Number(form.activityLevel),
    waistSize: form.waistSize === "" ? null : Number(form.waistSize),
    neckSize: form.neckSize === "" ? null : Number(form.neckSize),
    hipSize: form.hipSize === "" ? null : Number(form.hipSize),
    bodyFat: form.bodyFat === "" ? null : Number(form.bodyFat),
    muscleMass: form.muscleMass === "" ? null : Number(form.muscleMass),
    visceralFat: form.visceralFat === "" ? null : Number(form.visceralFat),
  });
};

const handleCancel = () => {
  emit("cancel");
};
</script>

<template>
  <form @submit.prevent="handleSubmit">
    <h2>
      {{ record ? "編輯紀錄" : "新增紀錄" }}
    </h2>
    <div>
      <label>紀錄日期</label>
      <input v-model="form.recordDate" type="date" required />
    </div>

    <div>
      <label>活動程度</label>
      <select v-model="form.activityLevel">
        <option :value="1.2">久坐無運動</option>
        <option :value="1.375">輕度活動 1~3 天</option>
        <option :value="1.55">中度活動 3~5 天</option>
        <option :value="1.725">高強度運動 6~7 天</option>
      </select>
    </div>

    <div>
      <label>身高(cm)</label>
      <input type="number" v-model="form.height" step="0.1" required />
    </div>

    <div>
      <label>體重(kg)</label>
      <input type="number" v-model="form.weight" step="0.1" required />
    </div>

    <div>
      <label>腰圍(cm)</label>
      <input type="number" v-model="form.waistSize" step="0.1" />
    </div>

    <div>
      <label>頸圍(cm)</label>
      <input type="number" v-model="form.neckSize" step="0.1" />
    </div>

    <div>
      <label>臀圍(cm)</label>
      <input type="number" v-model="form.hipSize" step="0.1" />
    </div>

    <div>
      <label>體脂率(%)</label>
      <input type="number" v-model="form.bodyFat" step="0.1" />
    </div>

    <div>
      <label>肌肉量(kg)</label>
      <input type="number" v-model="form.muscleMass" step="0.1" />
    </div>

    <div>
      <label>內臟脂肪</label>
      <input type="number" v-model="form.visceralFat" step="0.1" />
    </div>

    <button type="submit">
      {{ record ? "儲存" : "新增" }}
    </button>

    <button v-if="record" type="button" @click="handleCancel">取消</button>
  </form>
</template>
