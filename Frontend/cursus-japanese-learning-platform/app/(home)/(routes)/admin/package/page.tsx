"use client";

import { Package } from "@/types/Package";
import { useEffect, useState } from "react";
import { DataTable } from "./_components/data-table-categories";
import { columns } from "./_components/columns-packages";
import { Breadcrumbs } from "@/components/breadcrumbs";
import { Heading } from "@/components/ui/heading";
import { Separator } from "@/components/ui/separator";
import { getAllPackages } from "@/actions/packages-management";

const breadcrumbItems = [
  { title: "Dashboard", link: "/admin/dashboard" },
  { title: "Packages Management", link: "/admin/package" },
];

const PackagesManagement = () => {
  const [packages, setPackages] = useState<Package[]>([]);

  const [newPackage, setPackage] = useState<Package>();

  const handlePackageChange = (newPackage: Package) => {
    setPackage(newPackage);
  };

  const fetchCategories = async () => {
    try {
      const data = await getAllPackages();
      setPackages(data.data);
    } catch (error) {
      console.error("Error fetching packages:", error);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  useEffect(() => {
    if (newPackage != null) {
      fetchCategories();
    }
  }, [newPackage]);

  return (
    <>
      <div className="p-6">
        <Breadcrumbs items={breadcrumbItems} />
        <Heading
          title={`Packages (${packages.length})`}
          description="Manage packages (Server side table functionalities.)"
        /> 
        <Separator />
        <DataTable
          columns={columns({ onPackageChange: handlePackageChange })}
          data={packages}
          onPackageChange={handlePackageChange}
        />
      </div>
    </>
  );
};

export default PackagesManagement;
