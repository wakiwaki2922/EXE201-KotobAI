
import { isAdmin } from "@/lib/admin";
import Image from "next/image";
import Link from "next/link";
import { useEffect, useState } from "react";

const Logo = () => {
  const [redirectPath, setRedirectPath] = useState("/");

  useEffect(() => {
    if (isAdmin()) {
      setRedirectPath("/admin/dashboard");
    } else {
      setRedirectPath("/");
    }
  }, []);

  return (
    <>
      <Link href={redirectPath}>
        <Image
          height={60}
          width={48}
          alt="Cursus Logo"
          src="/cursus-logo.svg"
          priority
        />
      </Link>
    </>
  );
};

export default Logo;
