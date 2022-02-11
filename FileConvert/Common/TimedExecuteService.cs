using FileConvert.Controllers;
using FileConvert.Log;
using FileConvert.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FileConvert.Common
{
    public class TimedExecuteService :MyBackgroundService
    {
		private readonly ILoggerHelper _logger;
		private readonly IFileConvertService _fileConvertService;

		public TimedExecuteService(ILoggerHelper loggerHelper, IFileConvertService fileConvertService)
		{
			_logger = loggerHelper;
			_fileConvertService = fileConvertService;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
			try
			{
				_logger.Debug(typeof(TimedExecuteService),DateTime.Now.ToString() + "BackgroundService启动");
				while (!stoppingToken.IsCancellationRequested)
				{
					await Task.Delay(5000, stoppingToken);
					if (_fileConvertService.DeleteList != null && _fileConvertService.DeleteList.Count > 0)
					{
						try
						{
							FileUpdate.DeleteAllFile(_fileConvertService.DeleteList.Dequeue());
						}
						catch (Exception ex)
						{
							_logger.Error(typeof(TimedExecuteService), DateTime.Now.ToString() + "DeleteAllFile：异常", ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (!stoppingToken.IsCancellationRequested)
				{
					_logger.Error(typeof(TimedExecuteService),DateTime.Now.ToString() + "BackgroundService：异常" ,ex);
				}
				else
				{
					
					_logger.Error(typeof(TimedExecuteService),DateTime.Now.ToString() + "BackgroundService：停止", ex);
				}
				throw ex;
			}
        }
    }
}
