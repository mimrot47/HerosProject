using HerosProject.Data;
using HerosProject.Entity;
using HerosProject.Handler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HerosProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly JwtSettings _jwtSettings;
        private readonly RefreshHandler _refreshHandler;

        public Auth(DataContext dataContext, IOptions<JwtSettings> options, RefreshHandler refresh)
        {
            _dataContext = dataContext;
            _jwtSettings = options.Value;
            _refreshHandler = refresh;
        }

        [HttpPost]
        [Route("CreateToken")]
        public async Task<IActionResult> CreateUser([FromForm] userCred userauth)
        {
            var result = await _dataContext.users.FirstOrDefaultAsync(x => x.Uname == userauth.uname && x.Password == userauth.password);
            if (result != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.securityKey);
                var tokenDesc = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name,result.FristName),
                        new Claim(ClaimTypes.Role,result.LastName)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                };
                var token = tokenHandler.CreateToken(tokenDesc);
                var finalToken = tokenHandler.WriteToken(token);
                return Ok(new TokenResponse
                {
                    Token = finalToken,
                    RefreshToken = await _refreshHandler.CreateToken(userauth.uname),
                });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("GenerateTokenFromRefreshToken")]
        public async Task<IActionResult> GenerateRefreshTaken([FromBody] TokenResponse tokens)
        {
            var refreshtoken = _dataContext.refreshtokens.FirstOrDefaultAsync(item => item.refreshToken == tokens.RefreshToken);
            if (refreshtoken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.securityKey);
                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(tokens.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out securityToken);
                var _token = securityToken as JwtSecurityToken;
                if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string username = principle.Identity.Name;
                    var exist = _dataContext.refreshtokens.FirstOrDefaultAsync(x => x.userID == username && x.refreshToken == tokens.RefreshToken);
                    if (exist != null)
                    {
                        var newtoken = new JwtSecurityToken(
                            claims: principle.Claims.ToArray(),
                            expires:DateAndTime.Now.AddMinutes(6),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.securityKey)),SecurityAlgorithms.HmacSha256)
                            );
                        var _finalToke = tokenHandler.WriteToken(newtoken);
                        return Ok(new TokenResponse
                        {
                            Token = _finalToke,
                            RefreshToken = await _refreshHandler.CreateToken(username),
                        });
                    }
                    else
                    {
                        return Unauthorized();
                    }                   
                }
            }
            return Ok();
        }
    }
}
