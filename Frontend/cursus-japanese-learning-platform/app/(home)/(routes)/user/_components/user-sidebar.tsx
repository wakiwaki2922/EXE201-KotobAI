"use client"

import { usePathname } from "next/navigation";


const UserSideBar = () => {
    const currentPath = usePathname();

    const getLinkClasses = (path: string) => {
        const baseClasses = "inline-flex items-center whitespace-nowrap rounded-md text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50 hover:text-accent-foreground h-9 px-4 py-2 justify-start";
        const activeClasses = "bg-muted hover:bg-muted";
        const inactiveClasses = "hover:bg-transparent hover:underline";

        return currentPath === path ? `${baseClasses} ${activeClasses}` : `${baseClasses} ${inactiveClasses}`;
    };

    return (
        <nav className="flex flex-wrap space-x-0 lg:flex-col lg:space-x-0 lg:space-y-1">
            <a
                href="/user/profile"
                className={getLinkClasses('/user/profile')}
            >
                Profile
            </a>
            <a
                href="/user/profile/avatar"
                className={getLinkClasses('/user/profile/avatar')}
            >
                Upload Avatar
            </a>
            <a
                href="/user/profile/password"
                className={getLinkClasses('/user/profile/password')}
            >
                Edit Password
            </a>
        </nav>
    );
}

export default UserSideBar;
