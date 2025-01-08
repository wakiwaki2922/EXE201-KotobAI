"use client";

import { useEffect, useState } from "react";
import { columns } from "./_components/columns-user";
import { DataTable } from "./_components/data-table-student";
import { useRouter } from "next/navigation";
import { Breadcrumbs } from "@/components/breadcrumbs";
import { Heading } from "@/components/ui/heading";
import { Separator } from "@/components/ui/separator";
import { User } from "@/types/User";
import { getAllUser } from "@/actions/user-management";

const breadcrumbItems = [
  { title: "Dashboard", link: "/admin/dashboard" },
  { title: "User Management", link: "/admin/user" },
];

const UserManagementPage = () => {
  const router = useRouter();

  //TODO: get courses by user for instructor
  const [user, setUsers] = useState<User[]>([]);

  useEffect(() => {
    const fetchStudents = async () => {
      try {
        const data = await getAllUser();
        setUsers(data.data);
      } catch (error) {
        console.error("Error fetching students:", error);
        router.push("/");
      }
    };

    fetchStudents();
  }, [router]);

  return (
    <>
      <div className="p-6">
        <Breadcrumbs items={breadcrumbItems} />
        <Heading
          title={`Users (${user.length})`}
          description="Manage users (Server side table functionalities.)"
        />
        <Separator />
        <DataTable columns={columns} data={user} />
      </div>
    </>
  );
};

export default UserManagementPage;
