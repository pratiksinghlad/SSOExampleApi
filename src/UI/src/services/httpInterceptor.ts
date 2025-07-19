import axios, { AxiosRequestConfig, AxiosResponse, AxiosError, InternalAxiosRequestConfig } from 'axios';
import { authService } from './authService';

// Create axios instance
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://graph.microsoft.com/v1.0',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Track ongoing token refresh to prevent multiple simultaneous requests
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (value: string) => void;
  reject: (error: any) => void;
}> = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(({ resolve, reject }) => {
    if (error) {
      reject(error);
    } else {
      if (token !== null) {
        resolve(token);
      } else {
        reject(new Error('Token is null'));
      }
    }
  });
  
  failedQueue = [];
};

// Request interceptor to add auth token
apiClient.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    try {
      // Skip auth for public endpoints
      if (config.url?.includes('/public') || config.headers?.['skip-auth']) {
        return config;
      }

      const token = await authService.getAccessToken();
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    } catch (error) {
      console.error('Failed to get access token in request interceptor:', error);
      
      // If user is authenticated but token acquisition fails, try to handle gracefully
      if (authService.isAuthenticated()) {
        console.warn('User is authenticated but token acquisition failed, proceeding without token');
      }
    }
    return config;
  },
  (error: AxiosError) => {
    console.error('Request interceptor error:', error);
    return Promise.reject(error);
  }
);

// Response interceptor to handle token expiry and refresh
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & { 
      _retry?: boolean;
      _retryCount?: number;
    };

    // Handle 401 Unauthorized errors
    if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
      
      // If already refreshing, queue this request
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then(token => {
          if (originalRequest.headers) {
            originalRequest.headers.Authorization = `Bearer ${token}`;
          }
          return apiClient(originalRequest);
        }).catch(err => {
          return Promise.reject(err);
        });
      }

      originalRequest._retry = true;
      originalRequest._retryCount = (originalRequest._retryCount || 0) + 1;

      // Prevent infinite retry loops
      if (originalRequest._retryCount > 2) {
        console.error('Max retry attempts reached, logging out user');
        await authService.logout();
        return Promise.reject(new Error('Authentication failed after multiple attempts'));
      }

      isRefreshing = true;

      try {
        console.log('Attempting to refresh token due to 401 error...');
        
        // Try to refresh the token
        const newToken = await authService.refreshToken();
        
        if (newToken && originalRequest.headers) {
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          processQueue(null, newToken);
          
          console.log('Token refreshed successfully, retrying original request');
          return apiClient(originalRequest);
        } else {
          throw new Error('Failed to obtain new token');
        }
      } catch (tokenError) {
        console.error('Token refresh failed:', tokenError);
        processQueue(tokenError, null);
        
        // If token refresh fails, logout user
        await authService.logout();
        return Promise.reject(new Error('Session expired. Please sign in again.'));
      } finally {
        isRefreshing = false;
      }
    }

    // Handle other HTTP errors
    if (error.response?.status === 403) {
      console.error('Access forbidden - insufficient permissions');
    } else if (error.response?.status >= 500) {
      console.error('Server error:', error.response.status, error.response.statusText);
    } else if (error.code === 'ECONNABORTED') {
      console.error('Request timeout');
    } else if (!error.response) {
      console.error('Network error - no response received');
    }

    return Promise.reject(error);
  }
);

// Helper function to make authenticated requests
export const makeAuthenticatedRequest = async <T = any>(
  config: AxiosRequestConfig
): Promise<AxiosResponse<T>> => {
  try {
    return await apiClient(config);
  } catch (error) {
    console.error('Authenticated request failed:', error);
    throw error;
  }
};

// Helper function to check if user needs to re-authenticate
export const checkAuthStatus = async (): Promise<boolean> => {
  try {
    await authService.getAccessToken();
    return true;
  } catch (error) {
    console.warn('Auth status check failed:', error);
    return false;
  }
};

export default apiClient;