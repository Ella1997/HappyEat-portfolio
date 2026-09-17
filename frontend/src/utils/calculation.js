export function calculateBMI(weight, height) {
  if (!weight || !height) return null;
  const heightMeter = height / 100;
  const bmi = weight / (heightMeter * heightMeter);
  return Number(bmi.toFixed(1));
}

export function calculateBMR({ weight, height, age, gender }) {
  if (!weight || !height || !age || !gender) return null;
  const base = 10 * weight + 6.25 * height - 5 * age;
  if (gender === "male") return Math.round(base + 5);
  if (gender === "female") return Math.round(base - 161);
  return null;
}

export function calculateTDEE(bmr, activityLevel) {
  if (!bmr || !activityLevel) return null;
  return Math.round(bmr * activityLevel); //Mifflin–St Jeor BMR公式
}

export function calculateAge(birthDate) {
  if (!birthDate) return null;
  const birth = new Date(birthDate);
  const today = new Date();

  let age = today.getFullYear() - birth.getFullYear();
  const monthDifference = today.getMonth() - birth.getMonth();
  if (
    monthDifference < 0 ||
    (monthDifference === 0 && today.getDate() < birth.getDate())
  ) {
    age--;
  }
  return age;
}

export function calculateRecommendedCal(tdee, goalType) {
  if (!tdee) return null;
  switch (goalType) {
    case "減脂":
      return Math.round(tdee - 300);
    case "增肌":
      return Math.round(tdee + 300);
    case "維持":
      return Math.round(tdee);
    default:
      return Math.round(tdee);
  }
}

export function calculateGoalWeightStatus(
  currentWeight,
  targetWeight,
  goalType,
) {
  if (!currentWeight || !targetWeight) return null;

  let achieved = false;

  if (goalType === "減脂") achieved = currentWeight <= targetWeight;
  else if (goalType === "增肌") achieved = currentWeight >= targetWeight;
  else achieved = currentWeight === targetWeight;

  return {
    achieved,
    difference: Number(Math.abs(currentWeight - targetWeight).toFixed(1)),
  };
}

export function calculateGoalProgress(
  startWeight,
  currentWeight,
  targetWeight,
  goalType,
) {
  if (startWeight == null || currentWeight == null || targetWeight == null) {
    return null;
  }

  let totalChange;
  let completedChange;

  if (goalType === "減脂") {
    totalChange = startWeight - targetWeight;
    completedChange = startWeight - currentWeight;
  } else if (goalType === "增肌") {
    totalChange = targetWeight - startWeight;
    completedChange = currentWeight - startWeight;
  } else {
    return null;
  }

  // 防止不合理或異常的目標資料產生錯誤進度
  if (totalChange <= 0) return null;

  const rawProgress = (completedChange / totalChange) * 100;

  const progress = Math.min(100, Math.max(0, Math.round(rawProgress)));

  return {
    startWeight,
    currentWeight,
    targetWeight,
    progress,
  };
}

export function calculateWaistHipRatio(waistSize, hipSize) {
  if (!waistSize || !hipSize) return null;

  return Number((waistSize / hipSize).toFixed(2));
}
