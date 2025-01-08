"use client";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { User } from "@/types/User";
import { ColumnDef } from "@tanstack/react-table";

import { ArrowUpDown, Eye, MoreHorizontal, Pencil } from "lucide-react";
import Link from "next/link";

export const columns: ColumnDef<User>[] = [
  {
    accessorKey: "fullName",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Student Name
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "emailAddress",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Email
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
    },
  },
  {
    accessorKey: "createdTime",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Verify
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      );
      
    },
    cell: ({ row }) => {
      const createdTime = new Date(row.original.createdTime);
      return createdTime.toLocaleDateString("en-GB"); // Formats as dd-MM-yyyy
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
    accessorKey: "isActive",
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
      const status = row.original.isActive;
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
    id: "actions",
    cell: ({ row }) => {
      const { id } = row.original;

      return (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="h-4 w-8 p-0">
              <span className="sr-only">Open menu</span>
              <MoreHorizontal className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <Link href={`/admin/user/${id}`}>
              <DropdownMenuItem>
                <Eye className="h-4 w-4 mr-2" />
                View
              </DropdownMenuItem>
            </Link>
          </DropdownMenuContent>
        </DropdownMenu>
      );
    },
  },
];
