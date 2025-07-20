/**
 * API endpoint constants for the application
 */

export const PUBLIC_ENDPOINTS = {
  /**
   * Public endpoint path that doesn't require authentication
   */
  PUBLIC_PATH: "/public",
} as const;

/**
 * Authentication-related constants
 */
export const AUTH_CONSTANTS = {
  /**
   * Header name to skip authentication for specific requests
   */
  SKIP_AUTH_HEADER: "skip-auth",

  /**
   * Maximum number of retry attempts for failed requests
   */
  MAX_RETRY_ATTEMPTS: 2,

  /**
   * Request timeout in milliseconds
   */
  REQUEST_TIMEOUT: 30000,
} as const;
