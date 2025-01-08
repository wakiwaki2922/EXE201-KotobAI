"use client";

import { getFullUserDetails, verifyEmailByToken } from "@/actions/authenticaton";
import { Button } from "@/components/ui/button";
import { useRouter } from "next/navigation";
import React, { useEffect, useRef, useState } from "react";
import Cookies from "js-cookie";

const VerifySuccess = ({ params }: { params: { token: string } }) => {
  const router = useRouter();
  const timerRef = useRef<NodeJS.Timeout | null>(null);
  const [countdown, setCountdown] = useState(5);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [verificationSuccess, setVerificationSuccess] = useState(false);
  const [done, setdone] = useState(false);

  useEffect(() => {
    const verifyEmail = async () => {
      try {
        let token = Cookies.get("jwtToken");
        if (token) {
          console.log("Token exists:");
        } else if (params.token && params.token.length > 0) {
          token = params.token;
          console.log("Token does not exist");
        } else {
          const url = window.location.href;

          token = url.includes("/verify/") ? url.split("/verify/")[1] : undefined;

          if (token) {
            console.log("Token exists:", token);
          } else {
            console.log("Token does not exist in URL");
          }
        }
        if (!token) {
          throw new Error("Token is undefined");
        }
        const response = await verifyEmailByToken(token);
        const accessToken = await response.data.accessToken;
        const refreshToken = await response.data.refreshToken;
        const userId = await response.data.userId;
        console.log("Response:", accessToken, refreshToken, userId);
        setVerificationSuccess(true);
        setdone(true);
        setLoading(false);
        try {
          setVerificationSuccess(true);
          setdone(true);
          setLoading(false);
          Cookies.set("jwtToken", accessToken, { expires: 1 });
          Cookies.set("refreshToken", refreshToken, { expires: 7 });
          const userData = await getFullUserDetails(userId);
          console.log("User Data Sign Up", userData);
          localStorage.setItem("userData", JSON.stringify(userData.data));
        } catch (error: any) {
          console.error("Get Full User Details failed:", error.message);
          setError(
            "Get Full User Details failed:" + String(error.message) ||
              "Something went wrong"
          );
        }
      } catch (error: any) {
        console.log("Verification failed", error);
        if (error.response && error.response.data) {
          setError(String(error.response.data) || "Something went wrong");
          setLoading(false);
          setdone(true);
        } else {
          setError(String(error.message) || "Something went wrong");
          setLoading(false);
          setdone(true);
        }
      }
    };
    verifyEmail();
  }, [params.token, router]);

  useEffect(() => {
    if (done == true) {
      timerRef.current = setTimeout(() => {
        if (
          error ==
          "You need to login again because you are expired your access."
        ) {
          router.push("/authenticate/login");
        } else router.push("/");
      }, 5000);

      const interval = setInterval(() => {
        setCountdown((prevCountdown) => prevCountdown - 1);
      }, 1000);

      return () => {
        if (timerRef.current) {
          clearTimeout(timerRef.current);
        }
        clearInterval(interval);
      };
    }
  }, [done, error, router]);

  const handleGoHome = () => {
    if (timerRef.current) {
      clearTimeout(timerRef.current);
    }
    router.push("/");
  };

  return (
    <div className="fixed flex inset-0 top-0 left-0 right-0 bottom-0 items-center justify-center bg-gradient">
      <div className="z-10 flex flex-col relative bg-white p-8 rounded-lg shadow-lg transform transition duration-500 hover:scale-105">
        {error && (
          <>
            <h2 className="text-3xl font-bold mb-4 text-center text-green-600">
              Email Verified Successfully!
            </h2>
            <h3 className="text-3xl font-bold mb-4 text-center text-black">
              But!!! {error}
            </h3>
            <p className="mb-4 text-center text-gray-600">
              You will be redirected to the next page in{" "}
              <span className="font-semibold text-gray-800">{countdown}</span>{" "}
              seconds...
            </p>
          </>
        )}
        {loading && (
          <p className="mb-4 text-center text-gray-600">Loading...</p>
        )}
        {!error && verificationSuccess && (
          <>
            <h2 className="text-3xl font-bold mb-4 text-center  text-green-600">
              Email Verified Successfully!
            </h2>
            <p className="mb-4 text-center text-gray-600">
              You will be redirected to the home page in{" "}
              <span className="font-semibold text-gray-800">{countdown}</span>{" "}
              seconds...
            </p>
            <div className="flex justify-center">
              <Button onClick={handleGoHome} disabled={loading}>
                Go to Home
              </Button>
            </div>
          </>
        )}
      </div>
    </div>
  );
};

export default VerifySuccess;
