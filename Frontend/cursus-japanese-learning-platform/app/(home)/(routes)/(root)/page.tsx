"use client";

import { redirect } from "next/navigation";
import { useEffect } from "react";

const RootPage = () => {
  useEffect(() => {
    redirect("/home");
  }, []);

  return null; // This component doesn't render anything itself
};

export default RootPage;