import { computed, reactive } from "vue";
import { foodSearchApi } from "@/services/foodSearchApi";

export function useFoodRecordForm(record = null) {
  const mealTypes = ["早餐", "早午餐", "午餐", "點心", "晚餐", "宵夜"];

  // =========================
  // Item
  // =========================
  const createEmptyItem = () => ({
    foodId: null,
    drinkId: null,
    itemName: "",
    quantity: 1,
    unit: "",

    // API 回傳的基準份量營養
    // Food：每 100g
    // Drink：每 500ml / 700ml
    baseCalories: null,
    baseCarbs: null,
    baseProtein: null,
    baseFat: null,

    // 搜尋 UI 狀態
    searchResults: [],
    isSearching: false,
    showDropdown: false,
    searchTimer: null,
  });

  // =========================
  // Form
  // =========================
  const createFormItem = (item) => {
    const quantity = Number(item.quantity) || 1;

    return {
      foodId: item.foodId ?? null,
      drinkId: item.drinkId ?? null,
      itemName: item.itemName ?? "",
      quantity,
      unit: item.unit ?? "",

      // FoodRecordItem 儲存的是實際份量營養
      // Edit 時除回 quantity，取得基準份量營養
      baseCalories: Number(item.calories ?? 0) / quantity,
      baseCarbs: Number(item.carbs ?? 0) / quantity,
      baseProtein: Number(item.protein ?? 0) / quantity,
      baseFat: Number(item.fat ?? 0) / quantity,

      searchResults: [],
      isSearching: false,
      showDropdown: false,
      searchTimer: null,
    };
  };

  const form = reactive({
    recordDate: record?.recordDate
      ? String(record.recordDate).slice(0, 10)
      : getTodayKey(),

    mealType: record?.mealType ?? "",
    description: record?.description ?? "",

    items:
      record?.items?.length > 0
        ? record.items.map(createFormItem)
        : [createEmptyItem()],
  });

  // =========================
  // Item CRUD
  // =========================
  const addItem = () => {
    form.items.push(createEmptyItem());
  };

  const removeItem = (index) => {
    if (form.items.length <= 1) return;

    const item = form.items[index];

    if (item.searchTimer) {
      clearTimeout(item.searchTimer);
    }

    form.items.splice(index, 1);
  };

  // =========================
  // Search
  // =========================
  const handleInputSearch = (item) => {
    // 名稱被重新修改後，
    // 原本選取的 DB 關聯就不應繼續保留
    item.foodId = null;
    item.drinkId = null;

    if (item.searchTimer) {
      clearTimeout(item.searchTimer);
    }

    const keyword = item.itemName.trim();

    if (!keyword) {
      item.searchResults = [];
      item.showDropdown = false;
      return;
    }

    item.searchTimer = setTimeout(async () => {
      item.isSearching = true;

      try {
        const response = await foodSearchApi.search(keyword);

        item.searchResults = response.data ?? [];
        item.showDropdown = item.searchResults.length > 0;
      } catch (err) {
        console.error("搜尋食物或飲料失敗：", err);

        item.searchResults = [];
        item.showDropdown = false;
      } finally {
        item.isSearching = false;
      }
    }, 300);
  };

  const selectSearchResult = (item, selected) => {
    item.itemName = selected.name;
    item.unit = selected.unit ?? "";
    item.quantity = 1;

    if (selected.type === "Drink") {
      item.drinkId = selected.id;
      item.foodId = null;
    } else {
      item.foodId = selected.id;
      item.drinkId = null;
    }

    item.baseCalories = Number(selected.calories ?? 0);
    item.baseCarbs = Number(selected.carbs ?? 0);
    item.baseProtein = Number(selected.protein ?? 0);
    item.baseFat = Number(selected.fat ?? 0);

    item.searchResults = [];
    item.showDropdown = false;
  };

  const hideDropdown = (item) => {
    setTimeout(() => {
      item.showDropdown = false;
    }, 200);
  };

  // =========================
  // Nutrition
  // =========================
  const getItemValue = (item, key) => {
    const quantity = Number(item.quantity) || 0;

    const baseKey = `base${key.charAt(0).toUpperCase()}${key.slice(1)}`;

    const baseValue = Number(item[baseKey]) || 0;

    return baseValue * quantity;
  };

  const totals = computed(() => {
    return form.items.reduce(
      (total, item) => {
        total.calories += getItemValue(item, "calories");
        total.carbs += getItemValue(item, "carbs");
        total.protein += getItemValue(item, "protein");
        total.fat += getItemValue(item, "fat");

        return total;
      },
      {
        calories: 0,
        carbs: 0,
        protein: 0,
        fat: 0,
      },
    );
  });

  // =========================
  // Item Type
  // =========================
  const getItemTypeLabel = (item) => {
    if (item.drinkId) return "飲料";
    if (item.foodId) return "食物";

    return "自訂";
  };

  const getItemTypeClass = (item) => {
    if (item.drinkId) {
      return "bg-info-strong text-text-primary";
    }

    if (item.foodId) {
      return "bg-primary-soft text-primary-dark";
    }

    return "bg-surface-soft text-text-secondary";
  };

  // =========================
  // Payload
  // =========================
  const buildPayload = () => ({
    recordDate: form.recordDate,
    mealType: form.mealType,
    description: form.description || null,

    items: form.items.map((item) => ({
      itemName: item.itemName || null,
      foodId: item.foodId ?? null,
      drinkId: item.drinkId ?? null,
      quantity: toNullableNumber(item.quantity),
      unit: item.unit || null,

      calories: round(getItemValue(item, "calories")),
      carbs: round(getItemValue(item, "carbs")),
      protein: round(getItemValue(item, "protein")),
      fat: round(getItemValue(item, "fat")),
    })),
  });

  return {
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
  };
}

// =========================
// Helpers
// =========================
function getTodayKey() {
  const today = new Date();

  const year = today.getFullYear();
  const month = String(today.getMonth() + 1).padStart(2, "0");
  const day = String(today.getDate()).padStart(2, "0");

  return `${year}-${month}-${day}`;
}

function toNullableNumber(value) {
  if (value === "" || value == null) {
    return null;
  }

  return Number(value);
}

function round(value) {
  return Number(Number(value).toFixed(2));
}
