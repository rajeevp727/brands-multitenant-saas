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
        
        // DEBUG: Check what user is trying to access from which tenant
        var userAnyTenant = await _unitOfWork.Repository<User>().GetQueryable()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (userAnyTenant == null)
        {
             _logger.LogWarning("DEBUG: User {Email} does not exist in ANY tenant.", request.Email);
        }
        else
        {
             _logger.LogInformation("DEBUG: User found in DB. Tenant: {TenantId}, Username: {Username}", userAnyTenant.TenantId, userAnyTenant.Username);
        }

        var user = await _unitOfWork.Repository<User>().GetQueryable()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            if (string.Equals(request.Email, "admin@rajeev.com", StringComparison.OrdinalIgnoreCase)
                && request.Password == "Pass123")
            {
                _logger.LogWarning("Bootstrapping fallback admin account because seeded admin user is missing.");

                var adminRole = await _unitOfWork.Repository<Role>().GetQueryable()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(r => r.Name == "Admin" && r.TenantId == "rajeev-pvt");

                if (adminRole == null)
                {
                    adminRole = new Role
                    {
                        Name = "Admin",
                        TenantId = "rajeev-pvt"
                    };

                    await _unitOfWork.Repository<Role>().AddAsync(adminRole);
                    await _unitOfWork.SaveChangesAsync();
                }

                var bootstrapAdmin = new User
                {
                    Username = "Admin",
                    Email = "admin@rajeev.com",
                    LegacyPassword = "Pass123",
                    LegacyRequestedRole = "Admin",
                    PasswordHash = "Pass123",
                    RoleId = adminRole.Id,
                    TenantId = "rajeev-pvt",
                    IsActive = true,
                    CreatedBy = "Bootstrap"
                };

                await _unitOfWork.Repository<User>().AddAsync(bootstrapAdmin);
                await _unitOfWork.SaveChangesAsync();

                user = await _unitOfWork.Repository<User>().GetQueryable()
                    .IgnoreQueryFilters()
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == request.Email);
            }
        }

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email} in CURRENT Tenant context.", request.Email);
            if (userAnyTenant != null)
            {
                 _logger.LogWarning("User exists in tenant '{UserTenant}' but request is for current tenant context.", userAnyTenant.TenantId);
            }
            throw new Exception("Invalid credentials");
        }

        bool isPasswordValid = false;
        try 
        {
            isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                // Fallback for seeded users with plain text passwords (DEV/DEMO ONLY)
                if (request.Password == user.PasswordHash)
                {
                    isPasswordValid = true;
                    _logger.LogWarning("Plain text password match for {Email}", request.Email);
                }
                else
                {
                    _logger.LogWarning("BCrypt verification failed for {Email}", request.Email);
                }
            }
        }
        catch (Exception ex)
        {
            // Fallback for seeded users even if BCrypt throws (due to invalid salt/hash format)
            if (request.Password == user.PasswordHash)
            {
                isPasswordValid = true;
                _logger.LogWarning("Plain text password match (after BCrypt error) for {Email}", request.Email);
            }
            else
            {
                _logger.LogError("BCrypt error for {Email}: {Message}", request.Email, ex.Message);
            }
        }

        if (!isPasswordValid)
        {
            throw new Exception("Invalid credentials");
        }

        _logger.LogInformation("Login successful for user: {Email} (Tenant: {TenantId})", request.Email, user.TenantId);

        var token = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.SaveChangesAsync();

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
            var role = await _unitOfWork.Repository<Role>().GetQueryable()
                .FirstOrDefaultAsync(r => r.Name == "Customer");

            user = new User
            {
                Username = name,
                Email = email,
                LegacyPassword = "EXTERNAL_AUTH",
                LegacyRequestedRole = "Customer",
                PasswordHash = "EXTERNAL_AUTH",
                RoleId = role?.Id,
                IsActive = true,
                TenantId = "rajeev-pvt", // Default to main tenant for external users
                CreatedBy = "External"
            };

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
            // Re-fetch to get any auto-generated fields/includes
            user = await _unitOfWork.Repository<User>().GetQueryable()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
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
