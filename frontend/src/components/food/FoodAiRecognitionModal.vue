<script setup>
import { computed, onBeforeUnmount, ref } from "vue";
import { foodImageApi } from "@/services/foodImageApi";
import { geminiApi } from "@/services/geminiApi";

const props = defineProps({
  userId: {
    type: Number,
    required: true,
  },
});

const emit = defineEmits(["confirm", "close"]);

// =========================
// State
// =========================

const imageFile = ref(null);
const imagePreviewUrl = ref(null);
const imageId = ref(null);

const result = ref(null);
const userPrompt = ref("");

const isAnalyzing = ref(false);
const errorMessage = ref("");

const hasResult = computed(() => result.value !== null);

// =========================
// Image
// =========================

const handleImageChange = (event) => {
  const file = event.target.files?.[0];

  if (!file) return;

  if (imagePreviewUrl.value) {
    URL.revokeObjectURL(imagePreviewUrl.value);
  }

  imageFile.value = file;
  imagePreviewUrl.value = URL.createObjectURL(file);

  // 更換圖片後清除舊辨識狀態
  imageId.value = null;
  result.value = null;
  userPrompt.value = "";
  errorMessage.value = "";
};

// =========================
// First Analyze
// =========================

const handleAnalyze = async () => {
  if (!imageFile.value) {
    errorMessage.value = "請先選擇餐點照片。";
    return;
  }

  isAnalyzing.value = true;
  errorMessage.value = "";

  let uploadedImageId = null;

  try {
    // 1. 上傳 FoodImage
    const formData = new FormData();

    formData.append("UserId", props.userId);
    formData.append("Image", imageFile.value);

    const imageResponse = await foodImageApi.create(formData);

    uploadedImageId = imageResponse.data.imageId;
    imageId.value = imageResponse.data.imageId;

    // 2. 第一次辨識，不提供補充說明
    const geminiResponse = await geminiApi.analyze(uploadedImageId, null);

    result.value = geminiResponse.data;
  } catch (err) {
    console.error("AI 辨識失敗：", err);
    errorMessage.value = getAiErrorMessage(err);
    //將未有辨識結果的圖片從DB刪除
    if (uploadedImageId) {
      try {
        await foodImageApi.delete(uploadedImageId);
        imageId.value = null;
      } catch (deleteErr) {
        console.error("清除便是失敗圖片時發生錯誤:", deleteErr);
      }
    }
  } finally {
    isAnalyzing.value = false;
  }
};

// =========================
// Error Message
// =========================

const getAiErrorMessage = (err) => {
  const detail = err.response?.data?.detail ?? "";

  // Gemini 忙碌 / Timeout / 高流量
  if (
    detail.includes("Gemini is busy") ||
    detail.includes("Gemini Timeout") ||
    detail.includes("high demand")
  ) {
    return "AI 目前使用人數較多，暫時無法完成辨識，請稍後再試。";
  }

  // 其他 Gemini 錯誤
  if (detail.includes("Gemini") || detail.includes("AI")) {
    return "AI 辨識暫時無法使用，請稍後再試。";
  }

  // 其他未知錯誤
  return "餐點辨識失敗，請稍後再試。";
};

// =========================
// Re-analyze
// =========================

const handleReanalyze = async () => {
  const prompt = userPrompt.value.trim();

  if (!prompt) {
    errorMessage.value = "請先輸入補充說明。";
    return;
  }

  if (!imageId.value) {
    errorMessage.value = "找不到圖片資料，請重新上傳照片。";
    return;
  }

  isAnalyzing.value = true;
  errorMessage.value = "";

  try {
    const response = await geminiApi.analyze(imageId.value, prompt);

    // 用新的辨識結果覆蓋原結果
    result.value = response.data;

    userPrompt.value = "";
  } catch (err) {
    console.error("重新辨識失敗：", err);

    errorMessage.value = getAiErrorMessage(err);
  } finally {
    isAnalyzing.value = false;
  }
};

