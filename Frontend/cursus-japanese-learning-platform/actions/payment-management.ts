import api from "@/lib/axios";

const getPaymentOfUser = async (id: string) => {
    try {
      const response = await api.get(`/api/payments/history/${id}`);
      return response.data; 
    } catch (error: any) {
      console.log("[ACTIONS_GET_USER_BY_ID]", error);
      return true; 
    }
};

export { getPaymentOfUser };