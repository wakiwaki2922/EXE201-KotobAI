"use client"

import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "@/components/ui/button";
import { Carousel, CarouselContent, CarouselItem } from "@/components/ui/carousel";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import Autoplay from "embla-carousel-autoplay"
import { useForm } from "react-hook-form";
import { forgotPassword } from "@/actions/authenticaton";

const formSchema = z.object({
    email: z.string().min(1, {
        message: "Email is required",
    }).email("Invalid email address"),
});

type FormSchemaType = z.infer<typeof formSchema>;

const ForgotPasswordPage = () => {

    const form = useForm<FormSchemaType>({
        resolver: zodResolver(formSchema),
        mode: "onChange",
        defaultValues: {
            email: "",
        },
    });

    const { handleSubmit, trigger, formState: { errors, isSubmitting, isValid } } = form;

    const onSubmit = async (values: z.infer<typeof formSchema>) => {
        forgotPassword(values.email);
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
                        <CarouselItem></CarouselItem>
                        <CarouselItem></CarouselItem>
                    </CarouselContent>
                </Carousel>
            </div>
            <div className="lg:p-8 pt-20">
                <h1 className="text-2xl pt-9">
                    Forgot Password
                </h1>
                <p className="text-sm text-muted-foreground">We’ll email you a link so you can reset your password.</p>
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
                                            placeholder="Email address"
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
                        <div className="flex items-center gap-x-2 justify-center">
                            <Button className="w-full" type="submit" disabled={!isValid || isSubmitting}>
                                Send Reset Link
                            </Button>
                        </div>
                    </form>
                </Form>
            </div>
        </div>
    );
}

export default ForgotPasswordPage;