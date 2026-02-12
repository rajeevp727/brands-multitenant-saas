# GreenPantry - SMTP Email Configuration Guide

## Overview
The GreenPantry application uses SMTP protocol to send emails for password reset functionality. This guide will help you configure SMTP settings.

## Configuration

### 1. Gmail SMTP Setup (Recommended for Development)

#### Step 1: Enable 2-Factor Authentication
1. Go to your Google Account settings
2. Navigate to Security
3. Enable 2-Step Verification

#### Step 2: Generate App Password
1. Go to https://myaccount.google.com/apppasswords
2. Select "Mail" and "Windows Computer" (or other device)
3. Click "Generate"
4. Copy the 16-character password

#### Step 3: Update appsettings.json
Open `backend/GreenPantry.API/appsettings.json` and update the SMTP section:

"SMTP": {
  "Host": "smtp.gmail.com",
  "Port": "587",
  "Email": "rajeev@greenpantry.in",
  "Password": "Raj@727_eev.greenPantry",
  "EnableSSL": "true",
  "FromName": "GreenPantry"
}

### 2. Other SMTP Providers

#### Outlook/Hotmail
```json
"SMTP": {
  "Host": "smtp-mail.outlook.com",
  "Port": "587",
  "Email": "your-email@outlook.com",
  "Password": "your-password",
  "EnableSSL": "true",
  "FromName": "GreenPantry"
}
```

#### SendGrid
```json
"SMTP": {
  "Host": "smtp.sendgrid.net",
  "Port": "587",
  "Email": "apikey",
  "Password": "your-sendgrid-api-key",
  "EnableSSL": "true",
  "FromName": "GreenPantry"
}
```

#### Mailgun
```json
"SMTP": {
  "Host": "smtp.mailgun.org",
  "Port": "587",
  "Email": "postmaster@your-domain.mailgun.org",
  "Password": "your-mailgun-password",
  "EnableSSL": "true",
  "FromName": "GreenPantry"
}
```

## Testing Email Functionality

### Using the API

#### 1. Request Password Reset
```bash
curl -X POST "http://localhost:5001/api/auth/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{"email":"rajeev@example.com"}'
```

#### 2. Check Backend Logs
The reset link will be logged in the console:
```
🔐 RESET LINK: https://azure.greenpantry.in/reset-password?token=xxx&userId=yyy
```

#### 3. Reset Password
```bash
curl -X POST "http://localhost:5001/api/auth/reset-password" \
  -H "Content-Type: application/json" \
  -d '{
    "userId":"user-id-from-link",
    "token":"token-from-link",
    "newPassword":"newPassword123"
  }'
```

## Email Template

The password reset email includes:
- Professional HTML template with GreenPantry branding
- Secure reset link with token
- 1-hour expiration notice
- Security disclaimer

## Security Features

1. **Token Expiration**: Reset tokens expire after 1 hour
2. **One-Time Use**: Tokens are invalidated after successful password reset
3. **Email Enumeration Prevention**: API always returns success, even for non-existent emails
4. **Secure Password Hashing**: Uses BCrypt for password storage

## Troubleshooting

### Email Not Sending

1. **Check SMTP Credentials**: Verify email and password are correct
2. **Check Firewall**: Ensure port 587 is not blocked
3. **Check Logs**: Review backend logs for detailed error messages
4. **Gmail Specific**: 
   - Ensure 2FA is enabled
   - Use App Password, not regular password
   - Check "Less secure app access" is disabled (use App Passwords instead)

### Common Errors

#### "SMTP email is not configured"
- Solution: Add SMTP credentials to appsettings.json

#### "Authentication failed"
- Solution: Verify email/password, use App Password for Gmail

#### "Connection timeout"
- Solution: Check firewall, verify SMTP host and port

## Production Recommendations

1. **Use Environment Variables**: Store SMTP credentials in environment variables, not appsettings.json
2. **Use Professional Email Service**: Consider SendGrid, Mailgun, or AWS SES for production
3. **Implement Rate Limiting**: Prevent abuse of password reset functionality
4. **Add Email Verification**: Verify email addresses during registration
5. **Monitor Email Delivery**: Track email delivery success rates

## Environment Variables (Production)

```bash
# Set these in your production environment
export SMTP__Host="smtp.sendgrid.net"
export SMTP__Port="587"
export SMTP__Email="apikey"
export SMTP__Password="your-api-key"
export SMTP__EnableSSL="true"
export SMTP__FromName="GreenPantry"
```

## Support

For issues or questions:
- Check backend logs for detailed error messages
- Verify SMTP provider documentation
- Test with a simple SMTP client first
