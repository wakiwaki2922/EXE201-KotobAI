"use client";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Package } from "@/types/Package";
import { ColumnDef } from "@tanstack/react-table";

import { ArrowUpDown, MoreHorizontal, Pencil } from "lucide-react";
import { Fragment, useState } from "react";
import ModalDelete from "./modal-delete";
import ModalEdit from "./modal-edit";
import DropdownMenuActions from "./dropdown-menu";
import { Emoji } from "emoji-picker-react";

interface ColumnProps {
  onPackageChange: (newPackage: Package) => void;
}

export const columns = ({
  onPackageChange,
}: ColumnProps): ColumnDef<Package>[] => [
  {
    accessorKey: "planType",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Plan Type
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "planName",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Plan Name
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "period",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Period
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "price",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Price
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
    cell: ({ row }) => {
      const price = row.original.price;
      return `$${price.toFixed(2)}`; // Formats price with a dollar sign and two decimal places
    },
  },
  {
    accessorKey: "status",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Status
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
    cell: ({ row }) => {
      const status = row.original.status;
      return (
        <span
  className={`inline-flex items-center justify-center rounded-full px-3 py-1 text-sm font-medium ${
    status ? "bg-blue-500 text-white" : "bg-red-500 text-white"
      }`}
>
  {status ? "Active" : "Inactive"}
</span>
      ); // Displaying "Active" or "Inactive" with a green background
    },
  },  
  {
    accessorKey: "isDelete",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Delete
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
    cell: ({ row }) => {
      const isDelete = row.original.isDelete;
      return (
        <span
          className={`inline-flex items-center justify-center rounded-full px-3 py-1 text-sm font-medium ${
            isDelete ? "bg-red-500 text-white" :"bg-green-500 text-white"
      }`}
>
        {isDelete ? "Yes" : "No"}
</span>
      ); // Displaying "Active" or "Inactive" with a green background
    },
  },
  {
    accessorKey: "lastUpdatedTime",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Last Updated
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
    cell: ({ row }) => {
      const lastUpdatedTime = new Date(row.original.lastUpdatedTime);
      return lastUpdatedTime.toLocaleDateString("en-GB"); // Formats as dd-MM-yyyy
    },
  },  
  {
    id: "actions",
    cell: ({ row }) => {
      const { id } = row.original;
      return (
        <DropdownMenuActions
          id={id}
          onPackageChange={onPackageChange}
        />
      );
    },
  },
];
