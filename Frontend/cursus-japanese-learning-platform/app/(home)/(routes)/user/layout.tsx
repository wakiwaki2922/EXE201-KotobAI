"use client";

import { useEffect, useState } from "react";
import { Banner } from "@/components/banner";
import { isAccountVerified, isStudentBlocked } from "@/lib/account-block";
import { isLoggedIn } from "@/lib/login-check";
import { ReactNode } from "react";

const UserLayout = ({ children }: { children: ReactNode }) => {
  const [studentBlocked, setStudentBlocked] = useState(false);
  const [accountVerified, setAccountVerified] = useState(true);

  useEffect(() => {
    isLoggedIn();
    setStudentBlocked(isStudentBlocked());
    setAccountVerified(isAccountVerified());
  }, []);

  return (
    <>
      {studentBlocked && (
        <Banner label="Your student account is blocked. Please contact support." />
      )}
      {!accountVerified && (
        <Banner label="Your account is not verified. Please verify your account to continue." />
      )}
      {children}
    </>
  );
};

export default UserLayout;
