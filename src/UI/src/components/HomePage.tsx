import React, { useState, useEffect } from "react";
import { UserProfile } from "../models/UserProfile";
import { useMsal } from "@azure/msal-react";
import { AccountInfo } from "@azure/msal-browser";
import apiClient, { checkAuthStatus } from "../services/httpInterceptor";
import { authService } from "../services/authService";

const HomePage: React.FC = () => {
  const { accounts } = useMsal();
  const [userProfile, setUserProfile] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [tokenInfo, setTokenInfo] = useState<{
    hasToken: boolean;
    lastRefresh: string | null;
  }>({ hasToken: false, lastRefresh: null });

  const account: AccountInfo | undefined = accounts[0];

  useEffect(() => {
    initializePage();
  }, []);

  const initializePage = async () => {
    await checkTokenStatus();
    await fetchUserProfile();
  };

  const checkTokenStatus = async () => {
    try {
      const hasValidToken = await checkAuthStatus();
      setTokenInfo({
        hasToken: hasValidToken,
        lastRefresh: hasValidToken ? new Date().toLocaleTimeString() : null,
      });
    } catch (error) {
      console.error("Token status check failed:", error);
    }
  };

  const fetchUserProfile = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await apiClient.get("/me");
      setUserProfile(response.data);

      // Update token status after successful API call
      setTokenInfo((prev) => ({
        ...prev,
        hasToken: true,
        lastRefresh: new Date().toLocaleTimeString(),
      }));
    } catch (err: unknown) {
      console.error("Failed to fetch user profile:", err);

      let errorMessage = "Failed to load user profile";
      if (typeof err === "object" && err !== null) {
        // Check for AxiosError shape
        if (
          "response" in err &&
          // eslint-disable-next-line
          typeof (err as any).response === "object" &&
          // eslint-disable-next-line
          (err as any).response !== null
        ) {
          // eslint-disable-next-line
          const status = (err as any).response.status;
          if (status === 401) {
            errorMessage = "Authentication expired. Please sign in again.";
          } else if (status === 403) {
            errorMessage = "Access denied. Insufficient permissions.";
          }
          // eslint-disable-next-line
        } else if ("message" in err && typeof (err as any).message === "string") {
          // eslint-disable-next-line
          errorMessage = (err as any).message;
        }
      }
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = async () => {
    setIsLoggingOut(true);
    try {
      await authService.logout();
    } catch (error) {
      console.error("Logout failed:", error);
      // Force logout by clearing storage and reloading
      sessionStorage.clear();
      localStorage.clear();
      window.location.reload();
    } finally {
      setIsLoggingOut(false);
    }
  };

  const handleRefreshToken = async () => {
    try {
      setLoading(true);
      await authService.refreshToken();
      await checkTokenStatus();
      await fetchUserProfile();
    } catch (error) {
      console.error("Token refresh failed:", error);
      setError("Failed to refresh authentication. Please sign in again.");
    }
  };

  if (loading && !userProfile) {
    return (
      <div style={styles.container}>
        <div style={styles.loadingCard}>
          <div style={styles.spinner}></div>
          <p style={styles.loadingText}>Loading your dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <div style={styles.headerLeft}>
          <h1 style={styles.title}>
            Welcome, {userProfile?.displayName || account?.name || "User"}!
          </h1>
          <div style={styles.statusIndicator}>
            <span
              style={{
                ...styles.statusDot,
                backgroundColor: tokenInfo.hasToken ? "#10b981" : "#ef4444",
              }}
            ></span>
            <span style={styles.statusText}>
              {tokenInfo.hasToken ? "Authenticated" : "Authentication Issue"}
            </span>
            {tokenInfo.lastRefresh && (
              <span style={styles.lastRefresh}>Last updated: {tokenInfo.lastRefresh}</span>
            )}
          </div>
        </div>
        <div style={styles.headerActions}>
          <button onClick={handleRefreshToken} style={styles.refreshButton} disabled={loading}>
            🔄 Refresh
          </button>
          <button onClick={handleLogout} style={styles.logoutButton} disabled={isLoggingOut}>
            {isLoggingOut ? "Signing out..." : "🚪 Sign Out"}
          </button>
        </div>
      </div>

      <div style={styles.content}>
        <div style={styles.profileCard}>
          <h2 style={styles.cardTitle}>
            <span style={styles.cardIcon}>👤</span>
            Your Profile
          </h2>
          {error ? (
            <div style={styles.errorContainer}>
              <div style={styles.errorMessage}>
                <span style={styles.errorIcon}>⚠️</span>
                {error}
              </div>
              <div style={styles.errorActions}>
                <button onClick={fetchUserProfile} style={styles.retryButton}>
                  🔄 Retry
                </button>
                <button onClick={handleRefreshToken} style={styles.refreshTokenButton}>
                  🔑 Refresh Auth
                </button>
              </div>
            </div>
          ) : (
            <div style={styles.profileInfo}>
              <div style={styles.profileItem}>
                <span style={styles.profileLabel}>Name:</span>
                <span style={styles.profileValue}>
                  {userProfile?.displayName || account?.name || "N/A"}
                </span>
              </div>
              <div style={styles.profileItem}>
                <span style={styles.profileLabel}>Email:</span>
                <span style={styles.profileValue}>
                  {userProfile?.mail ||
                    userProfile?.userPrincipalName ||
                    account?.username ||
                    "N/A"}
                </span>
              </div>
              {userProfile?.jobTitle && (
                <div style={styles.profileItem}>
                  <span style={styles.profileLabel}>Job Title:</span>
                  <span style={styles.profileValue}>{userProfile.jobTitle}</span>
                </div>
              )}
              {userProfile?.department && (
                <div style={styles.profileItem}>
                  <span style={styles.profileLabel}>Department:</span>
                  <span style={styles.profileValue}>{userProfile.department}</span>
                </div>
              )}
              {userProfile?.officeLocation && (
                <div style={styles.profileItem}>
                  <span style={styles.profileLabel}>Office:</span>
                  <span style={styles.profileValue}>{userProfile.officeLocation}</span>
                </div>
              )}
              {userProfile?.mobilePhone && (
                <div style={styles.profileItem}>
                  <span style={styles.profileLabel}>Mobile:</span>
                  <span style={styles.profileValue}>{userProfile.mobilePhone}</span>
                </div>
              )}
            </div>
          )}
        </div>

        <div style={styles.featuresCard}>
          <h2 style={styles.cardTitle}>
            <span style={styles.cardIcon}>🚀</span>
            Application Features
          </h2>
          <div style={styles.featuresList}>
            <div style={styles.featureItem}>
              <span style={styles.featureIcon}>🔐</span>
              <div style={styles.featureContent}>
                <div style={styles.featureTitle}>Azure AD Authentication</div>
                <div style={styles.featureDescription}>Secure SSO with Microsoft Identity</div>
              </div>
            </div>
            <div style={styles.featureItem}>
              <span style={styles.featureIcon}>🔄</span>
              <div style={styles.featureContent}>
                <div style={styles.featureTitle}>Automatic Token Refresh</div>
                <div style={styles.featureDescription}>Seamless token management</div>
              </div>
            </div>
            <div style={styles.featureItem}>
              <span style={styles.featureIcon}>🛡️</span>
              <div style={styles.featureContent}>
                <div style={styles.featureTitle}>HTTP Interceptors</div>
                <div style={styles.featureDescription}>Automatic auth header injection</div>
              </div>
            </div>
            <div style={styles.featureItem}>
              <span style={styles.featureIcon}>📊</span>
              <div style={styles.featureContent}>
                <div style={styles.featureTitle}>Microsoft Graph API</div>
                <div style={styles.featureDescription}>Access to user profile data</div>
              </div>
            </div>
            <div style={styles.featureItem}>
              <span style={styles.featureIcon}>⚡</span>
              <div style={styles.featureContent}>
                <div style={styles.featureTitle}>React 19 & TypeScript</div>
                <div style={styles.featureDescription}>Modern development stack</div>
              </div>
            </div>
          </div>
        </div>

        <div style={styles.debugCard}>
          <h2 style={styles.cardTitle}>
            <span style={styles.cardIcon}>🔧</span>
            Debug Information
          </h2>
          <div style={styles.debugInfo}>
            <div style={styles.debugItem}>
              <span style={styles.debugLabel}>Account ID:</span>
              <span style={styles.debugValue}>{account?.homeAccountId || "N/A"}</span>
            </div>
            <div style={styles.debugItem}>
              <span style={styles.debugLabel}>Tenant ID:</span>
              <span style={styles.debugValue}>{account?.tenantId || "N/A"}</span>
            </div>
            <div style={styles.debugItem}>
              <span style={styles.debugLabel}>Environment:</span>
              <span style={styles.debugValue}>{account?.environment || "N/A"}</span>
            </div>
            <div style={styles.debugItem}>
              <span style={styles.debugLabel}>Token Status:</span>
              <span
                style={{
                  ...styles.debugValue,
                  color: tokenInfo.hasToken ? "#10b981" : "#ef4444",
                }}
              >
                {tokenInfo.hasToken ? "Valid" : "Invalid/Expired"}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

const styles = {
  container: {
    minHeight: "100vh",
    backgroundColor: "#f8fafc",
    fontFamily: "Segoe UI, Tahoma, Geneva, Verdana, sans-serif",
    padding: "1rem",
  },
  header: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "flex-start",
    marginBottom: "2rem",
    backgroundColor: "white",
    padding: "1.5rem 2rem",
    borderRadius: "12px",
    boxShadow: "0 4px 6px rgba(0, 0, 0, 0.05)",
  },
  headerLeft: {
    display: "flex",
    flexDirection: "column" as const,
    gap: "0.5rem",
  },
  title: {
    fontSize: "1.75rem",
    fontWeight: "700",
    color: "#1f2937",
    margin: 0,
  },
  statusIndicator: {
    display: "flex",
    alignItems: "center",
    gap: "0.5rem",
  },
  statusDot: {
    width: "8px",
    height: "8px",
    borderRadius: "50%",
  },
  statusText: {
    fontSize: "0.875rem",
    fontWeight: "500",
    color: "#6b7280",
  },
  lastRefresh: {
    fontSize: "0.75rem",
    color: "#9ca3af",
  },
  headerActions: {
    display: "flex",
    gap: "0.75rem",
  },
  refreshButton: {
    backgroundColor: "#f3f4f6",
    color: "#374151",
    border: "1px solid #d1d5db",
    padding: "8px 16px",
    borderRadius: "6px",
    cursor: "pointer",
    fontSize: "0.875rem",
    fontWeight: "500",
    transition: "all 0.2s",
  },
  logoutButton: {
    backgroundColor: "#dc2626",
    color: "white",
    border: "none",
    padding: "8px 16px",
    borderRadius: "6px",
    cursor: "pointer",
    fontSize: "0.875rem",
    fontWeight: "500",
    transition: "all 0.2s",
  },
  content: {
    display: "grid",
    gridTemplateColumns: "repeat(auto-fit, minmax(350px, 1fr))",
    gap: "1.5rem",
  },
  profileCard: {
    backgroundColor: "white",
    padding: "2rem",
    borderRadius: "12px",
    boxShadow: "0 4px 6px rgba(0, 0, 0, 0.05)",
  },
  featuresCard: {
    backgroundColor: "white",
    padding: "2rem",
    borderRadius: "12px",
    boxShadow: "0 4px 6px rgba(0, 0, 0, 0.05)",
  },
  debugCard: {
    backgroundColor: "white",
    padding: "2rem",
    borderRadius: "12px",
    boxShadow: "0 4px 6px rgba(0, 0, 0, 0.05)",
    gridColumn: "1 / -1",
  },
  cardTitle: {
    display: "flex",
    alignItems: "center",
    gap: "0.5rem",
    fontSize: "1.25rem",
    fontWeight: "600",
    color: "#1f2937",
    marginBottom: "1.5rem",
  },
  cardIcon: {
    fontSize: "1.25rem",
  },
  profileInfo: {
    display: "flex",
    flexDirection: "column" as const,
    gap: "1rem",
  },
  profileItem: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    padding: "0.75rem",
    backgroundColor: "#f9fafb",
    borderRadius: "6px",
  },
  profileLabel: {
    fontWeight: "500",
    color: "#374151",
  },
  profileValue: {
    color: "#6b7280",
    textAlign: "right" as const,
  },
  featuresList: {
    display: "flex",
    flexDirection: "column" as const,
    gap: "1rem",
  },
  featureItem: {
    display: "flex",
    alignItems: "center",
    gap: "1rem",
    padding: "1rem",
    backgroundColor: "#f9fafb",
    borderRadius: "8px",
  },
  featureIcon: {
    fontSize: "1.5rem",
  },
  featureContent: {
    display: "flex",
    flexDirection: "column" as const,
    gap: "0.25rem",
  },
  featureTitle: {
    fontWeight: "500",
    color: "#1f2937",
  },
  featureDescription: {
    fontSize: "0.875rem",
    color: "#6b7280",
  },
  debugInfo: {
    display: "grid",
    gridTemplateColumns: "repeat(auto-fit, minmax(250px, 1fr))",
    gap: "1rem",
  },
  debugItem: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    padding: "0.75rem",
    backgroundColor: "#f9fafb",
    borderRadius: "6px",
  },
  debugLabel: {
    fontWeight: "500",
    color: "#374151",
  },
  debugValue: {
    color: "#6b7280",
    fontSize: "0.875rem",
    fontFamily: "monospace",
  },
  loadingCard: {
    display: "flex",
    flexDirection: "column" as const,
    alignItems: "center",
    justifyContent: "center",
    backgroundColor: "white",
    padding: "3rem",
    borderRadius: "12px",
    boxShadow: "0 4px 6px rgba(0, 0, 0, 0.05)",
    margin: "2rem auto",
    maxWidth: "400px",
  },
  spinner: {
    width: "40px",
    height: "40px",
    border: "4px solid #f3f3f3",
    borderTop: "4px solid #0078d4",
    borderRadius: "50%",
    animation: "spin 1s linear infinite",
    marginBottom: "1rem",
  },
  loadingText: {
    color: "#6b7280",
    fontSize: "1rem",
    margin: 0,
  },
  errorContainer: {
    padding: "1rem",
    backgroundColor: "#fef2f2",
    border: "1px solid #fecaca",
    borderRadius: "8px",
  },
  errorMessage: {
    display: "flex",
    alignItems: "center",
    color: "#dc2626",
    marginBottom: "1rem",
  },
  errorIcon: {
    marginRight: "0.5rem",
    fontSize: "1.25rem",
  },
  errorActions: {
    display: "flex",
    gap: "0.5rem",
  },
  retryButton: {
    backgroundColor: "#0078d4",
    color: "white",
    border: "none",
    padding: "8px 16px",
    borderRadius: "4px",
    cursor: "pointer",
    fontSize: "0.875rem",
  },
  refreshTokenButton: {
    backgroundColor: "#059669",
    color: "white",
    border: "none",
    padding: "8px 16px",
    borderRadius: "4px",
    cursor: "pointer",
    fontSize: "0.875rem",
  },
};

export default HomePage;
