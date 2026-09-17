<script setup>
import { reactive } from "vue";

const props = defineProps({
  record: {
    type: Object,
    default: null,
  },
});

const emit = defineEmits(["submit", "cancel"]);

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
</script>

<template>
  <form @submit.prevent="handleSubmit">
    <!-- 基本資訊 -->
    <section>
      <h3 class="mb-4 text-sm font-semibold text-text-primary">基本資訊</h3>

      <div class="grid grid-cols-1 gap-x-5 gap-y-4 md:grid-cols-2">
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            紀錄日期
          </label>

          <input
            v-model="form.recordDate"
            type="date"
            required
            class="w-full rounded-xl border border-border bg-white px-4 py-3 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
          />
        </div>

        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            每週活動量
          </label>

          <select
            v-model="form.activityLevel"
            required
            class="w-full rounded-xl border border-border bg-white px-4 py-3 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
          >
            <option value="" disabled>請選擇活動程度</option>
            <option :value="1.2">久坐無運動</option>
            <option :value="1.375">輕度活動 1~3 天</option>
            <option :value="1.55">中度活動 3~5 天</option>
            <option :value="1.725">高強度運動 6~7 天</option>
          </select>
        </div>
      </div>
    </section>

    <!-- 身體組成 -->
    <section class="mt-4">
      <h3 class="mb-4 text-sm font-semibold text-text-primary">身體組成數據</h3>

      <div class="grid grid-cols-1 gap-x-5 gap-y-4 md:grid-cols-2">
        <!-- 身高 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            身高
          </label>

          <div class="relative">
            <input
              v-model="form.height"
              type="number"
              min="1"
              step="0.1"
              required
              placeholder="例如 165"
              class="w-full rounded-xl border border-border bg-white px-4 py-3 pr-12 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
            />

            <span
              class="absolute top-1/2 right-4 -translate-y-1/2 text-sm text-text-muted"
            >
              cm
            </span>
          </div>
        </div>

        <!-- 體重 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            體重
          </label>

          <div class="relative">
            <input
              v-model="form.weight"
              type="number"
              min="1"
              step="0.1"
              required
              placeholder="例如 60"
              class="w-full rounded-xl border border-border bg-white px-4 py-3 pr-12 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
            />

            <span
              class="absolute top-1/2 right-4 -translate-y-1/2 text-sm text-text-muted"
            >
              kg
            </span>
          </div>
        </div>

        <!-- 體脂率 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            體脂率
            <span class="font-normal text-text-muted">（選填）</span>
          </label>

          <div class="relative">
            <input
              v-model="form.bodyFat"
              type="number"
              min="0"
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

        <!-- 肌肉量 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            肌肉量
            <span class="font-normal text-text-muted">（選填）</span>
          </label>

          <div class="relative">
            <input
              v-model="form.muscleMass"
              type="number"
              min="0"
              step="0.1"
              placeholder="例如 40"
              class="w-full rounded-xl border border-border bg-white px-4 py-3 pr-12 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
            />

            <span
              class="absolute top-1/2 right-4 -translate-y-1/2 text-sm text-text-muted"
            >
              kg
            </span>
          </div>
        </div>

        <!-- 內臟脂肪 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            內臟脂肪
            <span class="font-normal text-text-muted">（選填）</span>
          </label>

          <input
            v-model="form.visceralFat"
            type="number"
            min="0"
            step="0.1"
            placeholder="例如 5"
            class="w-full rounded-xl border border-border bg-white px-4 py-3 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
          />
        </div>
      </div>
    </section>

    <!-- 圍度尺寸 -->
    <section class="mt-4">
      <h3 class="mb-4 text-sm font-semibold text-text-primary">
        圍度尺寸
        <span class="font-normal text-text-muted">（選填）</span>
      </h3>

      <div class="grid grid-cols-1 gap-x-5 gap-y-4 md:grid-cols-3">
        <!-- 頸圍 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            頸圍
          </label>

          <div class="relative">
            <input
              v-model="form.neckSize"
              type="number"
              min="0"
              step="0.1"
              placeholder="例如 32"
              class="w-full rounded-xl border border-border bg-white px-4 py-3 pr-12 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
            />

            <span
              class="absolute top-1/2 right-4 -translate-y-1/2 text-sm text-text-muted"
            >
              cm
            </span>
          </div>
        </div>

        <!-- 腰圍 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            腰圍
          </label>

          <div class="relative">
            <input
              v-model="form.waistSize"
              type="number"
              min="0"
              step="0.1"
              placeholder="例如 70"
              class="w-full rounded-xl border border-border bg-white px-4 py-3 pr-12 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
            />

            <span
              class="absolute top-1/2 right-4 -translate-y-1/2 text-sm text-text-muted"
            >
              cm
            </span>
          </div>
        </div>

        <!-- 臀圍 -->
        <div>
          <label class="mb-2 block text-sm font-medium text-text-secondary">
            臀圍
          </label>

          <div class="relative">
            <input
              v-model="form.hipSize"
              type="number"
              min="0"
              step="0.1"
              placeholder="例如 90"
              class="w-full rounded-xl border border-border bg-white px-4 py-3 pr-12 text-text-primary outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/20"
            />

            <span
              class="absolute top-1/2 right-4 -translate-y-1/2 text-sm text-text-muted"
            >
              cm
            </span>
          </div>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <div class="mt-4 flex justify-end gap-3 border-t border-border pt-5">
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
        {{ record ? "儲存修改" : "儲存紀錄" }}
      </button>
    </div>
  </form>
</template>
