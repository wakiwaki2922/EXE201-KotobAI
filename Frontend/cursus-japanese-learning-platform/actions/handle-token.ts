import api from "@/lib/axios";

const isTokenExpired = async (token: string) => {
  try {
    await api.get("/api/auth/token/validate?token=" + token);
    console.log("[ACTIONS_IS_TOKEN_EXPIRED] Token is valid");
    return false; // Token is valid
  } catch (error: any) {
    console.log("[ACTIONS_IS_TOKEN_EXPIRED]", error);
    return true; // Token is expired or invalid
  }
};

const getRefreshToken = async ({accessToken, refreshToken}: {accessToken: string, refreshToken: string}) => {
  try {
    console.log("[ACTIONS_GET_REFRESH_TOKEN] Requesting refresh token: accessToken", accessToken, "refreshToken", refreshToken);
    const response = await api.post("/api/auth/token/refresh", {
        accessToken,
        refreshToken,
    });
    console.log("[ACTIONS_GET_REFRESH_TOKEN] Refresh token response: ", response.data);
    return response.data;
  } catch (error: any) {
    console.error("[ACTIONS_GET_REFRESH_TOKEN]", error);
    if (error.response && error.response.data) {
      throw new Error(error.response.data.message || "Something went wrong!");
    } else {
      throw new Error(error.message || "Something went wrong!");
    }
  }
};

export { isTokenExpired, getRefreshToken };