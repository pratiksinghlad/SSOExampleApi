# Postman Setup for Personal Microsoft Account Authentication

## Prerequisites
1. Import the `SSOExampleApi_Postman.json` collection into Postman
2. Make sure your Azure AD app registration includes Postman's redirect URI

## Azure AD Configuration for Postman

### Step 1: Add Postman Redirect URI to Azure AD
1. Go to Azure Portal → App registrations → Your app
2. Navigate to "Authentication"
3. Add this redirect URI: `https://oauth.pstmn.io/v1/callback`
4. Make sure it's set to "Web" platform type
5. Save the changes

### Step 2: Configure Postman Collection

1. **Import the Collection**:
   - Import `SSOExampleApi_Postman.json` into Postman

2. **Configure Authentication**:
   - Open the collection settings
   - Go to the "Authorization" tab
   - The OAuth 2.0 settings should already be configured with:
     - **Grant Type**: Authorization Code with PKCE
     - **Auth URL**: `https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize`
     - **Access Token URL**: `https://login.microsoftonline.com/consumers/oauth2/v2.0/token`
     - **Client ID**: `8b019f08-0c45-4976-a085-bba224d83ee9`
     - **Scope**: `https://graph.microsoft.com/User.Read openid profile email`
     - **Redirect URI**: `https://oauth.pstmn.io/v1/callback`

3. **Get Token**:
   - Click "Get New Access Token"
   - Sign in with your personal Microsoft account
   - Grant permissions
   - Use the token for API requests

## Alternative: Manual Token Generation

If you prefer to get tokens manually, you can use this URL in your browser:

```
https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize?
client_id=8b019f08-0c45-4976-a085-bba224d83ee9
&response_type=code
&redirect_uri=https://oauth.pstmn.io/v1/callback
&scope=https://graph.microsoft.com/User.Read openid profile email
&state=postman-test
&code_challenge_method=S256
&code_challenge=GENERATED_CODE_CHALLENGE
```

**Note**: For the manual approach, you'll need to generate a PKCE code challenge and handle the authorization code exchange yourself.

## Testing the API

Once you have a valid access token:

1. **Test Authentication**: 
   - Use the "Test Authentication" request
   - Should return user information if authenticated

2. **Get User Profile**:
   - Use the "Get User Profile" request
   - Should return detailed user profile data

## Troubleshooting

1. **Invalid Redirect URI**: Make sure `https://oauth.pstmn.io/v1/callback` is added to your Azure AD app registration

2. **Scope Issues**: Ensure the scopes match what's configured in Azure AD and your API

3. **CORS Issues**: The API should handle CORS for localhost testing

4. **Token Expiry**: Tokens expire after 1 hour, get a new token if requests start failing
