import { createRouter, createWebHistory } from "vue-router";
import BodyManagementView from "@/views/BodyManagementView.vue";
import FoodDiaryView from "../views/FoodDiaryView.vue";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/body",
      name: "body",
      component: BodyManagementView,
    },
    {
      path: "/food",
      name: "food",
      component: FoodDiaryView,
    },
  ],
});

export default router;
