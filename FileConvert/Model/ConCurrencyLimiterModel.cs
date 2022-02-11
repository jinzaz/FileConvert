using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileConvert.Model
{
    public class ConCurrencyLimiterModel
    {
        /// <summary>
        /// 最大并发请求数
        /// </summary>
        public int MaxConcurrentRequests { get; set; }

        /// <summary>
        /// 请求队列长度限制
        /// </summary>
        public int RequestQueueLimit { get; set; }

        /// <summary>
        /// 限制接口列表
        /// </summary>
        public List<string> RequestQueueForActions { get; set; }
    }
}
