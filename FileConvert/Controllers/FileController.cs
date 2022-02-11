using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileConvert.Model;
using FileConvert.ffmpeg;
using FileConvert.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Exceptionless;
using FileConvert.Common;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;
using System.Buffers;
using System.Threading;
using FileConvert.Services;
using Microsoft.AspNetCore.Authentication;

namespace FileConvert.Controllers
{
    [Route("[controller]")]
    [DisableRequestSizeLimit]
    [Authorize]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly VideoCompressSettings _videioCompressSettings;
        private readonly ImageCompressSettings _imageCompressSettings;
        private readonly ILoggerHelper _logger;
        private readonly IFileConvertService _fileConvertService;

        public FileController(IOptions<VideoCompressSettings> videioOptions, 
            IOptions<ImageCompressSettings> imageOptions, 
            ILoggerHelper loggerHelper,
            IFileConvertService fileConvertService)
        {
            _videioCompressSettings = videioOptions.Value;
            _imageCompressSettings = imageOptions.Value;
            _logger = loggerHelper;
            _fileConvertService = fileConvertService;
        }


        /// <summary>
        /// 视频压缩（raw-Body形式）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("text/plain")]
        [Route("VideoCompress")]
        public async Task<IActionResult> VideoCompress([FromBody] string videoBase64)
        {
            try
            {
                CancellationToken cancellationToken = HttpContext.RequestAborted;
                if (!cancellationToken.IsCancellationRequested)
                {
                    byte[] videoBytes;
                    try
                    {
                        videoBytes = Convert.FromBase64String(videoBase64);
                    }
                    catch (FormatException ex)
                    {
                        videoBase64 = videoBase64.Base64Filter();
                        videoBytes = Convert.FromBase64String(videoBase64);
                    }
                    if (videoBytes.Length <= _videioCompressSettings.MinFileMB)
                    {
                        videoBytes = null;
                        return Ok(videoBase64);
                    }
                    var fileString = await _fileConvertService.VideoCompress(videoBytes);
                    ExceptionlessClient.Default.SubmitLog(typeof(Program).FullName, $"接口：VideoCompress调用成功 Source：{HttpContext.Request.Headers["Origin"]}");
                    return Ok(Convert.ToBase64String(fileString));
                }
                throw new OperationCanceledException("客户端手动取消");
            }
            catch (Exception ex)
            {
                _logger.Error(typeof(FileController), $"请求异常 Source{HttpContext.Request.Headers["Origin"]}", ex);
                ex.ToExceptionless().Submit();
                return Ok(videoBase64);
            }
            finally
            {
                videoBase64 = "";
            }
        }

        /// <summary>
        /// 视频压缩（from-data形式）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("VideoCompressByForm")]
        public async Task<IActionResult> VideoCompressByForm()
        {
            byte[] videoBytes = null;
            try
            {
                var files = Request.Form.Files;
                Stream fileStream = files[0].OpenReadStream();
                videoBytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(videoBytes, 0, (int)fileStream.Length);
                await fileStream.DisposeAsync();
                if (videoBytes.Length <= _videioCompressSettings.MinFileMB)
                {
                    return Ok(Convert.ToBase64String(videoBytes));
                }
                var fileString = await _fileConvertService.VideoCompress(videoBytes);
                return Ok(Convert.ToBase64String(fileString));
            }
            catch (Exception ex)
            {
                _logger.Error(typeof(FileController), "请求异常", ex);
                ex.ToExceptionless().Submit();
                return Ok(videoBytes);
            }
        }


        /// <summary>
        /// 图片压缩（raw-body形式）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("text/plain")]
        [Route("ImageCompress")]
        public async Task<IActionResult> ImageCompress([FromBody] string imageBase64)
        {
            try
            {
                CancellationToken cancellationToken = HttpContext.RequestAborted;
                if(!cancellationToken.IsCancellationRequested)
                {
                    byte[] imageBytes;
                    try
                    {
                        imageBytes = Convert.FromBase64String(imageBase64);
                    }
                    catch (FormatException ex)
                    {
                        imageBase64 = imageBase64.Base64Filter();
                        imageBytes = Convert.FromBase64String(imageBase64);
                    }
                    if (imageBytes.Length <= _imageCompressSettings.MinFileMB)
                    {
                        return Ok(new JsonResultModel { result = 0, msg = "图片小于1M" });
                    }
                    var fileString = await _fileConvertService.ImageCompress(imageBytes);
                    ExceptionlessClient.Default.SubmitLog(typeof(Program).FullName, $"接口：ImageCompress调用成功 Source：{HttpContext.Request.Headers["Origin"]}");
                    return Ok(Convert.ToBase64String(fileString));
                }
                throw new OperationCanceledException("客户端手动取消");
            }
            catch (Exception ex)
            {
                _logger.Error(typeof(FileController), "请求异常", ex);
                ex.ToExceptionless().Submit();
                return Ok(new JsonResultModel { result = -1, msg = "请求异常" });
            }
            finally
            {
                imageBase64 = "";
            }
        }

        /// <summary>
        /// 图片压缩（from-data形式）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("ImageCompressByForm")]
        public async Task<IActionResult> ImageCompressByForm()
        {
            byte[] videoBytes = null;
            try
            {
                var files = Request.Form.Files;
                Stream fileStream = files[0].OpenReadStream();
                videoBytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(videoBytes, 0, (int)fileStream.Length);
                await fileStream.DisposeAsync();
                if (videoBytes.Length <= _imageCompressSettings.MinFileMB)
                {
                    return Ok(new JsonResultModel { result = 0, msg = "图片小于1M" });
                }
                var fileString = await _fileConvertService.ImageCompress(videoBytes);
                ExceptionlessClient.Default.SubmitLog(typeof(Program).FullName, $"接口：ImageCompressByForm调用成功 Source：{HttpContext.Request.Headers["Origin"]}");
                return Ok(Convert.ToBase64String(fileString));
            }
            catch (Exception ex)
            {
                _logger.Error(typeof(FileController), "请求异常", ex);
                ex.ToExceptionless().Submit();
                return Ok(new JsonResultModel { result = -1, msg = "请求异常" });
            }
        }

        #region 通用方法


        #endregion
    }
}