import api from "@/lib/axios";

const searchesForVocabularyAPI = async (word: string) => {
  try {
    const response = await api.get(`/api/vocabularies/search/api?word=${word}`);
    return response.data; 
  } catch (error: any) {
    console.log("[ACTIONS_SEARCHES_FOR_VOCABULARY_API]", error);
    return true; 
  }
};

export { searchesForVocabularyAPI };