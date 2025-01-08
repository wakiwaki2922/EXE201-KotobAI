import api from "@/lib/axios";

const getAllMessagesByChatId = async (chatId: string) => {
    try {
        const endpoint = `/api/messages/chat/${chatId}`;
        const response = await api.get(endpoint);
        return response.data;
    } catch (error: any) {
        console.error("[ACTIONS_GET_ALL_MESSAGES_BY_CHAT_ID]", error);
        if (error.response && error.response.data) {
        throw new Error(error.response.data.message || "Something went wrong!");
        } else {
        throw new Error(error.message || "Something went wrong!");
        }
    }
}

const createMessages = async (chatId: string, messageContent: string, senderType: string) => {
    try {
        const endpoint = `/api/messages/`;
        const response = await api.post(endpoint, { chatId, messageContent, senderType });
        return response.data;
    } catch (error: any) {
        console.error("[ACTIONS_CREATE_MESSAGES]", error);
        if (error.response && error.response.data) {
        throw new Error(error.response.data.message || "Something went wrong!");
        } else {
        throw new Error(error.message || "Something went wrong!");
        }
    }
}

const deleteMessagesByIdAndUserNames = async (messageId: string, userName: string) => {
    try {
        const endpoint = `/api/messages/${messageId}/user/${userName}`;
        const response = await api.delete(endpoint, { data: { userName } });
        return response.data;
    } catch (error: any) {
        console.error("[ACTIONS_DELETE_MESSAGES_BY_ID_AND_USER_NAMES]", error);
        if (error.response && error.response.data) {
        throw new Error(error.response.data.message || "Something went wrong!");
        } else {
        throw new Error(error.message || "Something went wrong!");
        }
    }
}

export { getAllMessagesByChatId, createMessages, deleteMessagesByIdAndUserNames };