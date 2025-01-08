"use client";

import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Pencil, MoreHorizontal } from "lucide-react";
import { Fragment, useState } from "react";
import ModalEdit from "./modal-edit";
import ModalDelete from "./modal-delete";
import { Package } from "@/types/Package";

interface DropdownMenuActionsProps {
  id: string;
  onPackageChange: (newPackage: Package) => void;
}

const DropdownMenuActions: React.FC<DropdownMenuActionsProps> = ({
  id,
  onPackageChange,
}) => {
  const [showModalEdit, setShowModalEdit] = useState(false);

  const [showModalDelete, setShowModalDelete] = useState(false);

  const handlePackageChange = (newPackage: Package) => {
    if (onPackageChange) {
      onPackageChange(newPackage);
    }
  };

  return (
    <div>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant="ghost" className="h-4 w-8 p-0">
            <span className="sr-only">Open menu</span>
            <MoreHorizontal className="h-4 w-4" />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuItem>
            <Fragment>
              <button
                className="flex items-center space-x-2 w-full"
                onClick={() => setShowModalEdit(true)}
              >
                <Pencil className="h-4 w-4 mr-2" />
                Edit
              </button>
            </Fragment>
          </DropdownMenuItem>
          <DropdownMenuItem>
          <Fragment>
              <button
                className="flex items-center space-x-2 w-full"
                onClick={() => setShowModalDelete(true)}
              >
                <Pencil className="h-4 w-4 mr-2" />
                Delete
              </button>
            </Fragment>
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
      <ModalEdit
        isVisible={showModalEdit}
        onClose={() => setShowModalEdit(false)}
        id={id}
        onPackageChange={handlePackageChange}
      />
      <ModalDelete
        isVisible={showModalDelete}
        onClose={() => setShowModalDelete(false)}
        id={id}
        onPackageChange={handlePackageChange}
      />
    </div>
  );
};

export default DropdownMenuActions;
