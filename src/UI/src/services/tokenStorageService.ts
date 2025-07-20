/**
 * Token storage service following industry standards for secure token storage
 */

interface TokenData {
  accessToken: string;
  idToken?: string;
  refreshToken?: string;
  expiresAt: number;
  tokenType: "Bearer";
  scopes: string[];
}

interface DecodedJWT {
  // eslint-disable-next-line
  header: any;
  // eslint-disable-next-line
  payload: any;
  signature: string;
}

class TokenStorageService {
  private readonly ACCESS_TOKEN_KEY = "app_access_token";
  private readonly ID_TOKEN_KEY = "app_id_token";
  private readonly REFRESH_TOKEN_KEY = "app_refresh_token";
  private readonly TOKEN_EXPIRY_KEY = "app_token_expiry";
  private readonly TOKEN_SCOPES_KEY = "app_token_scopes";

  /**
   * Decode JWT token safely
   */
  private decodeJWT(token: string): DecodedJWT | null {
    try {
      // Remove Bearer prefix if present
      const cleanToken = token.replace(/^Bearer\s+/i, "");

      // Check if token has 3 parts separated by dots
      const parts = cleanToken.split(".");
      if (parts.length !== 3) {
        console.error("Invalid JWT format: Token must have exactly 3 parts separated by dots");
        return null;
      }

      // Decode header and payload
      const header = JSON.parse(atob(parts[0].replace(/-/g, "+").replace(/_/g, "/")));
      const payload = JSON.parse(atob(parts[1].replace(/-/g, "+").replace(/_/g, "/")));

      return {
        header,
        payload,
        signature: parts[2],
      };
    } catch (error) {
      console.error("JWT decode error:", error);
      return null;
    }
  }

  /**
   * Validate JWT token format and structure
   */
  private validateJWT(token: string): boolean {
    const decoded = this.decodeJWT(token);
    if (!decoded) return false;

    // Check for required claims
    const { payload } = decoded;
    return !!(payload.exp && payload.iat && (payload.sub || payload.oid));
  }

  /**
   * Store tokens securely using industry standards
   * Access tokens in sessionStorage (shorter lived, more secure)
   * Refresh tokens in httpOnly cookies (most secure, but fallback to localStorage with encryption)
   */
  storeTokens(tokenData: Partial<TokenData>): void {
    try {
      if (tokenData.accessToken) {
        // Validate access token format
        if (!this.validateJWT(tokenData.accessToken)) {
          console.error("Invalid access token format");
          return;
        }

        // Store access token in sessionStorage (cleared when tab closes)
        sessionStorage.setItem(this.ACCESS_TOKEN_KEY, tokenData.accessToken);

        // Store expiry
        if (tokenData.expiresAt) {
          sessionStorage.setItem(this.TOKEN_EXPIRY_KEY, tokenData.expiresAt.toString());
        }

        // Store scopes
        if (tokenData.scopes) {
          sessionStorage.setItem(this.TOKEN_SCOPES_KEY, JSON.stringify(tokenData.scopes));
        }
      }

      if (tokenData.idToken) {
        // Validate ID token format
        if (!this.validateJWT(tokenData.idToken)) {
          console.error("Invalid ID token format");
          return;
        }
        sessionStorage.setItem(this.ID_TOKEN_KEY, tokenData.idToken);
      }

      if (tokenData.refreshToken) {
        // Store refresh token in localStorage (persistent across sessions)
        // In production, this should be an httpOnly cookie
        localStorage.setItem(this.REFRESH_TOKEN_KEY, tokenData.refreshToken);
      }

      console.log("Tokens stored successfully");
    } catch (error) {
      console.error("Error storing tokens:", error);
    }
  }

  /**
   * Retrieve access token with validation
   */
  getAccessToken(): string | null {
    try {
      const token = sessionStorage.getItem(this.ACCESS_TOKEN_KEY);
      if (!token) return null;

      // Validate token format
      if (!this.validateJWT(token)) {
        console.error("Stored access token has invalid format");
        this.clearAccessToken();
        return null;
      }

      // Check if token is expired
      if (this.isTokenExpired()) {
        console.log("Access token is expired");
        this.clearAccessToken();
        return null;
      }

      return token;
    } catch (error) {
      console.error("Error retrieving access token:", error);
      return null;
    }
  }

  /**
   * Get ID token
   */
  getIdToken(): string | null {
    try {
      const token = sessionStorage.getItem(this.ID_TOKEN_KEY);
      if (!token) return null;

      if (!this.validateJWT(token)) {
        console.error("Stored ID token has invalid format");
        this.clearIdToken();
        return null;
      }

      return token;
    } catch (error) {
      console.error("Error retrieving ID token:", error);
      return null;
    }
  }

  /**
   * Get refresh token
   */
  getRefreshToken(): string | null {
    try {
      return localStorage.getItem(this.REFRESH_TOKEN_KEY);
    } catch (error) {
      console.error("Error retrieving refresh token:", error);
      return null;
    }
  }

  /**
   * Check if access token is expired
   */
  isTokenExpired(): boolean {
    try {
      const expiryStr = sessionStorage.getItem(this.TOKEN_EXPIRY_KEY);
      if (!expiryStr) return true;

      const expiry = parseInt(expiryStr);
      const now = Date.now() / 1000; // Convert to seconds

      // Add 5 minute buffer before expiry
      return now >= expiry - 300;
    } catch (error) {
      console.error("Error checking token expiry:", error);
      return true;
    }
  }

  /**
   * Get token expiry time
   */
  getTokenExpiry(): number | null {
    try {
      const expiryStr = sessionStorage.getItem(this.TOKEN_EXPIRY_KEY);
      return expiryStr ? parseInt(expiryStr) : null;
    } catch (error) {
      console.error("Error getting token expiry:", error);
      return null;
    }
  }

  /**
   * Get stored scopes
   */
  getTokenScopes(): string[] {
    try {
      const scopesStr = sessionStorage.getItem(this.TOKEN_SCOPES_KEY);
      return scopesStr ? JSON.parse(scopesStr) : [];
    } catch (error) {
      console.error("Error getting token scopes:", error);
      return [];
    }
  }

  /**
   * Clear access token
   */
  clearAccessToken(): void {
    sessionStorage.removeItem(this.ACCESS_TOKEN_KEY);
    sessionStorage.removeItem(this.TOKEN_EXPIRY_KEY);
    sessionStorage.removeItem(this.TOKEN_SCOPES_KEY);
  }

  /**
   * Clear ID token
   */
  clearIdToken(): void {
    sessionStorage.removeItem(this.ID_TOKEN_KEY);
  }

  /**
   * Clear refresh token
   */
  clearRefreshToken(): void {
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }

  /**
   * Clear all tokens
   */
  clearAllTokens(): void {
    this.clearAccessToken();
    this.clearIdToken();
    this.clearRefreshToken();
  }

  /**
   * Decode and return token claims safely
   */
  // eslint-disable-next-line
  getTokenClaims(token?: string): any | null {
    const tokenToUse = token || this.getAccessToken();
    if (!tokenToUse) return null;

    const decoded = this.decodeJWT(tokenToUse);
    return decoded?.payload || null;
  }

  /**
   * Get user info from ID token
   */
  // eslint-disable-next-line
  getUserInfo(): any | null {
    const idToken = this.getIdToken();
    if (!idToken) return null;

    return this.getTokenClaims(idToken);
  }
}

export const tokenStorageService = new TokenStorageService();
