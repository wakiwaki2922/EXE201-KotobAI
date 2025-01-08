"use client";

import { Button } from "@/components/ui/button";
import { ImageIcon, Pencil, PlusCircle } from "lucide-react";
import { useState } from "react";
import toast, { Toaster } from "react-hot-toast";
import Image from "next/image";
import FileUpload from "@/components/file-upload";
import { getFullUserDetails } from "@/actions/authenticaton";
import { uploadAvatar } from "@/actions/user-management";


interface ImageFormProps {
  initialData?: string | "";
  userId?: string | "";
  onAvatarUpdated: () => void;
}

const ImageForm = ({
  initialData,
  userId,
  onAvatarUpdated,
}: ImageFormProps) => {
  const [isEditing, setIsEditing] = useState(false);

  const toggleEdit = () => setIsEditing((current) => !current);

  const uploadImage = async (userId: string, file: File) => {
    try {
      console.log("Uploading image", userId, file);
      await uploadAvatar(file, userId);
      const userData = await getFullUserDetails(userId);
      localStorage.setItem("userData", JSON.stringify(userData.data));
      toast.success("Image uploaded successfully");
      toggleEdit();
      onAvatarUpdated();
    } catch (error: any) {
      toast.error(error.message || "Failed to upload image");
    }
  };

  return (
    <>
      <Toaster />
      <div className="mt-6 border bg-slate-100 rounded-md p-4">
        <div className="font-medium flex items-center justify-between">
          Avatar Image
          <Button onClick={toggleEdit} variant="ghost">
            {isEditing && <>Cancel</>}
            {!isEditing && !initialData && (
              <>
                <PlusCircle className="h-4 w-4 mr-2" />
                Add image
              </>
            )}
            {!isEditing && initialData && (
              <>
                <Pencil className="h-4 w-4 mr-2" />
                Edit image
              </>
            )}
          </Button>
        </div>
        {!isEditing &&
          (!initialData ? (
            <div className="flex items-center justify-center h-60 bg-slate-200 rounded-md">
              <ImageIcon className="h-10 w-10 text-slate-500" />
            </div>
          ) : (
            <div className="relative aspect-square mt-2">
              <Image
                alt="Upload"
                fill
                className="object-cover rounded-md"
                src={initialData}
                sizes="(max-width: 640px) 100vw, 50vw"
                priority
              />
            </div>
          ))}
        {isEditing && (
          <div>
            <FileUpload
              className="p-10 my-4 border border-muted-foreground rounded-md"
              id={userId || ""}
              onUpload={uploadImage}
            />
            <div className="text-xs text-muted-foreground mt-4">
              4:4 aspect ratio recommended
            </div>
          </div>
        )}
      </div>
    </>
  );
};

export default ImageForm;
