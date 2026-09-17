<script setup>
import { computed } from "vue";

// =========================
// Props / Emits
// =========================
const props = defineProps({
  record: {
    type: Object,
    required: true,
  },
});

const emit = defineEmits(["close", "edit", "delete"]);

// =========================
// Food Items
// =========================
const items = computed(() => {
  return props.record.items ?? [];
});

// =========================
// Format
// =========================
const formatNumber = (value) => {
  if (value == null) {
    return "-";
  }

  const number = Number(value);

  return Number.isInteger(number)
    ? number.toLocaleString()
    : number.toLocaleString(undefined, {
        maximumFractionDigits: 1,
      });
};

const formatDate = (dateString) => {
  if (!dateString) return "-";

  const [datePart] = String(dateString).split("T");
  const [year, month, day] = datePart.split("-");

  return `${year}/${month}/${day}`;
};

// =========================
// Meal Type Style
// =========================
const getMealTypeClass = (mealType) => {
  switch (mealType) {
    case "早餐":
      return "bg-primary-soft text-primary-dark";

    case "早午餐":
      return "bg-info-soft text-primary-dark";

    case "午餐":
      return "bg-info-strong text-text-primary";

    case "點心":
      return "bg-accent/20 text-text-primary";

    case "晚餐":
      return "bg-accent/35 text-text-primary";

    case "宵夜":
      return "bg-border text-text-secondary";

    default:
      return "bg-surface-soft text-text-secondary";
  }
};

// =========================
// Actions
// =========================
const handleEdit = () => {
  emit("edit", props.record);
};

const handleDelete = () => {
  emit("delete", props.record);
};

const handleBackdropClick = () => {
  emit("close");
};
</script>

