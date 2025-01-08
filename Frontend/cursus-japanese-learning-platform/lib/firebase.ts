import { initializeApp } from 'firebase/app';
import { getAuth, getRedirectResult, GoogleAuthProvider, signInWithPopup, signInWithRedirect, signOut } from "firebase/auth";

const firebaseConfig = {
    apiKey: "AIzaSyDkzYdK1W8NwoZnJyCCb3xdB6gxVdvsmpc",
    authDomain: "cursus-japanese.firebaseapp.com",
    projectId: "cursus-japanese",
    storageBucket: "cursus-japanese.appspot.com",
    messagingSenderId: "1019529004140",
    appId: "1:1019529004140:web:031c160411adedcfc44449",
    measurementId: "G-8L3WPGW7M7"
  };

const app = initializeApp(firebaseConfig);
const provider = new GoogleAuthProvider();
provider.setCustomParameters({
    prompt: "select_account "
});
export const auth = getAuth(app);
export const signInWithGoogleRedirect = () => signInWithRedirect(auth, provider);
export const getGoogleRedirectResult = () => getRedirectResult(auth);
export const signInWithGooglePopup = () => signInWithPopup(auth, provider);
export const signOutUser = () => signOut(auth);