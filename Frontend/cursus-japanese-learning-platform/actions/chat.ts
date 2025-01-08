import api from "@/lib/axios";

const createChatForUser = async (userId: string) => {
    try {
        const endpoint = `/api/chats/user/${userId}`;
        const response = await api.post(endpoint);
        console.log("Create chat for user", response.data);
        return response.data;
    } catch (error: any) {
        console.error("[ACTIONS_CREATE_CHAT_FOR_USER]", error);
        if (error.response && error.response.data) {
        throw new Error(error.response.data.message || "Something went wrong!");
        } else {
        throw new Error(error.message || "Something went wrong!");
        }
    }
}

const getAllChat = async () => {
    try {
        const endpoint = `/api/chats/`;
        const response = await api.get(endpoint);
        return response.data;
    } catch (error: any) {
        console.error("[ACTIONS_GET_ALL_CHAT]", error);
        if (error.response && error.response.data) {
        throw new Error(error.response.data.message || "Something went wrong!");
        } else {
        throw new Error(error.message || "Something went wrong!");
        }
    }
}

const getChatById = async (chatId: string) => {
    try {
        const endpoint = `/api/chats/${chatId}`;
        const response = await api.get(endpoint);
        console.log("getChatByUserId response:", response.data);
        return response.data;
    } catch (error: any) {
        console.error("[ACTIONS_GET_CHAT_BY_ID]", error);
        if (error.response && error.response.data) {
        throw new Error(error.response.data.message || "Something went wrong!");
        } else {
        throw new Error(error.message || "Something went wrong!");
        }
    }
}

const getChatByUserId = async (userId: string) => {
    try {
        const endpoint = `/api/chats/user/${userId}`;
        const response = await api.get(endpoint);
        return response.data;
    } catch (error: any) {
        console.error("[ACTIONS_GET_CHAT_BY_USER_ID]", error);
        if (error.response && error.response.data) {
        throw new Error(error.response.data.message || "Something went wrong!");
        } else {
        throw new Error(error.message || "Something went wrong!");
        }
    }
}

const deleteChatByUserID = async (chatId: string ,userId: string) => {
    try {
        const endpoint = `/api/chats/${chatId}/user/${userId}`;
        const response = await api.delete(endpoint);
        return response.data;
    } catch (error: any) {
        console.error("[ACTIONS_DELETE_CHAT_BY_USER_ID]", error);
        if (error.response && error.response.data) {
        throw new Error(error.response.data.message || "Something went wrong!");
        } else {
        throw new Error(error.message || "Something went wrong!");
        }
    }
}

const checkIfChatExist = async (chatId: string) => {
    try {
        const endpoint = `/api/chats/${chatId}/exists`;
        const response = await api.get(endpoint);
        return response.data;
    } catch (error: any) {
        console.error("[ACTIONS_CHECK_IF_CHAT_EXIST]", error);
        if (error.response && error.response.data) {
        throw new Error(error.response.data.message || "Something went wrong!");
        } else {
        throw new Error(error.message || "Something went wrong!");
        }
    }
}

export { createChatForUser, getAllChat, getChatById, getChatByUserId, deleteChatByUserID, checkIfChatExist };