﻿namespace OnlineExam.Core.IServices.Provider
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtSetting _jwtSetting;

        public JwtProvider(IOptions<JwtSetting> jwtSetting)
        {
            _jwtSetting = jwtSetting.Value;
        }

        public (string token, int expireIn) GenerateToken(AppUser applicationUser, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            var descriptor = new SecurityTokenDescriptor()
            {
                Issuer = _jwtSetting.Issuer,
                Audience = _jwtSetting.Audience,
                Expires = DateTime.Now.AddMinutes(_jwtSetting.lifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key)), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
                    new Claim(ClaimTypes.GivenName, applicationUser.FullName),
                    new Claim(ClaimTypes.Name, applicationUser.UserName!),
                    new Claim(ClaimTypes.Email, applicationUser.Email!),
                    new Claim(nameof(roles), System.Text.Json.JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
                    new Claim(nameof(permissions), System.Text.Json.JsonSerializer.Serialize(permissions), JsonClaimValueTypes.JsonArray),
                })
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var createToken = tokenHandler.CreateToken(descriptor);
            var token = tokenHandler.WriteToken(createToken);

            return (token, _jwtSetting.lifeTime);
        }

        public string? ValidateToken(string Token)
        {
            var handler = new JwtSecurityTokenHandler();
            var SymmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key));

            try
            {
                handler.ValidateToken(Token, new TokenValidationParameters
                {
                    IssuerSigningKey = SymmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                },
                out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = jwtToken.Claims.First(claim => claim.Type == "nameid").Value;

                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
