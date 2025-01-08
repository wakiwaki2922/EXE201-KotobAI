import axios from "axios";
import Cookies from "js-cookie";

const api = axios.create({
  baseURL: process.env.BASE_API_URL,
  withCredentials: true,
});

api.interceptors.request.use(
  async (config) => {
    let token;

    if (typeof window !== "undefined") {
      // Client-side code
      token = Cookies.get('jwtToken');
      console.log("Client side cookies: ", token);
    }

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // Set 'Content-Type': 'application/json' if the request is not FormData
    if (!(config.data instanceof FormData)) {
      config.headers["Content-Type"] = "application/json";
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;

