"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import * as z from "zod";
import toast from "react-hot-toast";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
} from "@/components/ui/carousel";
import Autoplay from "embla-carousel-autoplay";
import {
  // getGoogleRedirectResult,
  signInWithGooglePopup,
  // signInWithGoogleRedirect,
} from "@/lib/firebase";
import { useEffect, useRef, useState } from "react";
import AuthForm from "../_components/auth-form";
import Cookies from "js-cookie";
import { getFullUserDetails, loginWithGoogle, signUp } from "@/actions/authenticaton";

// Define the schema using zod
const formSchema = z.object({
  email: z
    .string()
    .min(1, {
      message: "Email is required",
    })
    .email("Invalid email address")
    .regex(/^\S+$/, {
      message: "Email should not contain spaces or tabs",
    }),
  password: z
    .string()
    .min(8, {
      message: "Password must be at least 8 characters",
    })
    .regex(/[A-Z]/, {
      message: "Password must contain at least one uppercase letter",
    })
    .regex(/[a-z]/, {
      message: "Password must contain at least one lowercase letter",
    })
    .regex(/\d/, {
      message: "Password must contain at least one number",
    })
    .regex(/[!@#$%^&*(),.?":{}|<>]/, {
      message: "Password must contain at least one special character",
    })
    .regex(/^\S+$/, {
      message: "Password should not contain spaces or tabs",
    }),
  fullName: z
    .string()
    .min(1, {
      message: "Full name is required",
    })
    .max(50, {
      message: "Full name can have a maximum of 50 characters",
    })
    .optional(),
});

const SignupPage = () => {
  const router = useRouter();

  const [done, setDone] = useState(false);
  const timerRef = useRef<NodeJS.Timeout | null>(null);
  const [countdown, setCountdown] = useState(5);
  const [isRegistered, setIsRegistered] = useState(false);

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      password: "",
      fullName: "",
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    try {
      console.log(values);
      const response = await signUp({
        emailAddress: values.email,
        password: values.password,
        fullName: values.fullName,
      });
      console.log("Sign up response: ", response);
      Cookies.set("jwtToken", response.data.accessToken, { expires: 1 });
      Cookies.set("refreshToken", response.data.refreshToken, { expires: 7 });
      const userData = await getFullUserDetails(response.data.userId);
      console.log("User Data Sign Up", userData.data);
      localStorage.setItem("userData", JSON.stringify(userData.data));
      toast.success("Your account has been created");
      setDone(true);
      setIsRegistered(true);
    } catch (error: any) {
      toast.error("Something went wrong");
    }
  };

  useEffect(() => {
    if (done == true) {
      timerRef.current = setTimeout(() => {
        router.push("/");
      }, 5000);
      const interval = setInterval(() => {
        setCountdown((prevCountdown) => prevCountdown - 1);
      }, 1000);
      return () => {
        if (timerRef.current) {
          clearTimeout(timerRef.current);
        }
        clearInterval(interval);
      };
    }
  }, [done, router]);

  // const handleGoogleSignIn = async () => {
  //     signInWithGoogleRedirect();
  // };

  // useEffect(() => {
  //     const fetchGoogleSignInResult = async () => {
  //         try {
  //             const result = await getGoogleRedirectResult();
  //             if (result) {
  //                 const user = result.user;
  //                 const token = await user.getIdToken();
  //                 console.log('Google Sign-In Result:', result);
  //                 const response = await axios.post(`/api/user/authenticate`, { token });
  //                 console.log(response);
  //                 Cookies.set('jwtToken', response.data, { expires: 1 });
  //                 const userData = await getFullUserDetails();
  //                 console.log(userData);
  //                 localStorage.setItem('userData', JSON.stringify(response));
  //                 router.push('/');
  //             }
  //         } catch (error) {
  //             console.error('Error fetching Google sign-in result:', error);
  //             toast.error("Fail to login with Google");
  //         }
  //     };
  //     fetchGoogleSignInResult();
  // }, [router]);

  const handleGoogleSignIn = async () => {
    try {
      console.log("Initiating Google sign-in popup...");
      const result = await signInWithGooglePopup();
      const user = result.user;
      const token = await user.getIdToken();
      console.log("Google Sign-In Result:", result);
      const response = await loginWithGoogle(token);
      Cookies.set('jwtToken', response.data.accessToken, { expires: 1 });
      Cookies.set('refreshToken', response.data.refreshToken, { expires: 7 });
      // const userData = {
      //     uid: user.uid,
      //     fullName: user.displayName,
      //     email: user.email,
      //     avatarImageUrl: user.photoURL,
      //     jwtToken: response.data,
      //     providerData: user.providerData,
      //     metadata: user.metadata
      // };
      const userData = await getFullUserDetails(response.data.userId);
      console.log("User Data:", userData.data);
      localStorage.setItem("userData", JSON.stringify(userData.data));
      router.push("/");
    } catch (error) {
      console.error("Error during Google sign-in with popup:", error);
      toast.error("Fail to login with Google");
    }
  };

  return (
    <>
      {!isRegistered ? (
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
                }),
              ]}
            >
              <CarouselContent>
                <CarouselItem></CarouselItem>
                <CarouselItem></CarouselItem>
              </CarouselContent>
            </Carousel>
          </div>
          <div className="lg:p-8">
            <h1 className="text-2xl pt-10">Create Account</h1>
            <div className="text-sm text-slate-600">
              Enter your information into required fields below to create
            </div>
            <AuthForm
              type="signup"
              onSubmit={onSubmit}
              handleGoogleSignIn={handleGoogleSignIn}
            />
          </div>
        </div>
      ) : (
        <div className="fixed inset-0 top-0 left-0 right-0 bottom-0 bg-white flex items-center justify-center">
          <div className="relative flex flex-col">
            <h1 className="text-center text-2xl pt-10">
              Thank you for registering!!
            </h1>
            <p className="text-center text-base text-slate-600">
              Please check your email to verify your account.
            </p>
            <p className="mb-4 text-center text-gray-600">
              You will be redirected to the home page in{" "}
              <span className="font-semibold text-gray-800">{countdown}</span>{" "}
              seconds...
            </p>
          </div>
        </div>
      )}
    </>
  );
};

export default SignupPage;
