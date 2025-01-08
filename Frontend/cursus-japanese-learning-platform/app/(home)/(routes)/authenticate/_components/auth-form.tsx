"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { FcGoogle } from "react-icons/fc";

const formSchema = z.object({
    email: z.string().min(1, {
        message: "Email is required",
    }).email("Invalid email address")
      .regex(/^\S+$/, {
        message: "Email should not contain spaces or tabs",
    }),
    password: z.string().min(8, {
        message: "Password must be at least 8 characters",
    }).regex(/[A-Z]/, {
        message: "Password must contain at least one uppercase letter",
    }).regex(/[a-z]/, {
        message: "Password must contain at least one lowercase letter",
    }).regex(/\d/, {
        message: "Password must contain at least one number",
    }).regex(/[!@#$%^&*(),.?":{}|<>]/, {
        message: "Password must contain at least one special character",
    }).regex(/^\S+$/, {
        message: "Password should not contain spaces or tabs",
    }),
    fullName: z.string().min(1, {
        message: "Full name is required",
    }).max(50, {
        message: "Full name can have a maximum of 50 characters",
    }).optional(),
});

type FormSchemaType = z.infer<typeof formSchema>;

interface AuthFormProps {
    type: "login" | "signup";
    onSubmit: (values: FormSchemaType) => void;
    handleGoogleSignIn: () => void;
}

const AuthForm: React.FC<AuthFormProps> = ({ type, onSubmit, handleGoogleSignIn }) => {
    const form = useForm<FormSchemaType>({
        resolver: zodResolver(formSchema),
        mode: "onChange",
        defaultValues: {
            email: "",
            password: "",
            fullName: type === "signup" ? "" : undefined,
        },
    });

    const { handleSubmit, trigger, formState: { errors, isSubmitting, isValid } } = form;

    return (
        <>
            <Form {...form}>
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 mt-8">
                    <FormField
                        control={form.control}
                        name="email"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Email</FormLabel>
                                <FormControl>
                                    <Input
                                        disabled={isSubmitting}
                                        placeholder="e.g. 'name@example.com'"
                                        {...field}
                                        onBlur={() => trigger("email")}
                                    />
                                </FormControl>
                                {errors.email && (
                                    <FormMessage>{errors.email.message}</FormMessage>
                                )}
                            </FormItem>
                        )}
                    />
                    <FormField
                        control={form.control}
                        name="password"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Password</FormLabel>
                                <FormControl>
                                    <Input
                                        type="password"
                                        disabled={isSubmitting}
                                        placeholder="******"
                                        {...field}
                                        onBlur={() => trigger("password")}
                                    />
                                </FormControl>
                                {errors.password && (
                                    <FormMessage>{errors.password.message}</FormMessage>
                                )}
                            </FormItem>
                        )}
                    />
                    {type === "signup" && (
                        <>
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
                                        {errors.fullName && (
                                            <FormMessage>{errors.fullName.message}</FormMessage>
                                        )}
                                    </FormItem>
                                )}
                            />                         
                        </>
                    )}
                    <p className="text-left text-sm text-muted-foreground">
                        <a href="/authenticate/forgot-password" className="underline underline-offset-4 hover:text-primary">
                            Forgot password?
                        </a>
                    </p>
                    <div className="flex items-center gap-x-2 justify-center">
                        <Button className="w-full" type="submit" disabled={!isValid || isSubmitting}>
                            {type === "login" ? "Login" : "Sign Up"}
                        </Button>
                    </div>
                </form>
                <div className="relative mt-4">
                    <div className="absolute inset-0 flex items-center">
                        <div className="w-full border-t border-gray-300"></div>
                    </div>
                    <div className="relative flex justify-center text-sm">
                        <span className="px-2 bg-white text-gray-500 text-xs">OR CONTINUE WITH</span>
                    </div>
                </div>
                <Button
                    className="mt-4 w-full bg-white text-black border hover:bg-accent hover:text-accent-foreground"
                    onClick={handleGoogleSignIn}
                >
                    <FcGoogle className="mr-2" />
                    Google
                </Button>
                <p className="px-8 pt-4 text-center text-sm text-muted-foreground">
                    By clicking continue, you agree to our <a href="" className="underline underline-offset-4 hover:text-primary"> Term of Service</a> and <a href="" className="underline underline-offset-4 hover:text-primary">Privacy Policy</a>
                </p>
            </Form>
        </>
    );
};

export default AuthForm;