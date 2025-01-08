"use client";

import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import UserSideBar from "../_components/user-sidebar";
import { useForm } from "react-hook-form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useEffect, useState } from "react";
import toast, { Toaster } from "react-hot-toast";
import { useRouter } from "next/navigation";
import Loader from "@/app/components/Loader";
import FormHeader from "../_components/form-header";
import { getUserWithImageById, updateUser } from "@/actions/user-management";
import { UserData } from "@/types/UserData";

const formSchema = z.object({
  email: z
    .string()
    .min(1, {
      message: "Email is required",
    })
    .email("Invalid email address"),
  fullName: z
    .string()
    .min(1, {
      message: "Full name is required",
    })
    .max(50, {
      message: "Full name can have a maximum of 50 characters",
    })
    .optional(),
  avatarImageUrl: z
  .union([
    z.string(),
    z.null(),
  ])
  .optional(),
});

type FormSchemaType = z.infer<typeof formSchema>;

const UserProfile = () => {
  const [user, setUser] = useState<FormSchemaType | null>(null);

  const form = useForm<FormSchemaType>({
    resolver: zodResolver(formSchema),
    mode: "onChange",
    defaultValues: user || undefined,
  });

  const router = useRouter();

  const {
    handleSubmit,
    trigger,
    formState: { errors, isSubmitting, isValid },
    reset,
    watch,
  } = form;

  const [isChanged, setIsChanged] = useState(false);

  const watchedFields = watch();

  const [userData, setUserData] = useState<UserData>();

  useEffect(() => {
    const fetchStoredUserData = async () => {
      const storedUserData = await localStorage.getItem("userData");
      if (storedUserData) {
        setUserData(JSON.parse(storedUserData));
      }
    };
    fetchStoredUserData();
  }, []);

  useEffect(() => {
    setIsChanged(
      Object.keys(watchedFields).some((key) => {
        const typedKey = key as keyof FormSchemaType;
        console.log(typedKey, watchedFields[typedKey], user?.[typedKey]);
        return watchedFields[typedKey] !== user?.[typedKey];
      })
    );
  }, [watchedFields, user]);

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        // const { data } = await axios.get("/api/user/profile");
        if (userData?.id) {
          const data = await getUserWithImageById(userData.id);
          const mappedData = {
            email: data.data.emailAddress,
            fullName: data.data.fullName,
            avatarImageUrl: data.data.imagePath,
          };
          const parsedUser = formSchema.parse(mappedData);
          console.log(mappedData);
          setUser(parsedUser);
          reset(parsedUser);
        } else {
          console.error("User data is undefined");
        }
      } catch (error) {
        console.error("Failed to fetch user profile:", error);
      }
    };
    fetchUserData();
  }, [reset, userData?.id]);

  const onSubmit = async (data: FormSchemaType) => {
    try {
      // await axios.put("/api/user/profile", data);
      if (userData?.id) {
        await updateUser(userData.id, { id: userData.id, fullName: data.fullName || "" });
      } else {
        console.error("User ID is undefined");
      }
      console.log("Profile updated successfully");
      toast.success("Profile updated successfully");
      router.refresh();
    } catch (error) {
      console.error("Failed to update profile:", error);
    }
  };

  if (!user) {
    return (
      <>
        <div className="flex flex-col items-center justify-center h-screen">
          <Loader />
        </div>
      </>
    );
  }
  console.log("Button: ", isValid, isSubmitting, isChanged);
  console.log("Disabled: ", isSubmitting || !isValid);

  return (
    <>
      <Toaster />
      <div className="p-6">
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
                    <h3 className="text-lg font-medium">Profile</h3>
                    <p className="text-sm text-muted-foreground">
                      This is how others will see you on site.
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
                        name="email"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Email</FormLabel>
                            <FormControl>
                              <Input
                                disabled
                                placeholder="e.g. 'name@example.com'"
                                {...field}
                                onBlur={() => trigger("email")}
                              />
                            </FormControl>
                            <FormDescription className="text-[0.8rem] text-muted-foreground">
                              You cannot change your email address.
                            </FormDescription>
                            {errors.email && (
                              <FormMessage>{errors.email.message}</FormMessage>
                            )}
                          </FormItem>
                        )}
                      />
                      <FormField
                        control={form.control}
                        name="fullName"
                        render={({ field }) => (
                          <FormItem>
                            <FormLabel>Full Name</FormLabel>
                            <FormControl>
                              <Input
                                disabled={isSubmitting}
                                placeholder="e.g. 'John Doe'"
                                {...field}
                                onBlur={() => trigger("fullName")}
                              />
                            </FormControl>
                            <FormDescription className="text-[0.8rem] text-muted-foreground">
                              This is your public display name and will on your
                              certificate. It should be your real name.
                            </FormDescription>
                            {errors.fullName && (
                              <FormMessage>
                                {errors.fullName.message}
                              </FormMessage>
                            )}
                          </FormItem>
                        )}
                      />
                      <div className="flex items-center gap-x-2 justify-end">
                        <Button
                          type="submit"
                          disabled={isSubmitting || !isValid || !isChanged}
                        >
                          Update profile
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

export default UserProfile;
