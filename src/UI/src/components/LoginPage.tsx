import React, { useState, useEffect } from "react";
import { useMsal } from "@azure/msal-react";
import { loginRequest, loginRedirectRequest } from "../config/authConfig";
import { BrowserAuthError, InteractionRequiredAuthError } from "@azure/msal-browser";

const LoginPage: React.FC = () => {
  const { instance } = useMsal();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loginMethod, setLoginMethod] = useState<"popup" | "redirect" | null>(null);

  useEffect(() => {
    // Clear any previous errors when component mounts
    setError(null);
  }, []);

  const handleLogin = async (method: "popup" | "redirect" = "popup") => {
    setIsLoading(true);
    setError(null);
    setLoginMethod(method);

    try {
      if (method === "popup") {
        await instance.loginPopup(loginRequest);
      } else {
        await instance.loginRedirect(loginRedirectRequest);
      }
      // eslint-disable-next-line
    } catch (error: any) {
      console.error(`Login ${method} failed:`, error);

      let errorMessage = "An unexpected error occurred during sign-in.";

      if (error instanceof BrowserAuthError) {
        switch (error.errorCode) {
          case "popup_window_error":
            errorMessage = "Popup was blocked. Please allow popups or try redirect sign-in.";
            break;
          case "empty_window_error":
            errorMessage = "Sign-in window was closed. Please try again.";
            break;
          case "user_cancelled":
            errorMessage = "Sign-in was cancelled.";
            break;
          case "monitor_window_timeout":
            errorMessage = "Sign-in timed out. Please try again.";
            break;
          default:
            errorMessage = `Sign-in error: ${error.errorMessage}`;
        }
      } else if (error instanceof InteractionRequiredAuthError) {
        errorMessage = "Additional authentication required. Please try again.";
      } else if (error.message) {
        errorMessage = error.message;
      }

      setError(errorMessage);
    } finally {
      setIsLoading(false);
      setLoginMethod(null);
    }
  };

  const clearError = () => {
    setError(null);
  };

  return (
    <div style={styles.container}>
      <div style={styles.loginCard}>
        <div style={styles.header}>
          <div style={styles.logoContainer}>
            <div style={styles.logo}>🔐</div>
          </div>
          <h1 style={styles.title}>Welcome</h1>
          <p style={styles.subtitle}>
            Sign in with your Microsoft account to access your dashboard
          </p>
        </div>

        {error && (
          <div style={styles.errorContainer}>
            <div style={styles.errorMessage}>
              <span style={styles.errorIcon}>⚠️</span>
              {error}
            </div>
            <button onClick={clearError} style={styles.clearErrorButton}>
              ✕
            </button>
          </div>
        )}

        <div style={styles.buttonContainer}>
          <button
            onClick={() => handleLogin("popup")}
            disabled={isLoading}
            style={{
              ...styles.loginButton,
              ...(isLoading && loginMethod === "popup" ? styles.loadingButton : {}),
            }}
          >
            {isLoading && loginMethod === "popup" ? (
              <>
                <span style={styles.spinner}></span>
                Signing in...
              </>
            ) : (
              <>
                <span style={styles.buttonIcon}>🚀</span>
                Sign in with Microsoft
              </>
            )}
          </button>

          <button
            onClick={() => handleLogin("redirect")}
            disabled={isLoading}
            style={{
              ...styles.redirectButton,
              ...(isLoading && loginMethod === "redirect" ? styles.loadingButton : {}),
            }}
          >
            {isLoading && loginMethod === "redirect" ? (
              <>
                <span style={styles.spinner}></span>
                Redirecting...
              </>
            ) : (
              <>
                <span style={styles.buttonIcon}>↗️</span>
                Sign in (Redirect)
              </>
            )}
          </button>
        </div>

        <div style={styles.infoSection}>
          <div style={styles.infoItem}>
            <span style={styles.infoIcon}>🔒</span>
            <span>Secure Azure AD Authentication</span>
          </div>
          <div style={styles.infoItem}>
            <span style={styles.infoIcon}>🔄</span>
            <span>Automatic Token Refresh</span>
          </div>
          <div style={styles.infoItem}>
            <span style={styles.infoIcon}>👤</span>
            <span>Single Sign-On (SSO)</span>
          </div>
        </div>

        <div style={styles.footer}>
          <p style={styles.footerText}>Powered by Microsoft Identity Platform</p>
        </div>
      </div>
    </div>
  );
};

