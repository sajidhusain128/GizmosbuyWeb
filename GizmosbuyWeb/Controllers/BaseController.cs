using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GizmosbuyWeb.Controllers
{
    public class BaseController : Controller
    {
        public readonly IWebConfiguration _webConfiguration;
        public BaseController(IOptions<WebConfiguration> webConfiguration)
        {
            _webConfiguration = webConfiguration.Value;
        }

        public Task CreateAuthenticationTicket(IUserModel user)
        {
            var key = Encoding.ASCII.GetBytes(_webConfiguration.SecretKey);
            var JWToken = new JwtSecurityToken(
                issuer: _webConfiguration.Issuer,
                audience: _webConfiguration.Issuer,
                claims: GetUserClaims(user),
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Now.AddMinutes(_webConfiguration.SesssionTimeoutMinutes)).DateTime,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );

            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            SetSession(user, token);
            return Task.CompletedTask;
        }

        private IEnumerable<Claim> GetUserClaims(IUserModel user)
        {
            List<Claim> claims = new List<Claim>();
            Claim _claim;
            _claim = new Claim(ClaimTypes.Name, user.UserName);
            claims.Add(_claim);
            _claim = new Claim(ClaimTypes.Email, user.Email);
            claims.Add(_claim);
            _claim = new Claim("Role", user.UserRole);
            claims.Add(_claim);

            return claims.AsEnumerable<Claim>();
        }

        private void SetSession(IUserModel user, string token)
        {
            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.UserRole);
            HttpContext.Session.SetString("Location", user.Location);
            HttpContext.Session.SetString("LocationId", user.locationId.ToString());
        }

        // Generate JWT Token
        //private string GenerateJwtToken(User user)
        //{
        //    string rawkey = _config["Jwt:SecretKey"].ToString();
        //    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(rawkey));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    };

        //    var token = new JwtSecurityToken(
        //        issuer: _config["Jwt:WebSiteDomain"],
        //        audience: _config["Jwt:WebSiteDomain"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(1),
        //        signingCredentials: creds);

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}
