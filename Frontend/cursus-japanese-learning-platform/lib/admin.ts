
export const isAdmin = () => {
    if (typeof window === 'undefined') {
        return false;
    }
    const userDataString = localStorage.getItem('userData');
    if (!userDataString) {
        return false;
    }

    const userData = JSON.parse(userDataString);

    return userData.role.includes('Admin');
};