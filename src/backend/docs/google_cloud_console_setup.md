# Google Cloud Console Setup for SSO

## Required Action

You need to update the authorized redirect URIs in Google Cloud Console to fix the `redirect_uri_mismatch` error.

## Steps

1. **Go to Google Cloud Console**:
   - Navigate to: https://console.cloud.google.com/apis/credentials
   - Sign in with your Google account

2. **Select your OAuth 2.0 Client**:
   - Find and click on the OAuth 2.0 Client ID: `1064046957302-jtj2rn4msvhnb05q5id2kaii6rkr9pgf`

3. **Add Authorized Redirect URIs**:
   Click "Edit" and add these URIs to the "Authorized redirect URIs" section:

   ```
   http://localhost:5114/signin-google
   http://localhost:5173/signin-google
   https://multitenantsa as-owbt.onrender.com/signin-google
   https://rajeevstech.in/signin-google
   https://rajeevstech.com/signin-google
   ```

4. **Save Changes**:
   - Click "Save" at the bottom
   - **Wait 5-10 minutes** for changes to propagate

## Verification

After saving and waiting:

1. **Test locally**:
   - Go to `http://localhost:5173/login`
   - Click "Sign in with Google"
   - Should redirect to Google OAuth consent screen
   - After consent, should redirect back successfully

2. **Test production**:
   - Go to `https://rajeevstech.com/login`
   - Click "Sign in with Google"
   - Complete OAuth flow
   - Verify successful login

## Troubleshooting

If you still get `redirect_uri_mismatch`:
- Double-check the URIs are saved correctly
- Wait the full 10 minutes for propagation
- Check the exact redirect_uri in the browser URL when the error occurs
- Ensure it matches one of the authorized URIs exactly
