using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileConvert.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FileConvert.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GDCAController : ControllerBase
    {
        private readonly List<GDCAInfoModel> _gdcaInfoList;
         
        public GDCAController(IOptions<List<GDCAInfoModel>> gdcainfolist)
        {
           _gdcaInfoList = gdcainfolist.Value;
        }

        /// <summary>
        /// 获取GDCA返回信息
        /// </summary>
        /// <returns></returns>
        [Route("GetGDCAInfo")]
        [HttpGet]
        public IEnumerable<GDCAInfoModel> GetGDCAInfo()
        {
            //JsonSerializerSettings options = new JsonSerializerSettings();
            //options.NullValueHandling = NullValueHandling.Ignore;
            //options.StringEscapeHandling = StringEscapeHandling.Default;
            foreach (var item in _gdcaInfoList)
            {
                 yield return item;
            }
        }

        /// <summary>
        /// 获取GDCA返回信息V2
        /// </summary>
        /// <returns></returns>
        [Route("GetGDCAInfoV2")]
        [HttpGet]
        public  IActionResult GetGDCAInfoV2()
        {
            return new JsonResult(_gdcaInfoList);
        }
    }
}