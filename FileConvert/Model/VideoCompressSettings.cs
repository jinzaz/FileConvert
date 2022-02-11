using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileConvert.Model
{
    public class VideoCompressSettings
    {
        /// <summary>
        /// 最小压缩大小
        /// </summary>
        private int _minFileMB;
        public int MinFileMB {
            get { return _minFileMB; }
            set { _minFileMB = value * 1024 * 1024; } 
        }

        /// <summary>
        /// 视频帧率
        /// </summary>
        private string _fpsSize;
        public string FpsSize {
            get { return _fpsSize; }
            set {
                if (!string.IsNullOrEmpty(value)) _fpsSize = $"-r {value}";
                else _fpsSize = value;
            }
        }

        /// <summary>
        /// 执行线程数
        /// </summary>
        private string _threads;
        public string Threads {
            get { return _threads; }
            set {
                if (!string.IsNullOrEmpty(value)) _threads = $"-threads {value}";
                else _threads = value;
            }
        }
        /// <summary>
        /// 分辨率
        /// </summary>
        private string _resolution;
        public string Resolution {
            get { return _resolution; }
            set {
                if (!string.IsNullOrEmpty(value)) _resolution = $"-s {value}"; 
                else _resolution = value;
            } 
        }

        /// <summary>
        /// 音频采样频率
        /// </summary>
        private string _audioSF;
        public string AudioSF
        {
            get { return _audioSF; }
            set
            {
                if (!string.IsNullOrEmpty(value)) _audioSF = $"-ar {value}";
                else _audioSF = value;
            }
        }

        /// <summary>
        /// 视频码率
        /// </summary>
        private string _videoBits;
        public string VideoBits
        {
            get { return _videoBits; }
            set
            {
                if (!string.IsNullOrEmpty(value)) _videoBits = $"-b {value}";
                else _videoBits = value;
            }
        }
    }
}
