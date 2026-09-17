export function formatActivityLevel(activityLevel) {
  if (!activityLevel) return "-";

  const activityLevels = {
    1.2: "久坐無運動",
    1.375: "輕度活動",
    1.55: "中度活動",
    1.725: "高強度運動",
  };

  return activityLevels[activityLevel] ?? "未知";
}
