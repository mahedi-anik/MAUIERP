using MAUIERP.ApplicationLayer.Common.Interfaces;
using MAUIERP.ApplicationLayer.Common.Models;
using MAUIERP.ApplicationLayer.DTOs.Auth;
using MAUIERP.Domain.Entities.Auth;
using MAUIERP.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MAUIERP.ApplicationLayer.Features.Auth.Commands
{
    public record LoginCommand(string UsernameOrEmail, string Password) : IRequest<Result<LoginResponseDto>>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher _passwordHasher;

        public LoginCommandHandler(IApplicationDbContext context, IJwtService jwtService, IPasswordHasher passwordHasher)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail, cancellationToken);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return Result<LoginResponseDto>.Failure("Invalid username/email or password");

            if (user.Status != UserStatus.Active)
                return Result<LoginResponseDto>.Failure("Account is not active");

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToList();

            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = "127.0.0.1"
            };

            await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Status = user.Status.ToString(),
                Roles = roles
            };

            return Result<LoginResponseDto>.Success(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = userDto
            });
        }
    }
}
