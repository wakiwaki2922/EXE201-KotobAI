import { ReactNode } from "react";
import { Toaster } from "react-hot-toast";

const AuthenticatePage = ({
    children
}: {
    children: ReactNode
}) => {
    
    return (
        <>
            <Toaster />
            {children}
        </>
    );
}

export default AuthenticatePage;