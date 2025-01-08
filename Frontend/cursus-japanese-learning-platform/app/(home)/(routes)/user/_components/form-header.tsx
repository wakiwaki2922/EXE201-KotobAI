"use client";

import { getUserById } from "@/actions/user-management";
import { Button } from "@/components/ui/button";
import { Toaster } from "@/components/ui/toaster";
import { isAccountVerified } from "@/lib/account-block";
import { User } from "@/types/User";
import { UserData } from "@/types/UserData";
import React, { useEffect, useState } from "react";
import toast from "react-hot-toast";
import Cookies from 'js-cookie';
import { verifyEmailByToken } from "@/actions/authenticaton";


const FormHeader = () => {
  const [user, setUser] = useState<User>();
  const [userData, setUserData] = useState<UserData>();
  useEffect(() => {
    const fetchUser = async () => {
      try {
        const userDataString = localStorage.getItem('userData');
        if (userDataString == null) {
          return;
        }
        setUserData(JSON.parse(userDataString));
        if (userData) {
          const response = await getUserById(userData.id);
          setUser(response.data);
        }
      } catch (error: any) {
        toast.error(error.message || "Failed to get user");
      }
    };
    fetchUser();
  }, []);

  if (!user) {
    return null;
  }

  console.log("Form header user data", user);

  const handleOnClick = async () => {
    try {
      const token = Cookies.get('jwtToken');
      if (!token) {
        throw new Error("Token is undefined");
      }
      await verifyEmailByToken(token);
      toast.success("Verification email sent! Check your inbox");
    } catch (error: any) {
      toast.error(error.message || "Something went wrong");
    }
  };

  return (
    <>
      <Toaster />
      <div className="space-y-0.5">
        <h2 className="text-2xl font-bold tracking-tight">Settings</h2>
        <p className="text-muted-foreground">
          Manage your account settings and what you wanna be in Cursus
        </p>
      </div>
      {!isAccountVerified() && (
        <Button onClick={handleOnClick}>Verify account</Button>
      )}
    </>
  );
};

export default FormHeader;
