"use client";

import React, { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import toast, { Toaster } from "react-hot-toast";
import {
  Form,
  FormField,
  FormControl,
  FormLabel,
  FormMessage,
  FormItem,
} from "@/components/ui/form";
import { useRouter } from "next/navigation";
import UserSideBar from "../../_components/user-sidebar";
import Loader from "@/app/components/Loader";
import FormHeader from "../../_components/form-header";
import { resetPassword } from "@/actions/authenticaton";
import Cookies from "js-cookie";

const formSchema = z
  .object({
    newPassword: z
      .string()
      .min(8, { message: "New password must be at least 8 characters" }),
    confirmPassword: z
      .string()
      .min(8, { message: "Confirm password must be at least 8 characters" }),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
  });

type FormSchemaType = z.infer<typeof formSchema>;

const EditPassword = () => {
  const [loading, setLoading] = useState(true);
  const form = useForm<FormSchemaType>({
    resolver: zodResolver(formSchema),
    mode: "onChange",
  });
  const router = useRouter();

  useEffect(() => {
    // Simulate data fetching or any async operation
    const fetchData = async () => {
      try {
        // Simulate a delay
        await new Promise((resolve) => setTimeout(resolve, 1000));
      } catch (error) {
        console.error("Failed to fetch data:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const {
    handleSubmit,
    formState: { errors, isSubmitting, isValid },
  } = form;

  const onSubmit = async (data: FormSchemaType) => {
    try {
      const token = Cookies.get("jwtToken");
      const { newPassword } = data;
      // await axios.patch('/api/user/profile', { currentPassword, newPassword });
      if (token) {
        await resetPassword({ password: newPassword, verificationToken: token });
        toast.success("Password updated successfully");
        router.refresh();
      } else {
        throw new Error("Token is missing");
      }
      toast.success("Password updated successfully");
      router.refresh();
    } catch (error: any) {
      console.error("Failed to update password:", error);
      toast.error(error.message || "Failed to update password");
    } finally {
      form.reset();
    }
  };

  if (loading) {
    return (
      <>
        <div className="flex flex-col items-center justify-center h-screen">
          <Loader />
        </div>
      </>
    );
  }

  return (
    <>
      <div className="p-6">
        <Toaster />
        <div className="rounded-[0.5rem] border bg-background shadow">
          <div className="space-y-6 p-10 pb-16">
            <FormHeader />
            <div className="shrink-0 bg-border h-[1px] w-full my-6"></div>
            <div className="flex flex-col space-y-8 lg:flex-row lg:space-x-12 lg:space-y-0">
              <aside className="-mx-4 lg:w-1/5">
                <UserSideBar />
              </aside>
              <div className="flex-1 lg:max-w-2xl">
                <div className="space-y-6">
                  <div>
                    <h3 className="text-lg font-medium">Edit Password</h3>
                    <p className="text-sm text-muted-foreground">
                      Change your account password.
                    </p>
                  </div>
                  <div className="shrink-0 bg-border h-[1px] w-full my-6"></div>
                  <Form {...form}>
                    <form
                      onSubmit={handleSubmit(onSubmit)}
                      className="space-y-4 mt-8"
                    >
                      <FormField
                        control={form.control}
                        name="newPassword"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>New Password</FormLabel>
                            <FormControl>
                              <Input
                                type="password"
                                placeholder="New Password"
                                {...field}
                              />
                            </FormControl>
                            {errors.newPassword && (
                              <FormMessage>
                                {errors.newPassword.message}
                              </FormMessage>
                            )}
                          </FormItem>
                        )}
                      />
                      <FormField
                        control={form.control}
                        name="confirmPassword"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Confirm Password</FormLabel>
                            <FormControl>
                              <Input
                                type="password"
                                placeholder="Confirm Password"
                                {...field}
                              />
                            </FormControl>
                            {errors.confirmPassword && (
                              <FormMessage>
                                {errors.confirmPassword.message}
                              </FormMessage>
                            )}
                          </FormItem>
                        )}
                      />
                      <div className="flex items-center gap-x-2 justify-end">
                        <Button
                          type="submit"
                          disabled={!isValid || isSubmitting}
                        >
                          Update Password
                        </Button>
                      </div>
                    </form>
                  </Form>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default EditPassword;
