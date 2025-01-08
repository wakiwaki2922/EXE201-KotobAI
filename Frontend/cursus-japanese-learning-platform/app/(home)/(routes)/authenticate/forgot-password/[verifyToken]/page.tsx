"use client"

import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "@/components/ui/button";
import { Carousel, CarouselContent, CarouselItem } from "@/components/ui/carousel";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import Autoplay from "embla-carousel-autoplay"
import { useForm } from "react-hook-form";
import { redirect, useParams, useRouter } from "next/navigation";
import { resetPassword } from "@/actions/authenticaton";

const formSchema = z.object({
    password: z.string().min(6, { message: 'New password must be at least 6 characters' }),
    confirmPassword: z.string().min(6, { message: 'Confirm password must be at least 6 characters' }),
}).refine(data => data.password === data.confirmPassword, {
    message: "Passwords don't match",
    path: ['confirmPassword'],
});

type FormSchemaType = z.infer<typeof formSchema>;

const ResetPassword = () => {

    const router = useRouter();
    const params = useParams();
    const verifyToken = params.verifyToken as string;
    
    // const url = window.location.href;

    // token = url.includes("/verify/") ? url.split("/verify/")[1] : undefined;

    // if (token) {
    // console.log("Token exists:", token);
    // } else {
    //     console.log("Token does not exist in URL");
    // }
    
    const form = useForm<FormSchemaType>({ resolver: zodResolver(formSchema), mode: 'onChange' });
    
    if (!verifyToken) {
        return redirect('/authenticate/forgot-password');
    }

    const { handleSubmit, formState: { errors, isSubmitting, isValid } } = form;

    const onSubmit = async (values: z.infer<typeof formSchema>) => {
        console.log(values, verifyToken)
        // Call the API to reset the password
        await resetPassword({ password: values.password, verificationToken: verifyToken });
        // Redirect to login page
        router.push('/authenticate/login');
    }

    return (
        <div className="container relative h-screen flex-col items-center justify-center md:grid lg:max-w-none lg:grid-cols-2 lg:px-0">
            <a
                href="/authenticate/login"
                className="inline-flex items-center justify-center whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-1
                    focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50 hover:bg-accent hover:text-accent-foreground h-9 px-4 py-2 absolute right-4 top-4 md:right-8 md:top-8"
            >
                Login
            </a>
            <div className="relative hidden h-full flex-col bg-muted text-white lg:flex dark:border-r">
                <Carousel
                    plugins={[
                        Autoplay({
                            delay: 6000,
                        })
                    ]}
                >
                    <CarouselContent>
                        <CarouselItem>
                        </CarouselItem>
                        <CarouselItem></CarouselItem>
                    </CarouselContent>
                </Carousel>
            </div>
            <div className="lg:p-8 pt-20">
                <h1 className="text-2xl pt-9">
                    Reset Password
                </h1>
                <p className="text-sm text-muted-foreground">Enter your new password.</p>
                <Form {...form}>
                    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 mt-8">
                        <FormField
                            control={form.control}
                            name="password"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Password</FormLabel>
                                    <FormControl>
                                        <Input
                                            type="password"
                                            placeholder="Password"
                                            {...field}
                                        />
                                    </FormControl>
                                    {errors.confirmPassword && <FormMessage>{errors.confirmPassword.message}</FormMessage>}
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
                                    {errors.confirmPassword && <FormMessage>{errors.confirmPassword.message}</FormMessage>}
                                </FormItem>
                            )}
                        />
                        <Button
                            type="submit"
                            disabled={!isValid || isSubmitting}
                            className="w-full"
                        >
                            {isSubmitting ? "Submitting..." : "Reset Password"}
                        </Button>
                    </form>
                </Form>
            </div>
        </div>
    );
}

export default ResetPassword;