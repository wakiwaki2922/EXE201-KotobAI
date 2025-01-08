"use client";

import { usePathname } from "next/navigation";
import SearchInput from "./search-input";
import Link from "next/link";
import { Button } from "./ui/button";
import {
  LogIn,
  LogOut,
  Settings,
} from "lucide-react";
import { useEffect, useState } from "react";
import { Avatar, AvatarImage } from "./ui/avatar";
import { signOutUser } from "@/lib/firebase";
import Cookies from "js-cookie";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "./ui/dropdown-menu";
import toast, { Toaster } from "react-hot-toast";
import { isAdmin } from "@/lib/admin";
import { UserData } from "@/types/UserData";


const NavbarRoutes = () => {
  const pathName = usePathname();

  const isAdminPage = pathName?.includes("/admin");
  const isSearchPage = pathName === "/home";
  const isLoginPage = pathName?.includes("authenticate");
  const isUpdateAvatarPage = pathName?.includes("avatar");

  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [userData, setUserData] = useState<UserData | null>(null);
  const [isClient, setIsClient] = useState(false);

  useEffect(() => {
    if (typeof window !== "undefined") {
      const storedUserData = localStorage.getItem("userData");
      if (storedUserData) {
        setUserData(JSON.parse(storedUserData));
        setIsLoggedIn(true);
      }
      setIsClient(true);
    }
  }, [isLoginPage, isUpdateAvatarPage]);

  const handleSignOut = async () => {
    await signOutUser();
    localStorage.removeItem("userData");
    Cookies.remove("jwtToken");
    setUserData(null);
    setIsLoggedIn(false);
    toast.success("Signed out successfully");
    window.location.href = `/`;
  };

  const handleSearch = (data: any) => {
    // Save search data to localStorage
    localStorage.setItem("searchData", JSON.stringify(data));

    // Trigger a custom event to notify other components
    const event = new CustomEvent("searchUpdated", { detail: data });
    window.dispatchEvent(event);
  };

  if (!isClient) return null;

  return (
    <>
      <Toaster />
      {isSearchPage && (
        <div className="hidden md:block">
          <SearchInput onSearch={handleSearch} />
        </div>
      )}
      <div className="flex gap-x-2 ml-auto align-middle items-center">
        {isClient && (
          <>
            {isAdminPage ? (
              <Link href="/">
                <Button size="sm" variant="ghost">
                  <LogOut className="h-4 w-4 mr-2" />
                  Exit Admin Mode
                </Button>
              </Link>
            ) : isAdmin() ? (
              <Link href="/admin/dashboard">
                <Button size="sm" variant="ghost">
                  Admin Dashboard
                </Button>
              </Link>
            ) : null}
          </>
        )}
        {!isLoginPage && (
          <>
            {isClient && isLoggedIn ? (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Avatar className="h-8 w-8">
                    <AvatarImage
                      src={userData?.imagePath || "/avatar-default.svg"}
                    />
                  </Avatar>
                </DropdownMenuTrigger>
                <DropdownMenuContent className="w-40">
                  <DropdownMenuLabel>My Account</DropdownMenuLabel>
                  <DropdownMenuSeparator />
                  <DropdownMenuGroup>
                    <DropdownMenuItem>
                      <Link href="/user/profile">
                        <Button size="sm" variant="ghost">
                          <Settings className="mr-2 h-4 w-4" />
                          <span>Settings</span>
                        </Button>
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem>
                      <Button size="sm" variant="ghost" onClick={handleSignOut}>
                        <LogOut className="h-4 w-4 mr-2" />
                        Sign Out
                      </Button>
                    </DropdownMenuItem>
                  </DropdownMenuGroup>
                </DropdownMenuContent>
              </DropdownMenu>
            ) : (
              <Link href="/authenticate/login">
                <Button size="sm" variant="ghost" className="bg-sky-200">
                  <LogIn className="h-4 w-4 mr-2" />
                  Login
                </Button>
              </Link>
            )}
          </>
        )}
      </div>
    </>
  );
};

export default NavbarRoutes;
