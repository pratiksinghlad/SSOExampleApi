# React 19 MSAL Authentication App

A modern React 19 application with Microsoft Azure AD authentication using MSAL (Microsoft Authentication Library). Features automatic token refresh, HTTP interceptors, and seamless SSO integration.

## 🚀 Features

- **Microsoft Azure AD Authentication** - Secure SSO with Microsoft Identity Platform
- **Automatic Token Refresh** - Seamless token management with silent refresh
- **HTTP Interceptors** - Automatic authentication header injection
- **Microsoft Graph API Integration** - Access to user profile data
- **React 19 & TypeScript** - Modern development stack
- **Responsive Design** - Mobile-friendly interface
- **Error Handling** - Comprehensive error handling and user feedback
- **Debug Information** - Built-in debugging tools for development

## 📋 Prerequisites

- Node.js 18+ 
- npm or yarn
- Azure AD tenant and app registration

## 🔧 Azure AD Setup

### 1. Create Azure App Registration

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Azure Active Directory** > **App registrations**
3. Click **New registration**
4. Fill in the details:
   - **Name**: Your app name (e.g., "React MSAL App")
   - **Supported account types**: Choose based on your needs
   - **Redirect URI**: 
     - Type: Single-page application (SPA)
     - URI: `http://localhost:5173` (for development)

### 2. Configure App Registration

After creating the app registration:

1. **Copy the Application (client) ID** - you'll need this for `VITE_CLIENT_ID`
2. **Copy the Directory (tenant) ID** - you'll need this for `VITE_TENANT_ID`
3. Go to **Authentication** tab:
   - Ensure `http://localhost:5173` is listed under SPA redirect URIs
   - Add `http://localhost:5173` to logout URLs if needed
   - Enable **Access tokens** and **ID tokens** under Implicit grant
4. Go to **API permissions** tab:
   - Ensure `User.Read` permission is granted (should be by default)
   - Add any additional Microsoft Graph permissions you need

## 🛠️ Installation & Setup

### 1. Clone and Install Dependencies

```bash
# Clone the repository
git clone <your-repo-url>
cd react-msal-auth-app

# Install dependencies
npm install
```

### 2. Environment Configuration

Create a `.env` file in the root directory:

```bash
cp .env.example .env
```

Edit the `.env` file with your Azure AD configuration:

```env
# Microsoft Azure AD Configuration
VITE_CLIENT_ID=your-azure-app-client-id
VITE_TENANT_ID=your-azure-tenant-id
VITE_REDIRECT_URI=http://localhost:5173
VITE_POST_LOGOUT_REDIRECT_URI=http://localhost:5173
VITE_API_BASE_URL=https://graph.microsoft.com/v1.0
```

### 3. Start Development Server

```bash
npm run dev
```

The application will be available at `http://localhost:5173`

## 🏗️ Project Structure

```
src/
├── components/
│   ├── AuthGuard.tsx      # Authentication wrapper component
│   ├── HomePage.tsx       # Main dashboard component
│   └── LoginPage.tsx      # Login interface component
├── config/
│   └── authConfig.ts      # MSAL configuration
├── services/
│   ├── authService.ts     # Authentication service
│   └── httpInterceptor.ts # HTTP client with auth interceptors
├── App.tsx               # Main app component
└── main.tsx             # Application entry point
```

## 🔐 Authentication Flow

1. **Initial Load**: App checks for existing authentication
2. **Login**: User clicks sign-in button
3. **MSAL Popup/Redirect**: Microsoft login flow
4. **Token Storage**: Tokens stored securely in session storage
5. **API Calls**: HTTP interceptor automatically adds auth headers
6. **Token Refresh**: Automatic silent token refresh when needed
7. **Logout**: Clean logout with session cleanup

## 🛡️ Security Features

- **Secure Token Storage**: Uses session storage (configurable)
- **Automatic Token Refresh**: Silent refresh prevents session expiry
- **HTTP Interceptors**: Automatic retry on 401 errors
- **PKCE Support**: Built-in PKCE for enhanced security
- **Error Handling**: Comprehensive error handling for auth failures

## 🔧 Configuration Options

### MSAL Configuration (`src/config/authConfig.ts`)

```typescript
export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_CLIENT_ID,
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_TENANT_ID}`,
    redirectUri: import.meta.env.VITE_REDIRECT_URI,
    postLogoutRedirectUri: import.meta.env.VITE_POST_LOGOUT_REDIRECT_URI
  },
  cache: {
    cacheLocation: 'sessionStorage', // or 'localStorage'
    storeAuthStateInCookie: false
  }
};
```

### Scopes Configuration

Default scopes include:
- `User.Read` - Basic user profile
- `openid` - OpenID Connect
- `profile` - User profile claims
- `email` - Email address

Add additional scopes in `authConfig.ts` as needed.

## 🚀 Deployment

### Build for Production

```bash
npm run build
```

### Environment Variables for Production

Update your production environment with:
- `VITE_CLIENT_ID` - Your Azure app client ID
- `VITE_TENANT_ID` - Your Azure tenant ID  
- `VITE_REDIRECT_URI` - Your production URL
- `VITE_POST_LOGOUT_REDIRECT_URI` - Your production URL

### Azure App Registration Updates

For production deployment:
1. Add your production URL to redirect URIs in Azure AD
2. Update logout URLs
3. Ensure proper CORS settings if needed

## 🐛 Troubleshooting

### Common Issues

1. **"Popup blocked" error**
   - Use redirect flow instead of popup
   - Allow popups for your domain

2. **"Invalid redirect URI" error**
   - Ensure redirect URI matches exactly in Azure AD
   - Check for trailing slashes

3. **Token refresh failures**
   - Check network connectivity
   - Verify Azure AD configuration
   - Check browser console for detailed errors

4. **CORS errors**
   - Ensure proper redirect URI configuration
   - Check Azure AD app registration settings

### Debug Mode

The app includes a debug card showing:
- Account ID
- Tenant ID
- Environment
- Token status

Use this information for troubleshooting authentication issues.

## 📚 Additional Resources

- [MSAL.js Documentation](https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-js-initializing-client-applications)
- [Microsoft Graph API](https://docs.microsoft.com/en-us/graph/)
- [Azure AD App Registration](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
- [React 19 Documentation](https://react.dev/)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.