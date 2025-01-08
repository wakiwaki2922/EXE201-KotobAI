import api from "@/lib/axios";

interface registerPayload {
  emailAddress: string;
  password: string;
  fullName: string;
}

interface userData {
  accessToken: string;
  refreshToken: string;
  userId: string;
  emailAddress: string;
  fullName: string;
  role: string;
}



const verifyEmailByToken = async (token: string) => {
  try {
    const endpoint = `/api/auth/email/verify/${token}`;
    const response = await api.get(endpoint);
    console.log("Response:", response.data);
    return response.data;
  } catch (error: any) {
    console.log("[ACTIONS_VERIFY_EMAIL]", error);
    if (error.response && error.response.data) {
      throw new Error(error.response.data.message || "Something went wrong!");
    } else {
      throw new Error(error.message || "Something went wrong!");
    }
  }
};

const login = async (payLoad: { emailAddress: string; password: string }) => {
  try {
    const endpoint = `/api/auth/login`;
    const response = await api.post(endpoint, payLoad);
    return response.data;
  } catch (error: any) {
    console.log("[ACTIONS_LOGIN]", error);
    if (error.response && error.response.data) {
      throw new Error(error.response.data.message || "Something went wrong!");
    } else {
      throw new Error(error.message || "Something went wrong!");
    }
  }
};

const loginWithGoogle = async (accessToken: string) => {
  try {
    const endpoint = `/api/auth/login/google`;
    console.log("Token: ", accessToken);
    const response = await api.post(endpoint, { AccessToken: accessToken });
    console.log("Backend response:", response.data);
    return response.data;
  } catch (error: any) {
    console.log("[ACTIONS_LOGIN_WITH_GOOGLE]", error);
    if (error.response && error.response.data) {
      throw new Error(error.response.data.message || "Login with Google fail!");
    } else {
      throw new Error(error.message || "Login with Google fail!");
    }
  }
};

const signUp = async (values: any) => {
  try {
    const endpoint = `/api/auth/register`;
    const response = await api.post(endpoint, values);
    return response.data;
  } catch (error: any) {
    console.log("API_USER_AUTHENTICATE_SIGN_UP", error);
    if (error.response && error.response.data) {
      throw new Error(error.response.data.message || "Signup fail!");
    } else {
      throw new Error(error.message || "Signup fail!");
    }
  }
};

const forgotPassword = async (email: string) => {
  try {
      const endpoint = `/api/auth/forgot-password/${email}`;
      const response = await api.get(endpoint);
      return response.data;
  } catch (error) {
      console.error("[ACTIONS_FORGOT_PASSWORD]", error);
      return null;
  }
};

const resetPassword = async (data: { password: string, verificationToken: string }) => {
  try {
      const endpoint = `/api/auth/reset-password`;
      const response = await api.post(endpoint, data);
      return response.data;
  } catch (error) {
      console.error("[ACTIONS_RESET_PASSWORD]", error);
      return null;
  }
}

const getFullUserDetails = async (id : string) => {
  try {
    const endpoint = `/api/auth/user/${id}`;
    const response = await api.get(endpoint);
    return response.data;
  } catch (error) {
    console.error("[ACTIONS_GET_FULL_USER_DETAILS]", error);
    return null;
  }
};

export {getFullUserDetails, verifyEmailByToken, login, loginWithGoogle, signUp, forgotPassword, resetPassword };
