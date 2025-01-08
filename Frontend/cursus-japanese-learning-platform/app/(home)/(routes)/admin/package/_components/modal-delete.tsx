"use client";

import { deletePackage } from "@/actions/packages-management";
import { Button } from "@/components/ui/button";
import { useToast } from "@/components/ui/use-toast";
import { Package } from "@/types/Package";
import { XIcon } from "lucide-react";
import React, { useEffect, useState } from "react";

type ModalDeleteProps = {
  id: string;
  isVisible: boolean;
  onClose: () => void;
  onPackageChange: (newPackage: Package) => void;
};

const ModalDelete: React.FC<ModalDeleteProps> = ({
  id,
  isVisible,
  onClose,
  onPackageChange
}) => {

  const [payLoad, setPayLoad] = useState<Package>({
    id: "",
    planType: "",
    planName: "",
    period: 0,
    price: 0,
    status: false,
    isDelete: false,
    createdTime: "",
    lastUpdatedTime: ""
  });

  const { toast } = useToast();
 
  const handleDelete = async () => {
    try {
      const message = await deletePackage(id);
      onPackageChange(payLoad);
      onClose();
      toast({
        description: message,
      })
    } catch (error: any) {
      console.error("Error deleting package:", error);
      toast({
        variant: "destructive",
        title: "Uh oh! Something went wrong.",
        description: String(error.message || "Something went wrong!"),
      })
    }
  };

  if (!isVisible) return null;

  const handleClose = (e: React.MouseEvent<HTMLDivElement, MouseEvent>) => {
    if (e.target instanceof Element && e.target.id === "wrapper") {
      onClose();
    }
  };

  return (
    <div
      className="fixed z-10 top-0 left-0 right-0 bottom-0 inset-0 flex items-center justify-center bg-black bg-opacity-25 backdrop-blur-sm"
      id="wrapper"
      onClick={handleClose}
    >
      <div className="bg-white rounded-lg w-96 p-8 z-10 relative flex flex-col">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold">Delete Package</h2>
          <button
            className="text-gray-600 hover:text-gray-900"
            onClick={onClose}
            aria-label="Close"
          >
            <XIcon className="h-6 w-6" />
          </button>
        </div>
        <div className="my-2 w-full">
            <p className="text-base">Are you sure you want to delete this package?</p>
        </div>
        <div className="flex justify-end mt-2">
          <Button variant="destructive" onClick={handleDelete} className="mx-1">
            Delete
          </Button>
          <Button variant="outline" onClick={onClose} className="mx-1">
            Cancel
          </Button>
        </div>
      </div>
    </div>
  );
};

export default ModalDelete;
