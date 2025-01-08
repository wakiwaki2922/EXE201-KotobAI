import api from "@/lib/axios";

// Lấy danh sách các gói (active packages)
const getActivePackages = async () => {
    try {
        const endpoint = `/api/packages`; // Lấy danh sách gói active
        const response = await api.get(endpoint);
        return response.data;
    } catch (error: any) {
        console.log("[ACTIONS_GET_ACTIVE_PACKAGES]", error);
        if (error.response && error.response.data) {
            throw new Error(error.response.data.message || "Something went wrong!");
        } else {
            throw new Error(error.message || "Something went wrong!");
        }
    }
};

// Lấy thông tin chi tiết một gói theo ID
const getPackageById = async (packageId: string) => {
    try {
        const endpoint = `/api/packages/${packageId}`; // Lấy thông tin chi tiết gói
        const response = await api.get(endpoint);
        return response.data;
    } catch (error: any) {
        console.log("[ACTIONS_GET_PACKAGE_BY_ID]", error);
        if (error.response && error.response.data) {
            throw new Error(error.response.data.message || "Something went wrong!");
        } else {
            throw new Error(error.message || "Something went wrong!");
        }
    }
};

// Tạo mới một gói
const createPackage = async (packageData: any) => {
    try {
        const endpoint = `/api/packages`; // Tạo mới gói
        const response = await api.post(endpoint, packageData);
        return response.data;
    } catch (error: any) {
        console.log("[ACTIONS_CREATE_PACKAGE]", error);
        if (error.response && error.response.data) {
            throw new Error(error.response.data.message || "Something went wrong!");
        } else {
            throw new Error(error.message || "Something went wrong!");
        }
    }
};

// Cập nhật thông tin một gói
const updatePackage = async (packageId: string, packageData: any) => {
    try {
        const endpoint = `/api/packages/${packageId}`; // Cập nhật gói theo ID
        const response = await api.put(endpoint, packageData);
        return response.data;
    } catch (error: any) {
        console.log("[ACTIONS_UPDATE_PACKAGE]", error);
        if (error.response && error.response.data) {
            throw new Error(error.response.data.message || "Something went wrong!");
        } else {
            throw new Error(error.message || "Something went wrong!");
        }
    }
};

// Xóa một gói theo ID
const deletePackage = async (packageId: string) => {
    try {
        const endpoint = `/api/packages/${packageId}`; // Xóa gói theo ID
        const response = await api.delete(endpoint);
        return response.data;
    } catch (error: any) {
        console.log("[ACTIONS_DELETE_PACKAGE]", error);
        if (error.response && error.response.data) {
            throw new Error(error.response.data.message || "Something went wrong!");
        } else {
            throw new Error(error.message || "Something went wrong!");
        }
    }
};

// Lấy tất cả các gói (admin only)
const getAllPackages = async () => {
    try {
        const endpoint = `/api/packages/getAll`; // Lấy tất cả các gói
        const response = await api.get(endpoint);
        return response.data;
    } catch (error: any) {
        console.log("[ACTIONS_GET_ALL_PACKAGES]", error);
        if (error.response && error.response.data) {
            throw new Error(error.response.data.message || "Something went wrong!");
        } else {
            throw new Error(error.message || "Something went wrong!");
        }
    }
};

export { createPackage, getPackageById, getActivePackages, updatePackage, deletePackage, getAllPackages };
