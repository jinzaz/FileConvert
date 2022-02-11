using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileConvert.Common
{
    public class JsonResultModel
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public int result { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 版本信息
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { get; set; }

    }
}
