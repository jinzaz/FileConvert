using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileConvert.Model
{
    public class ImageCompressSettings
    {
        /// <summary>
        /// 最小压缩大小
        /// </summary>
        private int _MinFileMB;
        public int MinFileMB
        {
            get { return _MinFileMB; }
            set { _MinFileMB = value * 1024 * 1024; }
        }
        /// <summary>
        /// 执行线程数
        /// </summary>
        private string _threads;
        public string Threads
        {
            get { return _threads; }
            set
            {
                if (!string.IsNullOrEmpty(value)) _threads = $"-threads {value}";
                else _threads = value;
            }
        }

        /// <summary>
        /// 图片质量
        /// </summary>
        private string _quality;
        public string Quality
        {
            get { return _quality; }
            set
            {
                if (!string.IsNullOrEmpty(value)) _quality = $"-q {value}";
                else _quality = value;
            }
        }
    }
}
