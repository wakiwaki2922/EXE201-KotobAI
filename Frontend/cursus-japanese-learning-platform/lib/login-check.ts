import Cookies from "js-cookie";
import toast from "react-hot-toast";
import { getRefreshToken, isTokenExpired as verifyToken } from "@/actions/handle-token";

const RedirectLogin = () => {
  if (typeof window !== "undefined") {
    localStorage.removeItem("userData");
    Cookies.remove("jwtToken");
    Cookies.remove("refreshToken");
    toast.error("Session expired. Please login again");
    window.location.href = "/authenticate/login";
  }
};

export const isLoggedIn = async () => {
  const token = Cookies.get("jwtToken");
  const refreshToken = Cookies.get("refreshToken");

  if (!token || !refreshToken) {
    RedirectLogin();
    return false;
  }

  if (await verifyToken(token) === true) {

    try {
      const response = await getRefreshToken({accessToken: token, refreshToken: refreshToken});
      console.log("Refresh token response: ", response);
      Cookies.set("jwtToken", response.data.accessToken);
      Cookies.set("refreshToken", response.data.refreshToken);
      return true;
    } catch (error) {
      handleErrorResponse(error);
      RedirectLogin();
      return false;
    }
  }
  return true;
};

const handleErrorResponse = (error: any) => {
  if (error.response) {
    switch (error.response.status) {
      case 403:
        toast.error("You do not have permission to access this resource.");
        break;
      case 401:
        toast.error("Session expired. Please login again.");
        break;
      case 500:
        toast.error("Server error. Please try again later.");
        break;
      default:
        toast.error("An error occurred. Please try again.");
    }
  } else {
    toast.error("Network error. Please check your connection.");
  }
};
