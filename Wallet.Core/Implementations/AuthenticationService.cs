using AutoMapper;
using EnsureThat;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using Wallet.Core.Interfaces;
using Wallet.Data.UnitOfWork.Interface;
using Wallet.Dtos.Authentication;
using Wallet.Models;
using Wallet.Models.Mail;
using Wallet.Utils;
using Wallet.Utils.Constants;
using Wallet.Utils.EmailHelper;

namespace Wallet.Core.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;

        public AuthenticationService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork,
            ILogger logger, IMapper mapper, IConfiguration configuration, IMailService mailService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
            _mailService = mailService;
        }
        public Task<Response<bool>> ChangePassword(string id, ChangePasswordDto changePasswordDto)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> ConfirmEmail(string email, string token)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<LoginResponseDto>> Login(LoginRequestDto loginDto)
        {
            _logger.Information("Login Attempt");
            var validationResult = await ValidateUser(loginDto);

            if (!validationResult.Succeeded)
            {
                _logger.Error("Login operation failed");
                return Response<LoginResponseDto>.Fail(validationResult.Message, validationResult.StatusCode);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); //sets refresh token for 7 days
            var result = new LoginResponseDto()
            {
                Id = user.Id,
                Token = await GenerateToken(user),
                RefreshToken = refreshToken
            };

            await _userManager.UpdateAsync(user);

            _logger.Information("User successfully logged in");
            return Response<LoginResponseDto>.Success("Login Successfully", result);
        }

        public Task<Response<RefreshTokenToResponseDto>> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<string>> Register(RegisterUserDto userDto)
        {
            var user = _mapper.Map<AppUser>(userDto);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _userManager.CreateAsync(user, userDto.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Customer);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = TokenConverter.EncodeToken(token);
                    var mailBody = EmailBodyBuilder.GetEmailBody(user, linkName: "ConfirmEmail", encodedToken, controllerName: "Authentication");
                    var mailRequest = new MailRequest()
                    {
                        Subject = "Confirm Your Registration",
                        Body = mailBody,
                        ToEmail = userDto.Email
                    };

                    bool emailResult = await _mailService.SendEmailAsync(new List<MailRequest> { mailRequest }); //Sends confirmation link to users email
                    if (emailResult)
                    {
                        _logger.Information("Mail sent successfully");
                        var wallet = new UserWallet() { Balance = 0, User = user, UserId = user.Id };
                        await _unitOfWork.Wallets.InsertAsync(wallet);
                        await _unitOfWork.Save();
                        transaction.Complete();
                        return Response<string>.Success("User created successfully!", user.Id, (int)HttpStatusCode.Created);
                    }
                    else
                    {
                        _logger.Information("Mail service failed");
                        transaction.Dispose();
                        return Response<string>.Fail("Registration failed. Please try again", (int)HttpStatusCode.BadRequest);
                    }
                }

                transaction.Complete();
                return Response<string>.Fail(GetErrors(result), (int)HttpStatusCode.BadRequest); ;
            };
        }

        public Task<Response<string>> ResetPassword(ResetPasswordDto model)
        {
            throw new NotImplementedException();
        }

        public Task<Response<string>> UpdatePassword(UpdatePasswordDto model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateUserRole(string userId, string[] roles)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Stringify and returns all the identity errors
        /// </summary>
        /// <param name="result"></param>
        /// <returns>Identity Errors</returns>
        private static string GetErrors(IdentityResult result)
        {
            return result.Errors.Aggregate(string.Empty, (current, err) => current + err.Description + "\n");
        }

        /// <summary>
        /// Validates a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns true if the user exists</returns>
        private async Task<Response<bool>> ValidateUser(LoginRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Response<bool>.Fail("Invalid Credentials", (int)HttpStatusCode.BadRequest);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user) && user.EmailConfirmed)
            {
                return Response<bool>.Fail("Account not activated", (int)HttpStatusCode.Forbidden);
            }
            return Response<bool>.Success("Login successful", true);
        }

        private async Task<string> GenerateToken(AppUser user)
        {
            EnsureArg.IsNotNull(user, nameof(user));

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
            };

            //Gets the roles of the logged in user and adds it to Claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            // Specifying JWTSecurityToken Parameters
            var token = new JwtSecurityToken
            (audience: _configuration["JwtSettings:Audience"],
             issuer: _configuration["JwtSettings:Issuer"],
             claims: authClaims,
             expires: DateTime.Now.AddHours(2),
             signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private async Task<AppUser> GetUserByRefreshToken(Guid token, string userId)
        {
            EnsureArg.IsNotNullOrEmpty(userId, nameof(userId));

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == token.ToString() && u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException($"User with Id {userId} does not exist");
            }

            return user;
        }
    }
}
