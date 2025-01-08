// import React from 'react';

// interface ChatSidebarProps {
//     chats: Array<{ id: string; name: string }>;
//     onSelectChat: (chatId: string) => void;
// }

// export default function ChatSidebar({ chats, onSelectChat }: ChatSidebarProps) {
//     return (
//         <div className="w-1/4 bg-gray-800 text-white p-4">
//             <h2 className="text-lg font-bold mb-4">Chat History</h2>
//             <ul>
//                 {chats.map((chat) => (
//                     <li
//                         key={chat.id}
//                         onClick={() => onSelectChat(chat.id)}
//                         className="cursor-pointer p-2 hover:bg-gray-700"
//                     >
//                         {chat.name}
//                     </li>
//                 ))}
//             </ul>
//         </div>
//     );
// }
