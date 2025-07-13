# Fix 401 Unauthorized - Azure AD API Scope Configuration

## The Problem
You're getting a 401 Unauthorized error because:
1. Swagger is requesting a token for Microsoft Graph (`https://graph.microsoft.com`)
2. Your API expects a token for itself (`api://your-client-id`)
3. Personal Microsoft accounts need explicit API scope registration

## Solution: Register API Scope in Azure AD

### Step 1: Expose an API Scope
1. Go to **Azure Portal** → **App registrations** → **Your app**
2. Click **Expose an API** in the left menu
3. Click **Add a scope**
4. Set **Application ID URI** to: `api://8b019f08-0c45-4976-a085-bba224d83ee9`
5. Click **Save and continue**

### Step 2: Create the Scope
Fill in these details:
- **Scope name**: `access_as_user`
- **Who can consent**: `Admins and users`
- **Admin consent display name**: `Access API as user`
- **Admin consent description**: `Allow the application to access the API on behalf of the signed-in user`
- **User consent display name**: `Access API as user`
- **User consent description**: `Allow the application to access the API on your behalf`
- **State**: `Enabled`

Click **Add scope**

### Step 3: Update API Permissions
1. Go to **API permissions** in the left menu
2. Click **Add a permission**
3. Click **My APIs** tab
4. Select your own application
5. Select **Delegated permissions**
6. Check `access_as_user`
7. Click **Add permissions**
8. Click **Grant admin consent** (if required)

### Step 4: Test the Fixed Configuration

The code has been updated to:
1. Use proper JWT Bearer authentication for personal accounts
2. Request the correct API scope in Swagger: `api://8b019f08-0c45-4976-a085-bba224d83ee9/access_as_user`
3. Validate tokens with the correct audience and issuer

## Testing Steps
1. **Complete the Azure AD configuration above**
2. **Clear browser cache**
3. **Restart your application**
4. **Go to Swagger UI**: `https://localhost:5001/swagger`
5. **Click Authorize**
6. **Sign in and grant permissions** (you may see a new consent screen for your API)
7. **Test the `/api/User/profile` endpoint**

## Alternative: Simplified Token Validation

If the above doesn't work immediately, I can also configure the API to accept Microsoft Graph tokens directly by updating the audience validation. Let me know if you prefer this approach.

## Debugging Token Issues

If you're still getting 401 errors, you can:
1. Copy the token from Swagger
2. Go to https://jwt.ms
3. Paste the token to see its claims
4. Check the `aud` (audience) and `iss` (issuer) values
5. Make sure they match what the API expects

The token should have:
- **aud**: `8b019f08-0c45-4976-a085-bba224d83ee9` or `api://8b019f08-0c45-4976-a085-bba224d83ee9`
- **iss**: `https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0`