// =========================
// Confirm
// =========================

const handleConfirm = () => {
  if (!result.value || !imageId.value) return;

  emit("confirm", {
    imageId: imageId.value,
    result: result.value,
  });
};

// =========================
// Cleanup
// =========================

onBeforeUnmount(() => {
  if (imagePreviewUrl.value) {
    URL.revokeObjectURL(imagePreviewUrl.value);
  }
});
</script>

<template>
  <!-- =========================
       Modal Overlay
  ========================== -->
  <div
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/35 p-4"
    @click.self="!isAnalyzing && emit('close')"
  >
    <!-- =========================
         Modal
    ========================== -->
    <div
      class="scrollbar-none relative max-h-[90vh] w-full max-w-3xl overflow-y-auto rounded-3xl border border-border bg-modal-content shadow-xl"
    >
      <!-- =========================
           Header
      ========================== -->
      <div
        class="sticky top-0 z-20 flex items-center justify-between border-b border-border bg-modal-header px-6 py-5"
      >
        <div>
          <h2 class="text-lg font-semibold text-text-primary">AI 飲食辨識</h2>

          <p class="mt-1 text-sm text-text-muted">
            上傳餐點照片，由 AI 協助分析食物與營養資訊
          </p>
        </div>

        <button
          type="button"
          class="flex h-9 w-9 items-center justify-center rounded-xl text-text-muted transition hover:bg-white/60 hover:text-text-primary"
          aria-label="關閉 AI 飲食辨識"
          :disabled="isAnalyzing"
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
      </div>

      <!-- =========================
           Content
      ========================== -->
      <div class="p-6">
        <!-- =========================
             Upload State
        ========================== -->
        <section v-if="!hasResult" class="space-y-5">
          <!-- Image Upload -->
          <label
            class="flex min-h-64 cursor-pointer flex-col items-center justify-center overflow-hidden rounded-3xl border border-dashed border-primary bg-surface-soft transition hover:bg-primary-soft/30"
          >
            <img
              v-if="imagePreviewUrl"
              :src="imagePreviewUrl"
              alt="餐點照片預覽"
              class="max-h-80 w-full object-contain"
            />

            <div
              v-else
              class="flex flex-col items-center justify-center px-6 py-14 text-center"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-10 w-10 text-text-muted"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                stroke-width="1.7"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M3 16.5V6.75A2.25 2.25 0 0 1 5.25 4.5h13.5A2.25 2.25 0 0 1 21 6.75v10.5a2.25 2.25 0 0 1-2.25 2.25H5.25A2.25 2.25 0 0 1 3 17.25v-.75Zm0 0 4.72-4.72a2.25 2.25 0 0 1 3.182 0l1.348 1.348 2.098-2.098a2.25 2.25 0 0 1 3.182 0L21 14.5M15.75 8.25h.008v.008h-.008V8.25Z"
                />
              </svg>

              <p class="mt-3 text-sm font-medium text-text-primary">
                點擊選擇餐點照片
              </p>

              <p class="mt-1 text-xs text-text-muted">
                JPG、PNG 等常見圖片格式
              </p>
            </div>

            <input
              type="file"
              accept="image/*"
              class="hidden"
              @change="handleImageChange"
            />
          </label>

          <!-- Error -->
          <div
            v-if="errorMessage"
            class="flex items-start gap-3 rounded-2xl border border-danger/20 bg-danger/10 px-4 py-3.5"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="mt-0.5 h-5 w-5 shrink-0 text-danger"
            >
              <circle cx="12" cy="12" r="10" />
              <path d="M12 8v4" />
              <path d="M12 16h.01" />
            </svg>

            <div>
              <p class="text-sm font-medium text-text-primary">
                辨識暫時無法完成
              </p>

              <p class="mt-0.5 text-sm leading-6 text-text-secondary">
                {{ errorMessage }}
              </p>
            </div>
          </div>

          <!-- Footer -->
          <div class="flex justify-end gap-3 border-t border-border pt-5">
            <button
              type="button"
              class="rounded-xl border border-border bg-surface px-5 py-2.5 text-sm font-medium text-text-secondary transition hover:bg-surface-soft"
              @click="emit('close')"
            >
              取消
            </button>

            <button
              type="button"
              class="rounded-xl bg-primary px-5 py-2.5 text-sm font-semibold text-text-primary transition hover:bg-primary-dark hover:text-white disabled:cursor-not-allowed disabled:opacity-50"
              :disabled="!imageFile"
              @click="handleAnalyze"
            >
              開始辨識
            </button>
          </div>
        </section>

        <!-- =========================
             Result State
        ========================== -->
        <section v-else class="space-y-6">
          <!-- Image + Description -->
          <div class="grid gap-5 md:grid-cols-[220px_1fr]">
            <div
              class="overflow-hidden rounded-2xl border border-border bg-surface"
            >
              <img
                :src="imagePreviewUrl"
                alt="餐點照片"
                class="h-full max-h-56 w-full object-cover"
              />
            </div>

            <div>
              <p class="text-sm font-medium text-primary-dark">AI 辨識完成</p>

              <h3 class="mt-1 text-lg font-semibold text-text-primary">
                辨識結果
              </h3>

              <p
                v-if="result.description"
                class="mt-2 text-sm leading-6 text-text-secondary"
              >
                {{ result.description }}
              </p>
            </div>
          </div>

          <!-- Items -->
          <div
            class="overflow-hidden rounded-2xl border border-border bg-surface"
          >
            <div
              class="grid grid-cols-[1fr_90px_100px] gap-3 border-b border-border bg-surface-soft px-4 py-3 text-xs font-medium text-text-muted"
            >
              <span>餐點</span>
              <span>份量</span>
              <span class="text-right">熱量</span>
            </div>

            <div
              v-for="(item, index) in result.items"
              :key="index"
              class="grid grid-cols-[1fr_90px_100px] gap-3 border-b border-border px-4 py-4 last:border-b-0"
            >
              <div>
                <p class="font-medium text-text-primary">
                  {{ item.itemName }}
                </p>

                <p class="mt-1 text-xs text-text-muted">
                  碳水 {{ Number(item.carbs ?? 0).toFixed(1) }}g · 蛋白質
                  {{ Number(item.protein ?? 0).toFixed(1) }}g · 脂肪
                  {{ Number(item.fat ?? 0).toFixed(1) }}g
                </p>
              </div>

              <p class="text-sm text-text-secondary self-end">
                {{ item.quantity ?? "-" }}
                {{ item.unit ?? "" }}
              </p>

              <p
                class="text-right text-sm font-semibold text-text-primary self-end"
              >
                {{ Number(item.calories ?? 0).toFixed(0) }}
                kcal
              </p>
            </div>
          </div>

          <!-- Nutrition Summary -->
          <div class="grid grid-cols-2 gap-3 md:grid-cols-4">
            <div class="rounded-2xl bg-primary-soft p-4">
              <p class="text-xs text-text-muted">總熱量</p>

              <p class="mt-1 text-lg font-semibold text-text-primary">
                {{ Number(result.totalCalories ?? 0).toFixed(0) }}

                <span class="text-xs font-normal text-text-muted"> kcal </span>
              </p>
            </div>

            <div class="rounded-2xl bg-surface-soft p-4">
              <p class="text-xs text-text-muted">碳水</p>

              <p class="mt-1 text-lg font-semibold text-text-primary">
                {{ Number(result.totalCarbs ?? 0).toFixed(1) }}

                <span class="text-xs font-normal text-text-muted"> g </span>
              </p>
            </div>

            <div class="rounded-2xl bg-surface-soft p-4">
              <p class="text-xs text-text-muted">蛋白質</p>

              <p class="mt-1 text-lg font-semibold text-text-primary">
                {{ Number(result.totalProtein ?? 0).toFixed(1) }}

                <span class="text-xs font-normal text-text-muted"> g </span>
              </p>
            </div>

            <div class="rounded-2xl bg-surface-soft p-4">
              <p class="text-xs text-text-muted">脂肪</p>

              <p class="mt-1 text-lg font-semibold text-text-primary">
                {{ Number(result.totalFat ?? 0).toFixed(1) }}

                <span class="text-xs font-normal text-text-muted"> g </span>
              </p>
            </div>
          </div>

          <!-- Re-analyze -->
          <div class="rounded-2xl border border-border bg-info-medium p-5">
            <h4 class="text-sm font-semibold text-text-primary">
              辨識結果不準確嗎？
            </h4>

            <p class="mt-1 text-sm text-text-secondary">
              補充餐點資訊後，可以請 AI 使用同一張照片重新分析。
            </p>

            <textarea
              v-model="userPrompt"
              rows="3"
              placeholder="例如：白飯大約只有半碗，肉是去皮雞腿肉。"
              class="mt-4 w-full resize-none rounded-xl border border-border bg-surface px-4 py-3 text-sm text-text-primary outline-none placeholder:text-text-muted focus:border-primary"
            ></textarea>

            <div class="mt-3 flex justify-end">
              <button
                type="button"
                class="rounded-xl border border-primary bg-surface px-4 py-2 text-sm font-medium text-primary-dark transition hover:bg-primary-soft disabled:cursor-not-allowed disabled:opacity-50"
                :disabled="!userPrompt.trim()"
                @click="handleReanalyze"
              >
                重新辨識
              </button>
            </div>
          </div>

          <!-- Error -->
          <div
            v-if="errorMessage"
            class="flex items-start gap-3 rounded-2xl border border-danger/20 bg-danger/10 px-4 py-3.5"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="mt-0.5 h-5 w-5 shrink-0 text-danger"
            >
              <circle cx="12" cy="12" r="10" />
              <path d="M12 8v4" />
              <path d="M12 16h.01" />
            </svg>

            <div>
              <p class="text-sm font-medium text-text-primary">
                辨識暫時無法完成
              </p>

              <p class="mt-0.5 text-sm leading-6 text-text-secondary">
                {{ errorMessage }}
              </p>
            </div>
          </div>

          <!-- Footer -->
          <div
            class="flex items-center justify-between border-t border-border pt-5"
          >
            <button
              type="button"
              class="rounded-xl border border-border bg-surface px-5 py-2.5 text-sm font-medium text-text-secondary transition hover:bg-surface-soft"
              @click="emit('close')"
            >
              取消
            </button>

            <button
              type="button"
              class="rounded-xl bg-primary px-5 py-2.5 text-sm font-semibold text-text-primary transition hover:bg-primary-dark hover:text-white"
              @click="handleConfirm"
            >
              確認辨識結果
            </button>
          </div>
        </section>
      </div>

      <!-- =====================================================
           AI Loading Overlay
      ====================================================== -->
      <div
        v-if="isAnalyzing"
        class="absolute inset-0 z-50 flex items-center justify-center rounded-3xl bg-white/80 backdrop-blur-[1px]"
      >
        <div class="flex flex-col items-center">
          <!-- Spinner -->
          <svg
            class="h-11 w-11 animate-spin text-primary-dark"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
          >
            <circle
              class="opacity-25"
              cx="12"
              cy="12"
              r="10"
              stroke="currentColor"
              stroke-width="4"
            />

            <path
              class="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 0 1 8-8v4a4 4 0 0 0-4 4H4Z"
            />
          </svg>

          <p class="mt-4 text-base font-semibold text-text-primary">
            AI 正在辨識餐點
          </p>

          <p class="mt-1 text-sm text-text-muted">
            正在分析食物與營養資訊，請稍候...
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
