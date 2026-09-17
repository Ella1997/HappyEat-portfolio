import api from "@/services/api";

export const geminiApi = {
  analyze(imageId, userPrompt = null) {
    return api.post(`/Gemini/analyze/${imageId}`, userPrompt);
  },
};
