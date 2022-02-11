using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FileConvert.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FileConvert.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly List<OriginWhiteModel> _originWhiteModellist;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthorizeController(IOptions<JwtSettings> jwtSettings,
                                   IOptions<List<OriginWhiteModel>> originWhiteModellist,
                                   IHttpContextAccessor httpContextAccessor)
        {
            _jwtSettings = jwtSettings.Value;
            _originWhiteModellist = originWhiteModellist.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="OrgCode">机构号</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetToken")]
        public  IActionResult GetToken(string OrgCode)
        {
            var data = _originWhiteModellist.Where(x => x.OrgCodeList.IndexOf(OrgCode.ToUpper()) > -1 && _httpContextAccessor.HttpContext.Request.Headers["Origin"].FirstOrDefault().Contains(x.Origin)).FirstOrDefault();
            if (data == null)
            { return Ok(new { token = "",expiration = "" }); }
            //创建claim
            var authClaims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,OrgCode),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };
            IdentityModelEventSource.ShowPII = true;
            //签名秘钥 可以放到json文件中
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var token = new JwtSecurityToken(
                   issuer: _jwtSettings.Issuer,
                   audience: _jwtSettings.Audience,
                   expires: DateTime.Now.AddMinutes(30),
                   claims: authClaims,
                   signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                   );

            //返回token和过期时间
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}