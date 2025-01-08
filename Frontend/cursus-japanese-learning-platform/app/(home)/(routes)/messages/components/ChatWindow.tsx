import { createMessages, getAllMessagesByChatId } from "@/actions/messages";
import React, { useState, useEffect, useRef } from "react";

type Message = {
    id: string;
    messageContent: string;
    senderType: string;
};

type ChatWindowProps = {
    chatId: string;
};

const ChatWindow: React.FC<ChatWindowProps> = ({ chatId }) => {
    const [messages, setMessages] = useState<Message[]>([]);
    const [newMessage, setNewMessage] = useState("");
    const [loading, setLoading] = useState(false);
    const messagesEndRef = useRef<HTMLDivElement>(null); // Thêm ref cho cuối danh sách tin nhắn

    useEffect(() => {
        const fetchMessages = async () => {
            try {
                const result = await getAllMessagesByChatId(chatId);
                if (Array.isArray(result.data)) {
                    setMessages(result.data);
                } else {
                    console.error("[ChatWindow] Result data is not an array:", result.data);
                    setMessages([]);
                }
            } catch (error) {
                console.error("[ChatWindow] Error fetching messages", error);
            }
        };

        if (chatId) {
            fetchMessages();
        }
    }, [chatId]);

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [messages]); // Tự động cuộn khi messages thay đổi

    const handleSendMessage = async () => {
        if (newMessage.trim() === "") return;

        setLoading(true);

        const userMessage = {
            id: `user-${Date.now()}`,
            messageContent: newMessage,
            senderType: "User",
        };

        setMessages((prevMessages) => [...prevMessages, userMessage]);

        try {
            const userMessageResponse = await createMessages(chatId, newMessage, "User");
            if (userMessageResponse && userMessageResponse.id) {
                setMessages((prevMessages) => prevMessages.map(msg =>
                    msg.id === userMessage.id ? userMessageResponse : msg
                ));
            }

            const botMessage = {
                id: `bot-${Date.now()}`,
                messageContent: "...",
                senderType: "Bot",
            };

            setMessages((prevMessages) => [...prevMessages, botMessage]);

            const botMessageResponse = await getChatbotResponse(newMessage);
            if (botMessageResponse) {
                const updatedBotMessage = { ...botMessage, messageContent: botMessageResponse };

                setMessages((prevMessages) => prevMessages.map(msg =>
                    msg.id === botMessage.id ? updatedBotMessage : msg
                ));

                const botMessageSaved = await createMessages(chatId, botMessageResponse, "Bot");
                if (botMessageSaved && botMessageSaved.id) {
                    setMessages((prevMessages) => prevMessages.map(msg =>
                        msg.id === updatedBotMessage.id ? botMessageSaved : msg
                    ));
                }
            }
        } catch (error) {
            console.error("[ChatWindow] Error sending message", error);
        } finally {
            setLoading(false);
            setNewMessage("");
        }
    };

    const getChatbotResponse = async (message: string) => {
        try {
            const apiKey = "AIzaSyBpwEbwSuVb1Aod6FNnoPVVOh53oRFZ3wU";
            const response = await fetch(`https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-pro-002:generateContent?key=${apiKey}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    contents: [{
                        parts: [{
                            text: `User message: "${message}"\n\nPlease respond in Japanese language only. Make sure to use natural Japanese expressions and appropriate levels of formality.`
                        }]
                    }]
                })
            });

            const data = await response.json();
            return data.candidates?.[0]?.content?.parts?.[0]?.text || "応答を処理できません。";
        } catch (error) {
            console.error("Error with chatbot API:", error);
            return "エラーが発生しました。";
        }
    };

    const getMessageStyle = (senderType: string) => {
        return senderType === "User"
            ? "bg-blue-500 text-white self-end rounded-lg shadow-lg p-3 max-w-xs break-words"
            : "bg-gray-200 text-black self-start rounded-lg shadow-lg p-3 max-w-xs break-words";
    };

    return (
        <div className="flex flex-col w-full max-w-3xl mx-auto bg-gray-100 shadow-md rounded-xl border border-gray-200">
            <div className="bg-gradient-to-r from-blue-700 to-blue-500 text-white p-6 rounded-t-xl">
                <h2 className="text-2xl font-semibold">Support Chat</h2>
            </div>

            <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-white rounded-b-xl max-h-[500px]">
                {messages.map((message) => (
                    <div
                        key={message.id}
                        className={`flex ${message.senderType === "User" ? "justify-end" : "justify-start"} mb-2`}
                    >
                        <div className={getMessageStyle(message.senderType)}>
                            <p className="text-sm leading-relaxed">{message.messageContent}</p>
                        </div>
                    </div>
                ))}
                <div ref={messagesEndRef} /> {/* Thêm ref để cuộn đến đây */}
            </div>

            <div className="bg-gray-200 p-4 rounded-b-xl flex items-center space-x-3">
                <input
                    type="text"
                    className="flex-1 p-3 rounded-full border border-gray-300 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
                    value={newMessage}
                    onChange={(e) => setNewMessage(e.target.value)}
                    disabled={loading}
                    placeholder="Type your message here..."
                />
                <button
                    onClick={handleSendMessage}
                    className="bg-blue-500 text-white p-3 rounded-full shadow-md hover:bg-blue-600 transition-colors duration-200 disabled:opacity-50"
                    disabled={loading}
                >
                    Send
                </button>
            </div>
        </div>
    );
};

export default ChatWindow;