<template>
  <!-- =========================
       Backdrop
  ========================== -->
  <div
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/35 p-4"
    @click="handleBackdropClick"
  >
    <!-- =========================
         Modal
    ========================== -->
    <div
      class="flex max-h-[90vh] w-full max-w-3xl flex-col overflow-hidden rounded-3xl border border-border bg-modal-content shadow-xl"
      @click.stop
    >
      <!-- =========================
           Header
      ========================== -->
      <header
        class="flex shrink-0 items-center justify-between border-b border-border bg-modal-header px-6 py-5"
      >
        <h2 class="text-xl font-semibold text-text-primary">飲食紀錄明細</h2>

        <!-- Close -->
        <button
          type="button"
          class="flex h-9 w-9 items-center justify-center rounded-xl text-text-muted transition hover:bg-white/60 hover:text-text-primary"
          aria-label="關閉飲食紀錄明細"
          @click="emit('close')"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="h-5 w-5"
          >
            <path d="M18 6 6 18" />
            <path d="m6 6 12 12" />
          </svg>
        </button>
      </header>

      <!-- =========================
           Content
      ========================== -->
      <div class="scrollbar-none flex-1 overflow-y-auto p-6">
        <!-- =========================
             Meal Overview
        ========================== -->
        <section class="grid gap-6 lg:grid-cols-[220px_1fr]">
          <!-- Image -->
          <div
            class="flex aspect-4/3 w-full items-center justify-center overflow-hidden rounded-2xl border border-border bg-info-soft"
          >
            <img
              v-if="record.imagePath"
              :src="record.imagePath"
              :alt="record.description || '餐點照片'"
              class="h-full w-full object-cover"
            />

            <!-- No Image -->
            <div
              v-else
              class="flex flex-col items-center gap-2 text-text-muted"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="1.6"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="h-9 w-9"
              >
                <rect width="18" height="18" x="3" y="3" rx="2" />

                <circle cx="9" cy="9" r="2" />

                <path d="m21 15-3.086-3.086a2 2 0 0 0-2.828 0L6 21" />
              </svg>

              <span class="text-sm"> 無餐點照片 </span>
            </div>
          </div>

          <!-- Meal Information -->
          <div class="flex flex-col justify-center">
            <!-- Meal Type + Date -->
            <div class="flex flex-wrap items-center gap-3">
              <span
                class="inline-flex rounded-lg px-3 py-1.5 text-sm font-medium"
                :class="getMealTypeClass(record.mealType)"
              >
                {{ record.mealType || "未分類" }}
              </span>

              <span class="text-sm text-text-muted">
                {{ formatDate(record.recordDate) }}
              </span>
            </div>

            <!-- Description -->
            <div
              v-if="record.description"
              class="mt-5 rounded-2xl border border-border bg-info-medium px-4 py-4"
            >
              <div class="flex items-center gap-2">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  class="h-4 w-4 text-primary-dark"
                >
                  <path
                    d="M12 3a6 6 0 0 0-3.6 10.8c.38.28.6.72.6 1.2v1h6v-1c0-.48.22-.92.6-1.2A6 6 0 0 0 12 3Z"
                  />
                  <path d="M9 20h6" />
                  <path d="M10 16v1" />
                  <path d="M14 16v1" />
                </svg>

                <span class="text-sm font-semibold text-text-primary">
                  AI 飲食建議/ 心得描述
                </span>
              </div>

              <p class="mt-2 text-sm leading-6 text-text-secondary">
                {{ record.description }}
              </p>
            </div>

            <!-- Source -->
            <div
              v-if="record.recordSource"
              class="mt-3 flex items-center gap-2 text-sm text-text-secondary"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="h-4 w-4 text-text-muted"
              >
                <path d="M12 2a10 10 0 1 0 10 10" />
                <path d="M12 6v6l4 2" />
              </svg>

              <span>
                {{ record.recordSource === "AI" ? "AI 辨識建立" : "手動新增" }}
              </span>
            </div>
          </div>
        </section>

        <!-- =========================
             Food Items
        ========================== -->
        <section class="mt-8">
          <!-- Section Header -->
          <div class="mb-4 flex items-center justify-between">
            <h3 class="text-base font-semibold text-text-primary">食物明細</h3>

            <span class="text-sm text-text-muted">
              共 {{ items.length }} 項
            </span>
          </div>

          <!-- Empty -->
          <div
            v-if="items.length === 0"
            class="rounded-2xl border border-dashed border-border bg-surface-soft px-5 py-8 text-center"
          >
            <p class="text-sm text-text-secondary">目前沒有食物明細</p>
          </div>

          <!-- Items Table -->
          <div
            v-else
            class="overflow-x-auto rounded-2xl border border-border bg-white scrollbar-none"
          >
            <table class="w-full min-w-180 border-collapse">
              <thead>
                <tr class="border-b border-border bg-surface-soft">
                  <th
                    class="px-4 py-3 text-left text-sm font-medium text-text-secondary"
                  >
                    食物
                  </th>

                  <th
                    class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                  >
                    份量
                  </th>

                  <th
                    class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                  >
                    碳水
                    <span class="font-normal text-text-muted"> (g) </span>
                  </th>

                  <th
                    class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                  >
                    蛋白質
                    <span class="font-normal text-text-muted"> (g) </span>
                  </th>

                  <th
                    class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                  >
                    脂肪
                    <span class="font-normal text-text-muted"> (g) </span>
                  </th>

                  <th
                    class="px-4 py-3 text-center text-sm font-medium text-text-secondary"
                  >
                    熱量
                    <span class="font-normal text-text-muted"> (kcal) </span>
                  </th>
                </tr>
              </thead>

              <tbody>
                <tr
                  v-for="(item, index) in items"
                  :key="item.itemId ?? `${item.itemName}-${index}`"
                  class="border-b border-border last:border-b-0"
                >
                  <!-- Food -->
                  <td class="px-4 py-4 text-base font-medium text-text-primary">
                    {{ item.itemName || "未命名食物" }}
                  </td>

                  <!-- Quantity -->
                  <td class="px-4 py-4 text-center text-base text-text-primary">
                    <template v-if="item.quantity != null">
                      {{ formatNumber(item.quantity) }}

                      <span
                        v-if="item.unit"
                        class="ml-1 text-sm text-text-secondary"
                      >
                        {{ item.unit }}
                      </span>
                    </template>

                    <span v-else class="text-text-muted"> - </span>
                  </td>

                  <!-- Carbs -->
                  <td class="px-4 py-4 text-center text-base text-text-primary">
                    {{ formatNumber(item.carbs) }}
                  </td>

                  <!-- Protein -->
                  <td class="px-4 py-4 text-center text-base text-text-primary">
                    {{ formatNumber(item.protein) }}
                  </td>

                  <!-- Fat -->
                  <td class="px-4 py-4 text-center text-base text-text-primary">
                    {{ formatNumber(item.fat) }}
                  </td>

                  <!-- Calories -->
                  <td
                    class="px-4 py-4 text-center text-base font-semibold text-text-primary"
                  >
                    {{ formatNumber(item.calories) }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>

        <!-- =========================
             Nutrition Summary
        ========================== -->
        <section class="mt-8">
          <h3 class="mb-4 text-base font-semibold text-text-primary">
            營養總計
          </h3>

          <div
            class="grid overflow-hidden rounded-2xl border border-border bg-white sm:grid-cols-2 lg:grid-cols-4"
          >
            <!-- Carbs -->
            <div class="px-5 py-5">
              <p class="text-sm text-text-secondary">碳水</p>

              <p class="mt-2 text-xl font-semibold text-text-primary">
                {{ formatNumber(record.totalCarbs) }}

                <span class="text-sm font-normal text-text-secondary"> g </span>
              </p>
            </div>

            <!-- Protein -->
            <div class="border-border px-5 py-5 sm:border-l">
              <p class="text-sm text-text-secondary">蛋白質</p>

              <p class="mt-2 text-xl font-semibold text-text-primary">
                {{ formatNumber(record.totalProtein) }}

                <span class="text-sm font-normal text-text-secondary"> g </span>
              </p>
            </div>

            <!-- Fat -->
            <div
              class="border-border px-5 py-5 sm:border-t lg:border-t-0 lg:border-l"
            >
              <p class="text-sm text-text-secondary">脂肪</p>

              <p class="mt-2 text-xl font-semibold text-text-primary">
                {{ formatNumber(record.totalFat) }}

                <span class="text-sm font-normal text-text-secondary"> g </span>
              </p>
            </div>

            <!-- Calories -->
            <div
              class="border-border bg-primary-soft px-5 py-5 sm:border-l sm:border-t lg:border-t-0"
            >
              <p class="text-sm text-text-secondary">總熱量</p>

              <p class="mt-2 text-xl font-semibold text-text-primary">
                {{ formatNumber(record.totalCalories) }}

                <span class="text-sm font-normal text-text-secondary">
                  kcal
                </span>
              </p>
            </div>
          </div>
        </section>
      </div>

      <!-- =========================
           Footer
      ========================== -->
      <footer
        class="flex shrink-0 items-center justify-between border-t border-border bg-white px-6 py-4"
      >
        <!-- Delete -->
        <button
          type="button"
          class="inline-flex items-center gap-2 rounded-xl px-4 py-2.5 text-sm font-medium text-danger transition hover:bg-danger/10"
          @click="handleDelete"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="h-4 w-4"
          >
            <path d="M3 6h18" />
            <path d="M8 6V4h8v2" />
            <path d="M19 6l-1 14H6L5 6" />
            <path d="M10 11v5" />
            <path d="M14 11v5" />
          </svg>

          刪除
        </button>

        <!-- Right Actions -->
        <div class="flex items-center gap-3">
          <button
            type="button"
            class="rounded-xl border border-border bg-white px-5 py-2.5 text-sm font-medium text-text-secondary transition hover:bg-surface-soft hover:text-text-primary"
            @click="emit('close')"
          >
            關閉
          </button>

          <button
            type="button"
            class="inline-flex items-center gap-2 rounded-xl bg-accent px-5 py-2.5 text-sm font-medium text-text-primary transition hover:bg-accent-hover"
            @click="handleEdit"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="h-4 w-4"
            >
              <path d="M12 20h9" />
              <path d="M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4Z" />
            </svg>

            編輯
          </button>
        </div>
      </footer>
    </div>
  </div>
</template>