const styles = {
  container: {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    minHeight: "100vh",
    backgroundColor: "#f5f5f5",
    fontFamily: "Segoe UI, Tahoma, Geneva, Verdana, sans-serif",
    padding: "1rem",
  },
  loginCard: {
    backgroundColor: "white",
    padding: "2.5rem",
    borderRadius: "12px",
    boxShadow: "0 8px 32px rgba(0, 0, 0, 0.1)",
    textAlign: "center" as const,
    maxWidth: "450px",
    width: "100%",
  },
  header: {
    marginBottom: "2rem",
  },
  logoContainer: {
    marginBottom: "1rem",
  },
  logo: {
    fontSize: "3rem",
    marginBottom: "0.5rem",
  },
  title: {
    fontSize: "2rem",
    fontWeight: "600",
    color: "#333",
    marginBottom: "0.5rem",
    margin: 0,
  },
  subtitle: {
    fontSize: "1rem",
    color: "#666",
    margin: 0,
    lineHeight: "1.5",
  },
  errorContainer: {
    display: "flex",
    alignItems: "flex-start",
    backgroundColor: "#fef2f2",
    border: "1px solid #fecaca",
    borderRadius: "6px",
    padding: "0.75rem",
    marginBottom: "1.5rem",
    position: "relative" as const,
  },
  errorMessage: {
    display: "flex",
    alignItems: "center",
    color: "#dc2626",
    fontSize: "0.875rem",
    flex: 1,
    textAlign: "left" as const,
  },
  errorIcon: {
    marginRight: "0.5rem",
    fontSize: "1rem",
  },
  clearErrorButton: {
    background: "none",
    border: "none",
    color: "#dc2626",
    cursor: "pointer",
    fontSize: "1rem",
    padding: "0",
    marginLeft: "0.5rem",
  },
  buttonContainer: {
    display: "flex",
    flexDirection: "column" as const,
    gap: "1rem",
    marginBottom: "2rem",
  },
  loginButton: {
    backgroundColor: "#0078d4",
    color: "white",
    border: "none",
    padding: "14px 24px",
    borderRadius: "6px",
    fontSize: "1rem",
    fontWeight: "500",
    cursor: "pointer",
    transition: "all 0.2s",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    gap: "0.5rem",
  },
  redirectButton: {
    backgroundColor: "transparent",
    color: "#0078d4",
    border: "2px solid #0078d4",
    padding: "12px 24px",
    borderRadius: "6px",
    fontSize: "1rem",
    fontWeight: "500",
    cursor: "pointer",
    transition: "all 0.2s",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    gap: "0.5rem",
  },
  loadingButton: {
    opacity: 0.7,
    cursor: "not-allowed",
  },
  buttonIcon: {
    fontSize: "1rem",
  },
  spinner: {
    width: "16px",
    height: "16px",
    border: "2px solid transparent",
    borderTop: "2px solid currentColor",
    borderRadius: "50%",
    animation: "spin 1s linear infinite",
  },
  infoSection: {
    display: "flex",
    flexDirection: "column" as const,
    gap: "0.75rem",
    marginBottom: "2rem",
    padding: "1rem",
    backgroundColor: "#f8fafc",
    borderRadius: "6px",
  },
  infoItem: {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    gap: "0.5rem",
    fontSize: "0.875rem",
    color: "#555",
  },
  infoIcon: {
    fontSize: "1rem",
  },
  footer: {
    borderTop: "1px solid #eee",
    paddingTop: "1rem",
  },
  footerText: {
    fontSize: "0.875rem",
    color: "#888",
    margin: 0,
  },
};

export default LoginPage;
