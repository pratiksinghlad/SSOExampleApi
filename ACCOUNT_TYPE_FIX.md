# URGENT FIX: Account Type and Redirect URI Issue

## CRITICAL PROBLEM IDENTIFIED

Based on your screenshot, the issue is that your Azure AD app is configured for **organizational accounts only** but we need it for **personal Microsoft accounts**.

## IMMEDIATE SOLUTION (Choose Option A or B)

### Option A: Change to Personal Microsoft Accounts (Recommended)

1. **Go to Azure Portal** → Azure Active Directory → App registrations → SSOExampleApi
2. **Click Authentication** (left sidebar)
3. **Under "Supported account types"** → Select **"Personal Microsoft accounts only"**
4. **Save** the changes
5. **Use this configuration**:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "consumers",
    "ClientId": "8b019f08-0c45-4976-a085-bba224d83ee9",
    "ClientSecret": "YOUR_SECRET",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc",
    "SignOutUrl": "https://login.microsoftonline.com/consumers/oauth2/v2.0/logout",
    "Scopes": ["openid", "profile", "email"]
  }
}
```

### Option B: Keep Organizational Accounts (Current Setup)

If you want to keep the organizational setup, the configuration I just updated should work:

1. **Keep** your current Azure AD account type setting
2. **Use** the tenant-specific configuration (already updated in your code)
3. **Test** with accounts from your organization

## WHY THIS FIXES THE ISSUE

### Root Cause
- **Account Type Mismatch**: Your app is set to "Single tenant" but trying to use consumer endpoints
- **Wrong OAuth2 Endpoints**: Using `/consumers/` URLs with organizational accounts causes redirect URI validation to fail

### The Fix
- **Matches Account Type with Endpoints**: Organizational accounts use tenant-specific URLs
- **Correct Redirect URI Validation**: Azure AD validates URIs based on the account type

## TESTING STEPS

### After Making Changes:

1. **Build and run**: `dotnet run`
2. **Open Swagger**: https://localhost:5001
3. **Click Authorize**: Should work without redirect URI error
4. **Login**: 
   - **Option A**: Use any Outlook.com/Live.com account
   - **Option B**: Use an account from your organization (tenant)

## RECOMMENDATION

**I strongly recommend Option A (Personal Microsoft accounts)** because:

1. **More Flexible**: Works with any Microsoft personal account
2. **Easier Testing**: You can use your own Outlook.com account
3. **Better for Demos**: Don't need organizational accounts
4. **Original Intent**: Your initial setup was designed for personal accounts

## EXACT STEPS FOR OPTION A (RECOMMENDED)

1. **Azure Portal Changes**:
   - Go to your app → Authentication
   - Change account type to "Personal Microsoft accounts only"
   - Save

2. **Update appsettings.json**:
   ```json
   "TenantId": "consumers"
   "SignOutUrl": "https://login.microsoftonline.com/consumers/oauth2/v2.0/logout"
   ```

3. **Test**: Use your personal Microsoft account (Outlook.com, Live.com, etc.)

## IF YOU STILL GET REDIRECT URI ERROR

1. **Double-check redirect URIs** in Azure AD:
   ```
   https://localhost:5001/signin-oidc
   https://localhost:5001/signout-callback-oidc  
   https://localhost:5001/swagger/oauth2-redirect.html
   ```

2. **Verify case sensitivity**: URIs must match exactly
3. **Check protocol**: Must be `https://` not `http://`
4. **Confirm port**: Must be `5001` (or whatever port your app uses)

The updated code will automatically use the correct endpoints based on your TenantId setting!
