import { Configuration, PopupRequest, RedirectRequest } from "@azure/msal-browser";

// MSAL configuration
export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_CLIENT_ID || "[YOUR_CLIENT_ID]",
    authority: "https://login.microsoftonline.com/consumers",
    redirectUri: import.meta.env.VITE_REDIRECT_URI || "http://localhost:5173",
    postLogoutRedirectUri: import.meta.env.VITE_POST_LOGOUT_REDIRECT_URI || "http://localhost:5173",
  },
  cache: {
    cacheLocation: "sessionStorage",
    storeAuthStateInCookie: false,
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return;
        switch (level) {
          case 0: // Error
            console.error(message);
            break;
          case 1: // Warning
            console.warn(message);
            break;
          case 2: // Info
            console.info(message);
            break;
          case 3: // Verbose
            console.debug(message);
            break;
        }
      },
    },
  },
};

// Scopes for login request
export const loginRequest: PopupRequest = {
  scopes: ["User.Read", "openid", "profile", "email"],
  prompt: "select_account",
};

// Scopes for redirect login
export const loginRedirectRequest: RedirectRequest = {
  scopes: ["User.Read", "openid", "profile", "email"],
  prompt: "select_account",
};

// Scopes for silent token requests
export const silentRequest = {
  scopes: ["User.Read"],
  forceRefresh: false,
};

// Microsoft Graph API endpoints
export const graphConfig = {
  graphMeEndpoint: "https://graph.microsoft.com/v1.0/me",
  graphMailEndpoint: "https://graph.microsoft.com/v1.0/me/messages",
};
