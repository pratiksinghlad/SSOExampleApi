import { 
  PublicClientApplication, 
  AccountInfo, 
  SilentRequest, 
  AuthenticationResult,
  InteractionRequiredAuthError,
  BrowserAuthError,
  AuthError
} from '@azure/msal-browser';
import { msalConfig, loginRequest, loginRedirectRequest, silentRequest } from '../config/authConfig';

class AuthService {
  private msalInstance: PublicClientApplication;
  private isInitialized = false;

  constructor() {
    this.msalInstance = new PublicClientApplication(msalConfig);
  }

  async initialize(): Promise<void> {
    if (!this.isInitialized) {
      await this.msalInstance.initialize();
      this.isInitialized = true;
      
      // Handle redirect response if any
      try {
        const response = await this.msalInstance.handleRedirectPromise();
        if (response) {
          console.log('Redirect response handled successfully:', response);
        }
      } catch (error) {
        console.error('Error handling redirect response:', error);
      }
    }
  }

  getInstance(): PublicClientApplication {
    return this.msalInstance;
  }

  async loginPopup(): Promise<AuthenticationResult> {
    try {
      await this.initialize();
      const response = await this.msalInstance.loginPopup(loginRequest);
      console.log('Login successful:', response);
      return response;
    } catch (error) {
      console.error('Login popup failed:', error);
      this.handleAuthError(error);
      throw error;
    }
  }

  async loginRedirect(): Promise<void> {
    try {
      await this.initialize();
      await this.msalInstance.loginRedirect(loginRedirectRequest);
    } catch (error) {
      console.error('Login redirect failed:', error);
      this.handleAuthError(error);
      throw error;
    }
  }

  async logout(): Promise<void> {
    try {
      const account = this.getAccount();
      if (account) {
        await this.msalInstance.logoutPopup({
          account: account,
          postLogoutRedirectUri: msalConfig.auth.postLogoutRedirectUri
        });
      } else {
        // Fallback logout
        await this.msalInstance.logoutPopup({
          postLogoutRedirectUri: msalConfig.auth.postLogoutRedirectUri
        });
      }
    } catch (error) {
      console.error('Logout failed:', error);
      // Clear local storage as fallback
      this.clearLocalAuth();
      window.location.reload();
    }
  }

  getAccount(): AccountInfo | null {
    const accounts = this.msalInstance.getAllAccounts();
    if (accounts.length > 1) {
      // Multiple accounts, return the first one or implement account selection logic
      console.warn('Multiple accounts found, using the first one');
    }
    return accounts.length > 0 ? accounts[0] : null;
  }

  async getAccessToken(scopes: string[] = ['User.Read'], forceRefresh = false): Promise<string> {
    const account = this.getAccount();
    if (!account) {
      throw new Error('No authenticated account found');
    }

    const tokenRequest: SilentRequest = {
      ...silentRequest,
      scopes: scopes,
      account: account,
      forceRefresh: forceRefresh
    };

    try {
      const response = await this.msalInstance.acquireTokenSilent(tokenRequest);
      console.log('Token acquired silently');
      return response.accessToken;
    } catch (error) {
      console.warn('Silent token acquisition failed:', error);
      
      if (error instanceof InteractionRequiredAuthError) {
        // Token expired or interaction required, try popup
        try {
          const response = await this.msalInstance.acquireTokenPopup({
            scopes: scopes,
            account: account,
          });
          console.log('Token acquired via popup');
          return response.accessToken;
        } catch (popupError) {
          console.error('Token acquisition via popup failed:', popupError);
          // If popup fails, logout user
          await this.logout();
          throw new Error('Authentication required. Please sign in again.');
        }
      } else {
        console.error('Token acquisition failed with non-interaction error:', error);
        throw error;
      }
    }
  }

  async refreshToken(scopes: string[] = ['User.Read']): Promise<string> {
    console.log('Forcing token refresh...');
    return this.getAccessToken(scopes, true);
  }

  isAuthenticated(): boolean {
    const account = this.getAccount();
    return account !== null;
  }

  getAccountInfo(): AccountInfo | null {
    return this.getAccount();
  }

  private handleAuthError(error: any): void {
    if (error instanceof BrowserAuthError) {
      console.error('Browser Auth Error:', error.errorCode, error.errorMessage);
    } else if (error instanceof AuthError) {
      console.error('Auth Error:', error.errorCode, error.errorMessage);
    } else {
      console.error('Unknown Auth Error:', error);
    }
  }

  private clearLocalAuth(): void {
    // Clear session storage
    sessionStorage.clear();
    // Clear any local storage items related to MSAL
    Object.keys(localStorage).forEach(key => {
      if (key.includes('msal')) {
        localStorage.removeItem(key);
      }
    });
  }

  // Check if token is expired
  isTokenExpired(): boolean {
    const account = this.getAccount();
    if (!account) return true;
    
    // This is a simplified check - MSAL handles token expiry internally
    // but you can add custom logic here if needed
    return false;
  }
}

export const authService = new AuthService();