"use client";

import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Checkbox } from "@/components/ui/checkbox";
import { useToast } from "@/components/ui/use-toast";
import { zodResolver } from "@hookform/resolvers/zod";
import { XIcon } from "lucide-react";
import React from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { Package } from "@/types/Package";
import { createPackage } from "@/actions/packages-management";

type ModalCreateProps = {
  isVisible: boolean;
  onClose: () => void;
  onPackageChange: (newPackage: Package) => void;
};

const packageSchema = z.object({
  planType: z
    .string()
    .min(1, { message: "Plan Type is required" })
    .max(50, { message: "Must not exceed 50 characters" }),
  planName: z
    .string()
    .min(1, { message: "Plan Name is required" })
    .max(50, { message: "Must not exceed 50 characters" }),
  period: z
    .string()
    .min(1, { message: "Period is required" }),
  price: z
    .string()
    .min(1, { message: "Price is required" }),
  status: z.boolean().default(false),
});

const ModalCreate: React.FC<ModalCreateProps> = ({
  isVisible,
  onClose,
  onPackageChange,
}) => {
  const form = useForm<z.infer<typeof packageSchema>>({
    resolver: zodResolver(packageSchema),
    defaultValues: {
      planType: "",
      planName: "",
      period: "",
      price: "",
      status: false
    },
  });

  const { handleSubmit, reset, trigger, formState: { errors, isSubmitting, isValid } } = form;
  const { toast } = useToast();

  const onSubmit = async (values: z.infer<typeof packageSchema>) => {
    try {
      const newPackage = await createPackage(values);
      onPackageChange(newPackage);
      reset();
      onClose();
      toast({ description: "Your package has been successfully created." });
    } catch (error: any) {
      console.error("Error creating package:", error);
      toast({
        variant: "destructive",
        title: "Uh oh! Something went wrong.",
        description: String(error.message || "Something went wrong!"),
      });
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
      <div className="bg-white rounded-lg mt-20 w-96 px-8 py-4 z-10 relative flex flex-col">
        <div className="flex justify-between items-center mt-3">
          <h1 className="text-xl font-semibold">Add New Package</h1>
          <button className="text-gray-600 hover:text-gray-900" onClick={onClose} aria-label="Close">
            <XIcon className="h-6 w-6" />
          </button>
        </div>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4 mt-4">
            <FormField
              control={form.control}
              name="planType"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Plan Type</FormLabel>
                  <FormControl>
                    <Input
                      disabled={isSubmitting}
                      {...field}
                      placeholder="Plan Type"
                      onBlur={() => trigger("planType")}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="planName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Plan Name</FormLabel>
                  <FormControl>
                    <Input
                      disabled={isSubmitting}
                      {...field}
                      placeholder="Plan Name"
                      onBlur={() => trigger("planName")}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="period"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Period</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      disabled={isSubmitting}
                      {...field}
                      placeholder="Period"
                      onBlur={() => trigger("period")}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="price"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Price</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      disabled={isSubmitting}
                      {...field}
                      placeholder="Price"
                      onBlur={() => trigger("price")}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="status"
              render={({ field }) => (
                <FormItem className="flex items-center gap-x-2">
                  <FormLabel>Status</FormLabel>
                  <Checkbox
                    checked={field.value}
                    onCheckedChange={(checked: boolean) => field.onChange(checked)}
                  />
                </FormItem>
              )}
            />
            <div className="flex justify-end items-center gap-x-2">
              <Button disabled={!isValid || isSubmitting} type="submit">
                Save
              </Button>
              <Button variant="outline" onClick={onClose} className="mx-1">
                Cancel
              </Button>
            </div>
          </form>
        </Form>
      </div>
    </div>
  );
};

export default ModalCreate;
