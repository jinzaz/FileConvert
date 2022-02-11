using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileConvert.Model
{
    public class JwtSettings
    {
        /// <summary>
        /// 听众
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 颁发者密钥
        /// </summary>
        public string SecretKey { get; set; }
    }
}
