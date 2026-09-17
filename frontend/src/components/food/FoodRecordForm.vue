<script setup>
import { useFoodRecordForm } from "@/composables/useFoodRecordForm";

const props = defineProps({
  record: {
    type: Object,
    default: null,
  },
});

const emit = defineEmits(["submit", "cancel"]);

const {
  form,
  mealTypes,
  totals,
  addItem,
  removeItem,
  handleInputSearch,
  selectSearchResult,
  hideDropdown,
  getItemValue,
  getItemTypeLabel,
  getItemTypeClass,
  buildPayload,
} = useFoodRecordForm(props.record);

const handleSubmit = () => {
  emit("submit", buildPayload());
};
</script>

<template>
  <form class="space-y-6" @submit.prevent="handleSubmit">
    <!-- =========================
         基本資料
    ========================== -->
    <section class="grid gap-4 md:grid-cols-2">
      <!-- 日期 -->
      <div>
        <label
          for="recordDate"
          class="mb-2 block text-sm font-medium text-text-secondary"
        >
          日期
        </label>

        <input
          id="recordDate"
          v-model="form.recordDate"
          type="date"
          required
          class="w-full rounded-2xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none transition focus:border-primary"
        />
      </div>

      <!-- 餐別 -->
      <div>
        <label
          for="mealType"
          class="mb-2 block text-sm font-medium text-text-secondary"
        >
          餐別
        </label>

        <select
          id="mealType"
          v-model="form.mealType"
          required
          class="w-full rounded-2xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none transition focus:border-primary"
        >
          <option value="" disabled>請選擇餐別</option>

          <option
            v-for="mealType in mealTypes"
            :key="mealType"
            :value="mealType"
          >
            {{ mealType }}
          </option>
        </select>
      </div>

      <!-- 餐點描述 -->
      <div class="md:col-span-2">
        <label
          for="description"
          class="mb-2 block text-sm font-medium text-text-secondary"
        >
          餐點描述
        </label>

        <input
          id="description"
          v-model="form.description"
          type="text"
          placeholder="例如：午餐健康餐"
          class="w-full rounded-2xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none transition placeholder:text-text-muted focus:border-primary"
        />
      </div>
    </section>

    <!-- =========================
         餐點項目
    ========================== -->
    <section>
      <!-- Section Header -->
      <div class="mb-4 flex items-end justify-between gap-4">
        <div>
          <h3 class="font-semibold text-text-primary">餐點項目</h3>

          <p class="mt-1 text-sm text-text-muted">
            搜尋食物或飲料，可自動帶入營養資訊
          </p>
        </div>

        <!-- 新增項目 -->
        <button
          type="button"
          class="inline-flex shrink-0 items-center gap-2 rounded-2xl border border-primary bg-primary-soft px-4 py-2.5 text-sm font-medium text-primary-dark transition hover:bg-primary/20"
          @click="addItem"
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
            <path d="M12 5v14" />
            <path d="M5 12h14" />
          </svg>

          新增項目
        </button>
      </div>

      <!-- =========================
           Item Cards
      ========================== -->
      <div class="space-y-4">
        <div
          v-for="(item, index) in form.items"
          :key="index"
          class="rounded-3xl border border-border bg-surface-soft p-5 md:p-6"
        >
          <!-- Item Header -->
          <div class="mb-5 flex items-center justify-between">
            <div class="flex items-center gap-2">
              <h4 class="font-semibold text-text-primary">
                項目 {{ index + 1 }}
              </h4>

              <!-- Food / Drink 類型 -->
              <span
                v-if="item.foodId || item.drinkId"
                class="rounded-full px-2.5 py-1 text-xs font-medium"
                :class="getItemTypeClass(item)"
              >
                {{ getItemTypeLabel(item) }}
              </span>
            </div>

            <!-- 刪除項目 -->
            <button
              type="button"
              class="flex h-8 w-8 items-center justify-center rounded-xl text-text-muted transition hover:bg-surface hover:text-danger disabled:cursor-not-allowed disabled:opacity-30"
              :disabled="form.items.length <= 1"
              aria-label="刪除項目"
              @click="removeItem(index)"
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
            </button>
          </div>

          <!-- =========================
               食物 / 飲料搜尋
          ========================== -->
          <div class="relative">
            <label class="mb-2 block text-sm font-medium text-text-secondary">
              食物 / 飲料名稱
            </label>

            <div class="relative">
              <!-- Search Icon -->
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="pointer-events-none absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-text-muted"
              >
                <circle cx="11" cy="11" r="8" />
                <path d="m21 21-4.3-4.3" />
              </svg>

              <input
                v-model="item.itemName"
                type="text"
                placeholder="搜尋食物或飲料"
                class="w-full rounded-2xl border border-border bg-surface py-3 pl-11 pr-4 text-sm text-text-primary outline-none transition placeholder:text-text-muted focus:border-primary"
                @input="handleInputSearch(item)"
                @focus="item.showDropdown = item.searchResults.length > 0"
                @blur="hideDropdown(item)"
              />
            </div>

            <!-- Search Dropdown -->
            <div
              v-if="item.showDropdown"
              class="absolute left-0 right-0 top-full z-30 mt-2 max-h-60 overflow-y-auto rounded-2xl border border-border bg-surface p-2 shadow-lg"
            >
              <button
                v-for="result in item.searchResults"
                :key="`${result.type}-${result.id}`"
                type="button"
                class="flex w-full items-center justify-between gap-4 rounded-xl px-3 py-3 text-left transition hover:bg-surface-soft"
                @mousedown.prevent="selectSearchResult(item, result)"
              >
                <div class="min-w-0">
                  <p class="truncate text-sm font-medium text-text-primary">
                    {{ result.name }}
                  </p>

                  <p class="mt-1 text-xs text-text-muted">
                    {{ result.unit || "份" }}
                    ·
                    {{ Number(result.calories ?? 0).toFixed(0) }} kcal
                  </p>
                </div>

                <span
                  class="shrink-0 rounded-full px-2.5 py-1 text-xs font-medium"
                  :class="
                    result.type === 'Drink'
                      ? 'bg-info-strong text-text-primary'
                      : 'bg-primary-soft text-primary-dark'
                  "
                >
                  {{ result.type === "Drink" ? "飲料" : "食物" }}
                </span>
              </button>
            </div>
          </div>

          <!-- =========================
               份量
          ========================== -->
          <div class="mt-5">
            <label class="mb-2 block text-sm font-medium text-text-secondary">
              份量
            </label>

            <div class="grid gap-3 md:grid-cols-2">
              <!-- Quantity / Unit -->
              <div class="flex items-center gap-3">
                <!-- Quantity -->
                <input
                  v-model.number="item.quantity"
                  type="number"
                  min="0"
                  step="0.1"
                  class="min-w-0 flex-1 rounded-2xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none transition focus:border-primary"
                />

                <span class="text-sm text-text-muted"> × </span>

                <!-- Unit -->
                <input
                  v-model="item.unit"
                  type="text"
                  placeholder="單位"
                  class="min-w-0 flex-1 rounded-2xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none transition placeholder:text-text-muted focus:border-primary"
                />
              </div>

              <!-- 系統計算：目前份量熱量 -->
              <div
                class="flex min-h-13 items-center justify-between rounded-2xl bg-info-medium px-5 py-3"
              >
                <div>
                  <p class="text-xs text-text-muted">目前份量熱量</p>

                  <div class="mt-1 flex items-baseline gap-1">
                    <span class="text-xl font-semibold text-text-primary">
                      {{ getItemValue(item, "calories").toFixed(0) }}
                    </span>

                    <span class="text-sm text-text-muted"> kcal </span>
                  </div>
                </div>

                <!-- Food 的單位為 100g 時，
                     額外顯示目前實際克數 -->
                <p
                  v-if="item.unit === '100g'"
                  class="text-xs text-text-secondary"
                >
                  約
                  {{ (Number(item.quantity || 0) * 100).toFixed(0) }}
                  g
                </p>
              </div>
            </div>
          </div>

          <!-- =========================
               基準份量營養
          ========================== -->
          <div class="mt-6">
            <div class="mb-3">
              <h5 class="text-sm font-medium text-text-secondary">
                基準份量營養
              </h5>

              <p class="mt-1 text-xs text-text-muted">
                以下數值代表 1 單位的營養資訊，可手動調整
              </p>
            </div>

            <div class="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
              <!-- Calories -->
              <div>
                <label class="mb-2 block text-xs text-text-muted">
                  熱量 (kcal)
                </label>

                <input
                  v-model.number="item.baseCalories"
                  type="number"
                  min="0"
                  step="0.1"
                  class="w-full rounded-2xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none transition focus:border-primary"
                />
              </div>

              <!-- Carbs -->
              <div>
                <label class="mb-2 block text-xs text-text-muted">
                  碳水 (g)
                </label>

                <input
                  v-model.number="item.baseCarbs"
                  type="number"
                  min="0"
                  step="0.01"
                  class="w-full rounded-2xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none transition focus:border-primary"
                />
              </div>

              <!-- Protein -->
              <div>
                <label class="mb-2 block text-xs text-text-muted">
                  蛋白質 (g)
                </label>

                <input
                  v-model.number="item.baseProtein"
                  type="number"
                  min="0"
                  step="0.01"
                  class="w-full rounded-2xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none transition focus:border-primary"
                />
              </div>

              <!-- Fat -->
              <div>
                <label class="mb-2 block text-xs text-text-muted">
                  脂肪 (g)
                </label>

                <input
                  v-model.number="item.baseFat"
                  type="number"
                  min="0"
                  step="0.01"
                  class="w-full rounded-2xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none transition focus:border-primary"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- =========================
         本餐營養 Summary
    ========================== -->
    <section>
      <h3 class="mb-4 font-semibold text-text-primary">本餐營養</h3>

      <div
        class="grid overflow-hidden rounded-2xl border border-border bg-surface sm:grid-cols-2 lg:grid-cols-4"
      >
        <!-- Calories -->
        <div class="bg-primary-soft px-5 py-4">
          <p class="text-sm text-text-secondary">總熱量</p>

          <div class="mt-2 flex items-baseline gap-1">
            <span class="text-2xl font-semibold text-text-primary">
              {{ totals.calories.toFixed(0) }}
            </span>

            <span class="text-sm text-text-muted"> kcal </span>
          </div>
        </div>

        <!-- Carbs -->
        <div class="border-border px-5 py-4 sm:border-l">
          <p class="text-sm text-text-secondary">碳水</p>

          <div class="mt-2 flex items-baseline gap-1">
            <span class="text-2xl font-semibold text-text-primary">
              {{ totals.carbs.toFixed(1) }}
            </span>

            <span class="text-sm text-text-muted"> g </span>
          </div>
        </div>

        <!-- Protein -->
        <div class="border-border px-5 py-4 sm:border-l">
          <p class="text-sm text-text-secondary">蛋白質</p>

          <div class="mt-2 flex items-baseline gap-1">
            <span class="text-2xl font-semibold text-text-primary">
              {{ totals.protein.toFixed(1) }}
            </span>

            <span class="text-sm text-text-muted"> g </span>
          </div>
        </div>

        <!-- Fat -->
        <div class="border-border px-5 py-4 sm:border-l">
          <p class="text-sm text-text-secondary">脂肪</p>

          <div class="mt-2 flex items-baseline gap-1">
            <span class="text-2xl font-semibold text-text-primary">
              {{ totals.fat.toFixed(1) }}
            </span>

            <span class="text-sm text-text-muted"> g </span>
          </div>
        </div>
      </div>
    </section>

    <!-- =========================
         Footer
    ========================== -->
    <div class="flex justify-end gap-3 border-t border-border pt-5">
      <!-- Cancel -->
      <button
        type="button"
        class="rounded-2xl border border-border bg-surface px-5 py-2.5 text-sm font-medium text-text-secondary transition hover:bg-surface-soft"
        @click="$emit('cancel')"
      >
        取消
      </button>

      <!-- Submit -->
      <button
        type="submit"
        class="rounded-2xl bg-accent px-6 py-2.5 text-sm font-semibold text-text-primary transition hover:bg-accent-hover"
      >
        {{ record ? "儲存修改" : "新增紀錄" }}
      </button>
    </div>
  </form>
</template>
