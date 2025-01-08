// import React, { useState } from 'react';
// import { createMessages } from '../../../../../actions/messages';

// interface MessageInputProps {
//     chatId: string;
//     onNewMessage: (message: { sender: string; content: string }) => void;
// }

// export default function MessageInput({ chatId, onNewMessage }: MessageInputProps) {
//     const [messageContent, setMessageContent] = useState('');

//     const handleSendMessage = async () => {
//         if (!messageContent.trim()) return;

//         try {
//             const newMessage = await createMessages(chatId, messageContent, 'user');
//             onNewMessage(newMessage);
//             setMessageContent('');
//         } catch (error) {
//             console.error('Failed to send message:', error);
//         }
//     };

//     return (
//         <div className="flex p-4 border-t border-gray-300">
//             <input
//                 type="text"
//                 value={messageContent}
//                 onChange={(e) => setMessageContent(e.target.value)}
//                 placeholder="Type a message..."
//                 className="flex-1 p-2 border rounded"
//             />
//             <button onClick={handleSendMessage} className="ml-2 px-4 py-2 bg-blue-500 text-white rounded">
//                 Send
//             </button>
//         </div>
//     );
// }
