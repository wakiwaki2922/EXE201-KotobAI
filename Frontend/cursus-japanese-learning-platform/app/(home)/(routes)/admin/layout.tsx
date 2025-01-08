"use client"

import { isAdmin } from "@/lib/admin";
import { useRouter } from "next/navigation";
import { ReactNode, useEffect } from "react";

const AdminPage = ({
    children
}: {
    children: ReactNode
}) => {

    const router = useRouter();

    useEffect(() => {
        if (!isAdmin()) {
            router.push('/authenticate/login');
        }
    }, [router]);

    return (
        <>
            {children}
        </>
    );
}

export default AdminPage;