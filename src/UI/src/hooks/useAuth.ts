import { useEffect, useState } from "react";
import { authService } from "../services/authService";
import { tokenStorageService } from "../services/tokenStorageService";

interface AuthState {
  isAuthenticated: boolean;
  isLoading: boolean;
  //tslint:disable-next-line
  user: UserProfile | null;
  error: string | null;
}
import { UserProfile } from "../models/UserProfile";

/**
 * Custom hook for authentication state management
 * Automatically checks stored tokens when the app loads
 */
export const useAuth = (): AuthState & {
  login: () => Promise<void>;
  logout: () => Promise<void>;
  refreshToken: () => Promise<void>;
} => {
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    isLoading: true,
    user: null,
    error: null,
  });

  useEffect(() => {
    initializeAuth();
  }, []);

  const initializeAuth = async () => {
    try {
      // Initialize MSAL
      await authService.initialize();

      // Check if user has valid stored tokens
      const storedToken = tokenStorageService.getAccessToken();
      const userInfo = tokenStorageService.getUserInfo();

      if (storedToken && userInfo) {
        // User has valid stored session
        setAuthState({
          isAuthenticated: true,
          isLoading: false,
          user: userInfo,
          error: null,
        });
      } else {
        // Check if MSAL has an account (in case of page refresh)
        const account = authService.getAccount();
        if (account) {
          try {
            // Try to get a fresh token
            await authService.getAccessToken();
            const refreshedUserInfo = authService.getUserInfo();

            setAuthState({
              isAuthenticated: true,
              isLoading: false,
              user: refreshedUserInfo,
              error: null,
            });
          } catch (error) {
            console.error("Failed to refresh token on initialization:", error);
            setAuthState({
              isAuthenticated: false,
              isLoading: false,
              user: null,
              error: "Session expired",
            });
          }
        } else {
          // No authentication state
          setAuthState({
            isAuthenticated: false,
            isLoading: false,
            user: null,
            error: null,
          });
        }
      }
    } catch (error) {
      console.error("Auth initialization failed:", error);
      setAuthState({
        isAuthenticated: false,
        isLoading: false,
        user: null,
        error: "Authentication initialization failed",
      });
    }
  };

  const login = async () => {
    try {
      setAuthState((prev) => ({ ...prev, isLoading: true, error: null }));

      await authService.loginPopup();
      const userInfo = authService.getUserInfo();

      setAuthState({
        isAuthenticated: true,
        isLoading: false,
        user: userInfo,
        error: null,
      });
    } catch (error) {
      console.error("Login failed:", error);
      setAuthState({
        isAuthenticated: false,
        isLoading: false,
        user: null,
        error: "Login failed",
      });
      throw error;
    }
  };

  const logout = async () => {
    try {
      setAuthState((prev) => ({ ...prev, isLoading: true }));

      await authService.logout();

      setAuthState({
        isAuthenticated: false,
        isLoading: false,
        user: null,
        error: null,
      });
    } catch (error) {
      console.error("Logout failed:", error);
      // Force clear state even if logout fails
      setAuthState({
        isAuthenticated: false,
        isLoading: false,
        user: null,
        error: null,
      });
    }
  };

  const refreshToken = async () => {
    try {
      await authService.refreshToken();
      const userInfo = authService.getUserInfo();

      setAuthState((prev) => ({
        ...prev,
        user: userInfo,
        error: null,
      }));
    } catch (error) {
      console.error("Token refresh failed:", error);
      setAuthState({
        isAuthenticated: false,
        isLoading: false,
        user: null,
        error: "Session expired",
      });
      throw error;
    }
  };

  return {
    ...authState,
    login,
    logout,
    refreshToken,
  };
};
