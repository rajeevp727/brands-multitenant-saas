# GreenPantry - Login Credentials & Testing Guide

## ✅ Login is Now Working!

The login issue has been resolved. The problem was a mismatch between the email addresses in the database seed data and the credentials provided.

## 🔐 Test User Credentials

You can now login with these credentials:

### User Account
- **Email**: `rajeev@example.com`
- **Password**: `password123`
- **Role**: User

### Admin Account
- **Email**: `admin@example.com`
- **Password**: `admin123`
- **Role**: Admin

### Vendor Account
- **Email**: `vendor@example.com`
- **Password**: `vendor123`
- **Role**: Vendor

## 🚀 How to Test

### 1. Start the Backend
```bash
cd backend
dotnet run --project GreenPantry.API --launch-profile http
```
Backend will run on: `http://localhost:5001`

### 2. Start the Frontend
```bash
cd frontend
npm run dev
```
Frontend will run on: `http://localhost:3001`

### 3. Test Login
1. Open browser: `http://localhost:3001`
2. Click on "Login" or navigate to login page
3. Use any of the credentials above
4. You should be successfully logged in!

## 🧪 API Testing

### Test Login via API
```bash
curl -X POST "http://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"rajeev@example.com","password":"password123"}'
```

Expected Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "guid-here",
  "expiresAt": "2026-01-17T02:00:00Z",
  "user": {
    "id": "user-id",
    "firstName": "Rajeev",
    "lastName": "User",
    "email": "rajeev@example.com",
    "role": "User"
  }
}
```

### Test Forgot Password
```bash
curl -X POST "http://localhost:5001/api/auth/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{"email":"rajeev@example.com"}'
```

**Note**: Check backend console logs for the reset link (marked with 🔐 emoji)

## 📧 Email Configuration (Optional)

To enable email sending for password reset:

1. Open `backend/GreenPantry.API/appsettings.json`
2. Update the SMTP section with your email credentials
3. See `SMTP_SETUP.md` for detailed instructions

## 🔧 What Was Fixed

1. **Email Mismatch**: Updated `DbInitializer.cs` to use `rajeev@example.com` instead of `rajeev@greenpantry.in`
2. **BCrypt Hashing**: Ensured all passwords are properly hashed with BCrypt
3. **Database Seeding**: Fixed the upsert logic to update existing users with correct password hashes
4. **Frontend Configuration**: Updated `.env` file to point to correct backend port (5001)
5. **API Configuration**: Added JSON string enum converter for proper enum serialization

## 🎯 Features Working

- ✅ User Registration
- ✅ User Login
- ✅ JWT Token Generation
- ✅ Password Reset Request (with email logging)
- ✅ Password Reset Completion
- ✅ Role-based Authentication
- ✅ Refresh Token

## 📝 Next Steps

1. **Configure SMTP**: Follow `SMTP_SETUP.md` to enable email sending
2. **Test Checkout Flow**: Login and test the complete order flow
3. **Test Payment**: Verify UPI QR payment integration
4. **Test Admin Features**: Login as admin and test admin-specific features

## 🐛 Troubleshooting

### Login Still Not Working?

1. **Clear Browser Cache**: Hard refresh (Ctrl+Shift+R)
2. **Check Backend Logs**: Look for errors in the console
3. **Verify Backend is Running**: Visit `http://localhost:5001/health`
4. **Check Frontend Console**: Open browser DevTools and check for errors

### Database Issues?

If you need to reset the database:
```bash
cd backend
dotnet ef database drop --project GreenPantry.Infrastructure --startup-project GreenPantry.API
dotnet ef database update --project GreenPantry.Infrastructure --startup-project GreenPantry.API
```

Then restart the backend to re-seed the data.

## 📞 Support

If you encounter any issues:
1. Check the backend console logs
2. Check the browser console (F12)
3. Verify both frontend and backend are running
4. Ensure you're using the correct credentials

---

**Last Updated**: 2026-01-17
**Status**: ✅ All systems operational
