import api from "@/lib/axios";

const getUserById = async (id: string) => {
  try {
    const response = await api.get("/api/user/" + id);
    return response.data; 
  } catch (error: any) {
    console.log("[ACTIONS_GET_USER_BY_ID]", error);
    return true; 
  }
};

const uploadAvatar = async (file: File, userId: string) => {
  try {
    const formData = new FormData();
    formData.append("imageUploadl", file);  

    const response = await api.put(`/api/user/upload-image/${userId}`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });

    return response.data;
  } catch (error: any) {
    console.log("[ACTIONS_UPLOAD_AVATAR]", error);
    return false;  
  }
}

const getUserWithImageById = async (id: string) => {
  try {
    const response = await api.get("/api/user/get-user-with-avatar/" + id);
    return response.data;
  } catch (error: any) {
    console.log("[ACTIONS_GET_USER_WITH_IMAGE_BY_ID]", error);
    return true;
  }
}

const updateUser = async (userId: string, userData : { id: string , fullName: string}) => {
  try {
    const response = await api.put("/api/user/" + userId, userData);
    return response.data;
  } catch (error: any) {
    console.log("[ACTIONS_UPDATE_USER]", error);
    return false;
  }
}

const getAllUser = async () => {
  try {
    const response = await api.get("/api/user");
    return response.data;
  } catch (error: any) {
    console.log("[ACTIONS_GET_ALL_USER]", error);
    return true;
  }
}

const updateUserAdmin = async (id: string, isDelete: boolean, isActive: boolean) => {
  try {
    const response = await api.put(`/api/user/admin/${id}`, {
      params: {
        isDelete: isDelete,
        isActive: isActive
      },
    });
    return response.data;
  } catch (error: any) {
    console.log("[ACTIONS_UPDATE_USER_ADMIN]", error);
    return true;
  }
};


export { getUserById, uploadAvatar, getUserWithImageById, updateUser, getAllUser, updateUserAdmin };