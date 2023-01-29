import { initializeApp } from 'https://www.gstatic.com/firebasejs/9.15.0/firebase-app.js';
import { getAuth, createUserWithEmailAndPassword, signInWithEmailAndPassword, signOut } from 'https://www.gstatic.com/firebasejs/9.15.0/firebase-auth.js';
import { FIREBASE_CONFIG } from './const.js';

const app = initializeApp(FIREBASE_CONFIG);
const auth = getAuth(app);

export async function firebaseCreateUser(email, password) {
    try {
        await createUserWithEmailAndPassword(auth, email, password);
        await firebaseEmailSignIn(email, password);
    } catch (error) {
        var errorResult = error.code + ": " + error.message;
        return errorResult;
    };
}

export async function firebaseEmailSignIn(email, password) {
    try {
        await signInWithEmailAndPassword(auth, email, password)
    } catch (error) {
        var errorResult = error.code + ": " + error.message;
        return errorResult;
    }
}

export async function firebaseGetCurrentUser() {
    var user = await auth.currentUser;
    if (user) {
        const JsonUser = JSON.stringify(user);
        return JsonUser;
    } else {
        return null;
    }
}

export async function firebaseSignOut() {
    try {
        await signOut(auth);
        return false;
    } catch (error) {
        return true;
    }
}

export async function initializeInactivityTimer(dotnetHelper) {
    var timer;
    let counter = 0;
    function logout() {
        dotnetHelper.invokeMethodAsync("LogoutClick");
        counter++;
    }
    function resetTimer() {
        if (counter == 0) {
            clearTimeout(timer);
            timer = setTimeout(logout, 20000);
        }
    }
    document.addEventListener("mousemove", resetTimer);
    document.addEventListener("keypress", resetTimer);
}

export async function getRefreshToken(refreshToken) {
    var myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");
    var raw = JSON.stringify({
        "grant_type": "refresh_token",
        "refresh_token": refreshToken
    });
    var requestOptions = {
        method: 'POST',
        headers: myHeaders,
        body: raw,
        redirect: 'follow'
    };
    const response = await fetch("https://securetoken.googleapis.com/v1/token?key=[API_KEY]", requestOptions)
    const responseText = await response.json();
    return JSON.stringify(responseText);
}
