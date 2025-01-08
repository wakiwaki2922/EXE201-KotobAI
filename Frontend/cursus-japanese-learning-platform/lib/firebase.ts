import { initializeApp } from 'firebase/app';
import { getAuth, getRedirectResult, GoogleAuthProvider, signInWithPopup, signInWithRedirect, signOut } from "firebase/auth";

const firebaseConfig = {
    apiKey: "",
    authDomain: "",
    projectId: "",
    storageBucket: "",
    messagingSenderId: "",
    appId: "",
    measurementId: ""
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