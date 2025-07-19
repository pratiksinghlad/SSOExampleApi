# Quick Setup Guide

## 1. Install Dependencies

```bash
npm install
```

## 2. Azure AD App Registration

### Create App Registration:
1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Azure Active Directory** > **App registrations**
3. Click **New registration**
4. Name: "React MSAL App" (or your preferred name)
5. Account types: "Accounts in this organizational directory only"
6. Redirect URI: 
   - Platform: **Single-page application (SPA)**
   - URI: `http://localhost:5173`

### After Registration:
1. Copy **Application (client) ID**
2. Copy **Directory (tenant) ID** 
3. Go to **Authentication** tab:
   - Verify `http://localhost:5173` is in SPA redirect URIs
   - Check **Access tokens** and **ID tokens** under "Implicit grant and hybrid flows"

## 3. Configure Environment

Edit `.env` file with your Azure AD details:

```env
VITE_CLIENT_ID=paste-your-client-id-here
VITE_TENANT_ID=paste-your-tenant-id-here
```

## 4. Start Development

```bash
npm run dev
```

Visit `http://localhost:5173` and test the authentication flow!

## 5. Test Authentication

1. Click "Sign in with Microsoft"
2. Complete Microsoft login
3. You should see your profile information
4. Test logout functionality

## Troubleshooting

- **Popup blocked**: Try "Sign in (Redirect)" button
- **Invalid redirect URI**: Double-check Azure AD configuration
- **Network errors**: Check browser console for details

Need help? Check the full README.md for detailed instructions.