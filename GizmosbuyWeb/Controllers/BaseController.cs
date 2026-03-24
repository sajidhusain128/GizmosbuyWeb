using System.Security.Claims;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.Web.Configurations;
using Gizmosbuy.Web.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GizmosbuyWeb.Controllers
{
    [NoCache]
    [EnableCors(Constant.MyPolicy)]
    public class BaseController : Controller
    {
        public readonly IWebConfiguration _webConfiguration;
        public BaseController(IOptions<WebConfiguration> webConfiguration)
        {
            _webConfiguration = webConfiguration.Value;
        }

        //public async Task<JwtSecurityToken> CreateAuthenticationTicket(IUserModel user)
        //{
        //    var key = Encoding.ASCII.GetBytes(_webConfiguration.SecretKey);
        //    var JWToken = new JwtSecurityToken(
        //        issuer: _webConfiguration.Issuer,
        //        audience: _webConfiguration.Issuer,
        //        claims: GetUserClaims(user),
        //        notBefore: new DateTimeOffset(DateTime.Now).DateTime,
        //        expires: new DateTimeOffset(DateTime.Now.AddMinutes(_webConfiguration.SesssionTimeoutMinutes)).DateTime,
        //        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    );

        //    var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
        //    SetSession(user, token);
        //    return await Task.FromResult(JWToken);
        //}

        private List<Claim> GetUserClaims(IUserModel user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("UserId", user.UserName));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Role, user.UserRole));
            claims.Add(new Claim("Location", user.Location));
            claims.Add(new Claim("LocationId", user.locationId.ToString()));

            return claims;
        }

        private void SetSession(IUserModel user, string token)
        {
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.UserRole);
            HttpContext.Session.SetString("Location", user.Location);
            HttpContext.Session.SetString("LocationId", user.locationId.ToString());

            Constants.SetSessionInstance(HttpContext);
        }

        public async Task CreateSignIn(IUserModel user)
        {
            SetSession(user, null);

            var claimsIdentity = new ClaimsIdentity(GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        public async Task CreateSignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public void ClearAllCookies()
        {
            // Delete all cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
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
