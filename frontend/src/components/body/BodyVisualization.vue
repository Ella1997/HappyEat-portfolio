<script setup>
import bodyModelImg from "@/assets/images/body-model.png";

defineProps({
  neckSize: {
    type: Number,
    default: null,
  },
  waistSize: {
    type: Number,
    default: null,
  },
  hipSize: {
    type: Number,
    default: null,
  },
});
</script>

<template>
  <div
    class="flex items-center justify-center rounded-2xl bg-surface-soft px-4 py-5"
  >
    <!-- 人體圖片 + Measurement Overlay -->
    <div class="relative mx-auto h-120 w-full max-w-90">
      <!-- 人體底圖 -->
      <img
        :src="bodyModelImg"
        alt="人體體態示意圖"
        class="pointer-events-none absolute left-1/2 top-1/2 h-[94%] w-auto -translate-x-1/2 -translate-y-1/2 select-none object-contain"
        draggable="false"
      />

      <!-- =========================
           Measurement Lines
      ========================== -->
      <svg
        class="pointer-events-none absolute inset-0 h-full w-full"
        viewBox="0 0 100 100"
        preserveAspectRatio="none"
        aria-hidden="true"
      >
        <!-- 頸圍：右側 -->
        <polyline
          points="51,17 68,17 76,17"
          class="fill-none stroke-accent"
          vector-effect="non-scaling-stroke"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />

        <!-- 腰圍：右側 -->
        <polyline
          points="58,42 70,42 82,42"
          class="fill-none stroke-accent"
          vector-effect="non-scaling-stroke"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />

        <!-- 臀圍：左側 -->
        <polyline
          points="42,52 28,52 16,52"
          class="fill-none stroke-accent"
          vector-effect="non-scaling-stroke"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>

      <!-- =========================
           Measurement Points
      ========================== -->

      <!-- 頸圍 Point -->
      <div
        class="pointer-events-none absolute left-[51%] top-[17%] h-3 w-3 -translate-x-1/2 -translate-y-1/2 rounded-full border-2 border-white bg-accent shadow-sm"
      ></div>

      <!-- 腰圍 Point -->
      <div
        class="pointer-events-none absolute left-[58%] top-[42%] h-3 w-3 -translate-x-1/2 -translate-y-1/2 rounded-full border-2 border-white bg-accent shadow-sm"
      ></div>

      <!-- 臀圍 Point -->
      <div
        class="pointer-events-none absolute left-[42%] top-[52%] h-3 w-3 -translate-x-1/2 -translate-y-1/2 rounded-full border-2 border-white bg-accent shadow-sm"
      ></div>

      <!-- =========================
           Measurement Labels
      ========================== -->

      <!-- 頸圍 -->
      <div
        class="pointer-events-none absolute right-[1%] top-[12.5%] text-left"
      >
        <p class="text-xs font-medium text-text-secondary">頸圍</p>

        <p class="mt-0.5 text-sm font-bold text-text-primary">
          {{ neckSize ?? "-" }}

          <span class="text-xs font-normal text-text-muted"> cm </span>
        </p>
      </div>

      <!-- 腰圍 -->
      <div
        class="pointer-events-none absolute right-[1%] top-[37.5%] text-left"
      >
        <p class="text-xs font-medium text-text-secondary">腰圍</p>

        <p class="mt-0.5 text-sm font-bold text-text-primary">
          {{ waistSize ?? "-" }}

          <span class="text-xs font-normal text-text-muted"> cm </span>
        </p>
      </div>

      <!-- 臀圍 -->
      <div
        class="pointer-events-none absolute left-[1%] top-[47.5%] text-right"
      >
        <p class="text-xs font-medium text-text-secondary">臀圍</p>

        <p class="mt-0.5 text-sm font-bold text-text-primary">
          {{ hipSize ?? "-" }}

          <span class="text-xs font-normal text-text-muted"> cm </span>
        </p>
      </div>
    </div>
  </div>
</template>
