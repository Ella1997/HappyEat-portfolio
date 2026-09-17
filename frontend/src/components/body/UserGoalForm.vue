<script setup>
import { computed, reactive } from "vue";

const props = defineProps({
  goal: {
    type: Object,
    default: null,
  },
  currentWeight: {
    type: Number,
    default: null,
  },
});

const emit = defineEmits(["submit", "cancel"]);

// 今天日期，格式 YYYY-MM-DD
const today = new Intl.DateTimeFormat("en-CA", {
  timeZone: "Asia/Taipei",
}).format(new Date());

const form = reactive({
  goalType: props.goal?.goalType ?? "",
  startWeight: props.goal?.startWeight ?? props.currentWeight ?? "",
  targetWeight: props.goal?.targetWeight ?? "",
  targetBodyFat: props.goal?.targetBodyFat ?? "",
  startDate: props.goal?.startDate ?? today,
  targetDate: props.goal?.targetDate ?? "",
});

// 目標日期至少要比開始日期晚一天
const minTargetDate = computed(() => {
  if (!form.startDate) return "";

  const date = new Date(`${form.startDate}T00:00:00`);
  date.setDate(date.getDate() + 1);

  return new Intl.DateTimeFormat("en-CA", {
    timeZone: "Asia/Taipei",
  }).format(date);
});

const handleSubmit = () => {
  emit("submit", {
    goalType: form.goalType,

    startWeight: form.startWeight === "" ? null : Number(form.startWeight),

    targetWeight: form.targetWeight === "" ? null : Number(form.targetWeight),

    targetBodyFat:
      form.targetBodyFat === "" ? null : Number(form.targetBodyFat),

    startDate: form.startDate,

    targetDate: form.targetDate === "" ? null : form.targetDate,
  });
};
</script>

<template>
  <form @submit.prevent="handleSubmit">
    <!-- =========================
         目標設定
    ========================== -->
    <section>
      <h3 class="mb-4 text-sm font-semibold text-text-primary">目標設定</h3>

      <div class="grid grid-cols-1 gap-x-5 gap-y-4 md:grid-cols-2">
        <!-- 目標類型 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            目標類型
          </label>

          <select
            v-model="form.goalType"
            required
            class="w-full rounded-xl border border-border bg-white px-4 py-3 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
          >
            <option value="" disabled>請選擇目標</option>
            <option value="減脂">減脂</option>
            <option value="增肌">增肌</option>
            <option value="維持">維持</option>
          </select>
        </div>

        <!-- 起始體重 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            {{ goal ? "起始體重" : "目前體重" }}
          </label>

          <div class="relative">
            <input
              v-model="form.startWeight"
              type="number"
              min="1"
              step="0.1"
              required
              :disabled="!!goal"
              placeholder="例如 65"
              class="w-full rounded-xl border border-border bg-white px-4 py-3 pr-12 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20 disabled:cursor-not-allowed disabled:bg-surface-soft disabled:text-text-muted"
            />

            <span
              class="absolute top-1/2 right-4 -translate-y-1/2 text-sm text-text-muted"
            >
              kg
            </span>
          </div>

          <p v-if="goal" class="mt-1.5 text-xs text-text-muted">
            起始體重建立後無法修改
          </p>
        </div>
      </div>
    </section>

    <!-- =========================
         目標數值
    ========================== -->
    <section class="mt-3">
      <h3 class="mb-4 text-sm font-semibold text-text-primary">目標數值</h3>

      <div class="grid grid-cols-1 gap-x-5 gap-y-4 md:grid-cols-2">
        <!-- 目標體重 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            目標體重
          </label>

          <div class="relative">
            <input
              v-model="form.targetWeight"
              type="number"
              min="1"
              step="0.1"
              placeholder="例如 55"
              class="w-full rounded-xl border border-border bg-white px-4 py-3 pr-12 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
            />

            <span
              class="absolute top-1/2 right-4 -translate-y-1/2 text-sm text-text-muted"
            >
              kg
            </span>
          </div>
        </div>

        <!-- 目標體脂率 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            目標體脂率
          </label>

          <div class="relative">
            <input
              v-model="form.targetBodyFat"
              type="number"
              min="1"
              max="100"
              step="0.1"
              placeholder="例如 20"
              class="w-full rounded-xl border border-border bg-white px-4 py-3 pr-12 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
            />

            <span
              class="absolute top-1/2 right-4 -translate-y-1/2 text-sm text-text-muted"
            >
              %
            </span>
          </div>
        </div>
      </div>
    </section>

    <!-- =========================
         目標期間
    ========================== -->
    <section class="mt-3">
      <h3 class="mb-4 text-sm font-semibold text-text-primary">目標期間</h3>

      <div class="grid grid-cols-1 gap-x-5 gap-y-4 md:grid-cols-2">
        <!-- 開始日期 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            開始日期
          </label>

          <input
            v-model="form.startDate"
            type="date"
            :min="today"
            required
            :disabled="!!goal"
            class="w-full rounded-xl border border-border bg-white px-4 py-3 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20 disabled:cursor-not-allowed disabled:bg-surface-soft disabled:text-text-muted"
          />

          <p v-if="goal" class="mt-1.5 text-xs text-text-muted">
            開始日期建立後無法修改
          </p>
        </div>

        <!-- 目標日期 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            目標日期
            <span class="font-normal text-text-muted">（選填）</span>
          </label>

          <input
            v-model="form.targetDate"
            type="date"
            :min="minTargetDate"
            class="w-full rounded-xl border border-border bg-white px-4 py-3 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
          />
        </div>
      </div>
    </section>

    <!-- =========================
         Footer
    ========================== -->
    <div class="mt-3 flex justify-end gap-3 border-t border-border pt-5">
      <button
        type="button"
        class="rounded-xl border border-border bg-white px-5 py-2.5 text-sm font-medium text-text-secondary transition hover:bg-surface-soft"
        @click="emit('cancel')"
      >
        取消
      </button>

      <button
        type="submit"
        class="rounded-xl bg-accent px-6 py-2.5 text-sm font-semibold text-white transition hover:bg-accent-hover"
      >
        {{ goal ? "儲存修改" : "建立目標" }}
      </button>
    </div>
  </form>
</template>
