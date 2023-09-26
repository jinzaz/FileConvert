using FileConvert.ffmpeg;
using FileConvert.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileConvert.Services
{
    public class FileConvertService :IFileConvertService
    {
        private readonly VideoCompressSettings _videioCompressSettings;
        private readonly ImageCompressSettings _imageCompressSettings;
        private readonly ILogger<FileConvertService> _logger;   
        private readonly string workPath = AppDomain.CurrentDomain.BaseDirectory;

        private static Queue<string> _deleteList;
        public Queue<string> DeleteList { get { return _deleteList; } set { _deleteList = value; } }



        public FileConvertService(IOptions<VideoCompressSettings> videioOptions, IOptions<ImageCompressSettings> imageOptions, ILogger<FileConvertService> logger)
        {
            if (DeleteList == null)
            {
                DeleteList = new Queue<string>();
            }
            _videioCompressSettings = videioOptions.Value;
            _imageCompressSettings = imageOptions.Value;
            _logger = logger;
        }

        public async Task<byte[]> VideoCompress(byte[] bytes)
        {
            var filePath = $"{workPath}\\TempFiles\\{Guid.NewGuid().ToString("N")}\\";
            var oldpath = filePath + "video1.mp4";
            var newpath = filePath + "video2.mp4";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var file = new FileStream(oldpath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough);
            await file.WriteAsync(bytes, 0, bytes.Length);
            file.Close();
            await file.DisposeAsync();
            ffmpegHelper.ConvertVideo(oldpath,newpath,_videioCompressSettings.FpsSize, _videioCompressSettings.Threads, _videioCompressSettings.Resolution, _videioCompressSettings.AudioSF,_videioCompressSettings.VideoBits);
            DeleteList.Enqueue(filePath);
            return  await File.ReadAllBytesAsync(newpath);
        }

        public async Task<byte[]> ImageCompress(byte[] bytes)
        {
            var filePath = $"{workPath}\\TempFiles\\{Guid.NewGuid().ToString("N")}\\";
            var oldpath = filePath + "img1.jpg";
            var newpath = filePath + "img2.jpg";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var file = new FileStream(oldpath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough);
            await file.WriteAsync(bytes, 0, bytes.Length);
            file.Close();
            await file.DisposeAsync();
            ffmpegHelper.ConvertImage(oldpath, newpath, _imageCompressSettings.Threads, _imageCompressSettings.Quality);
            DeleteList.Enqueue(filePath);
            return await File.ReadAllBytesAsync(newpath);
        }
    }
}
