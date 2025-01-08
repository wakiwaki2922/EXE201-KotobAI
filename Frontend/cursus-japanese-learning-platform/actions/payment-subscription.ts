import api from "@/lib/axios";
import { PackagePlan } from "@/types/PackagePlan";

async function createSubscriptionPayment(plan: PackagePlan) {
  try {
    const BASE_FE_URL = process.env.BASE_API_URL_FE;
    const endpoint = `/api/payments`;
    const response = await api.post(endpoint, {
      packageId: plan.id,
      returnUrl: `${BASE_FE_URL}/payments/success`,
      cancelUrl: `${BASE_FE_URL}/payments/cancel`,
    });
    console.log("Create subscription payment", response.data);
    return response.data;
  } catch (error: any) {
    console.error("[ACTIONS_CREATE_SUBSCRIPTION_PAYMENT]", error);
    if (error.response && error.response.data) {
      throw new Error(error.response.data.message || "Something went wrong!");
    } else {
      throw new Error(error.message || "Something went wrong!");
    }
  }
}

const completeSubscription = async (paymentId: string, payerId: string, token: string) => {
  try {
    const endpoint = `/api/payments/complete?paymentId=${paymentId}&payerId=${payerId}&token=${token}`;
    const response = await api.get(endpoint);
    console.log("Subscription completed successfully:", response.data);
    return response.data;
  } catch (error: any) {
    console.log("[ACTIONS_COMPLETE_SUBSCRIPTION]", error);
    if (error.response && error.response.data) {
      throw new Error(error.response.data.message || "Something went wrong!");
    } else {
      throw new Error(error.message || "Something went wrong!");
    }
  }
};

export { createSubscriptionPayment, completeSubscription };
