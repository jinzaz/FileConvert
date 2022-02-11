using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileConvert.Services
{
    public interface IFileConvertService
    {
        Queue<string> DeleteList { get; set; }
        Task<byte[]> VideoCompress(byte[] bytes);

        Task<byte[]> ImageCompress(byte[] bytes);
    }
}
