using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaaS.Application.DTOs;
using SaaS.Domain.Entities;
using SaaS.Domain.Interfaces;
using System.Threading.Tasks;

namespace SaaS.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<User> FindOrRegisterExternalUserAsync(string email, string name);
    string GenerateTokenForUser(User user);
    string GetFrontendRedirectUrl(string token, string email);
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtProvider _jwtProvider;
    private readonly Microsoft.Extensions.Logging.ILogger<AuthService> _logger;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IJwtProvider jwtProvider, Microsoft.Extensions.Logging.ILogger<AuthService> logger, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);
        
        // BOOTSTRAP ADMIN - Direct hardcoded authentication for testing
        if (string.Equals(request.Email, "admin@rajeev.com", StringComparison.OrdinalIgnoreCase)
            && request.Password == "Pass123")
        {
            _logger.LogInformation("Bootstrap admin login successful!");
            
            // Create a temporary admin user object
            var bootstrapAdmin = new User
            {
                Id = Guid.NewGuid(),
                Username = "Admin",
                Email = "admin@rajeev.com",
                PasswordHash = "Pass123",
                TenantId = "rajeev-pvt",
                IsActive = true,
                CreatedBy = "Bootstrap"
            };

            var token = _jwtProvider.GenerateToken(bootstrapAdmin);
            var refreshToken = GenerateRefreshToken();

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                User = new UserDto
                {
                    Id = bootstrapAdmin.Id.ToString(),
                    Username = bootstrapAdmin.Username,
                    Email = bootstrapAdmin.Email,
                    Role = "Admin",
                    TenantId = bootstrapAdmin.TenantId
                }
            };
        }

        // Normal flow - try to find user in database
        var user = await _unitOfWork.Repository<User>().GetQueryable()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // If not found in current tenant, try without tenant filter
        if (user == null)
        {
            _logger.LogWarning("User {Email} not found in current tenant context. Checking all tenants...", request.Email);
            user = await _unitOfWork.Repository<User>().GetQueryable()
                .IgnoreQueryFilters()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null)
            {
                _logger.LogWarning("User {Email} does not exist in ANY tenant.", request.Email);
            }
            else
            {
                _logger.LogInformation("User found in different tenant. Tenant: {TenantId}, Username: {Username}", user.TenantId, user.Username);
            }
        }

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email}.", request.Email);
            throw new Exception("Invalid credentials");
        }

        // Simple plaintext password comparison (NO HASHING)
        bool isPasswordValid = request.Password == user.PasswordHash;

        if (!isPasswordValid)
        {
            _logger.LogWarning("Invalid password for {Email}", request.Email);
            throw new Exception("Invalid credentials");
        }

        _logger.LogInformation("Login successful for user: {Email} (Tenant: {TenantId})", request.Email, user.TenantId);

        var userToken = _jwtProvider.GenerateToken(user);
        var refreshTokenVal = GenerateRefreshToken();
        user.RefreshToken = refreshTokenVal;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            Token = userToken,
            RefreshToken = refreshTokenVal,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime.Value,
            User = new UserDto
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                Role = user.Role?.Name ?? "User",
                TenantId = user.TenantId
            }
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Global Query Filter automatically filters by TenantId, 
        // but we explicitly check uniqueness for clarity and demo safety.
        var existingUser = await _unitOfWork.Repository<User>().GetQueryable()
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            throw new Exception("This email is already registered with this brand. Choose another email or brand.");
        }

        var roleName = request.RoleName ?? "Customer";
        var role = await _unitOfWork.Repository<Role>().GetQueryable()
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
        {
             // Fallback to any role if seeded roles are missing
             role = await _unitOfWork.Repository<Role>().GetQueryable().FirstOrDefaultAsync();
        }

        var refreshToken = GenerateRefreshToken();
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            LegacyPassword = request.Password,
            LegacyRequestedRole = request.RoleName ?? "Customer",
            PasswordHash = request.Password, // Simple for demo
            RoleId = role?.Id,
            IsActive = true,
            CreatedBy = "Self",
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
        };

        await _unitOfWork.Repository<User>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtProvider.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime.Value,
            User = new UserDto
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                Role = role?.Name ?? "User",
                TenantId = user.TenantId
            }
        };
    }

    public async Task<User> FindOrRegisterExternalUserAsync(string email, string name)
    {
        var user = await _unitOfWork.Repository<User>().GetQueryable()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            try
            {
                var role = await _unitOfWork.Repository<Role>().GetQueryable()
                    .FirstOrDefaultAsync(r => r.Name == "Customer");

                // Ensure name/username is not empty
                var username = !string.IsNullOrWhiteSpace(name) ? name : email.Split('@')[0];

                user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Email = email,
                    LegacyPassword = "EXTERNAL_AUTH",
                    LegacyRequestedRole = "Customer",
                    PasswordHash = "EXTERNAL_AUTH",
                    RoleId = role?.Id,
                    IsActive = true,
                    TenantId = "rajeev-pvt", // Default to main tenant for external users
                    CreatedBy = "GoogleOAuth",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsEmailVerified = true, // Email verified through OAuth provider
                    IsDeleted = false
                };

                await _unitOfWork.Repository<User>().AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("External user created successfully: {Email} via GoogleOAuth", email);
                
                // Re-fetch to get any auto-generated fields/includes
                user = await _unitOfWork.Repository<User>().GetQueryable()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating external user {Email}: {Message}", email, ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception: {Message}", ex.InnerException.Message);
                }
                throw new Exception($"Failed to create external user: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        return user!;
    }

    public string GenerateTokenForUser(User user)
    {
        return _jwtProvider.GenerateToken(user);
    }

    public string GetFrontendRedirectUrl(string token, string email)
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? (_configuration["ASPNETCORE_ENVIRONMENT"] == "Development" ? "http://localhost:5173" : "https://rajeevs-pvt-ltd.vercel.app");
        // Token is now handled via secure cookies, so we only redirect to the callback page
        return $"{frontendUrl}/auth/callback?email={email}";
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _unitOfWork.Repository<User>().GetQueryable()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new Exception("Invalid or expired refresh token");
        }

        var newJwtToken = _jwtProvider.GenerateToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime.Value,
            User = new UserDto
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                Role = user.Role?.Name ?? "User",
                TenantId = user.TenantId
            }
        };
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
