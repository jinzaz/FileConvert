using FileConvert.Services;
using Microsoft.Extensions.Logging;
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
		private readonly ILogger<TimedExecuteService> _logger;
		private readonly IFileConvertService _fileConvertService;

		public TimedExecuteService(ILogger<TimedExecuteService> logger, IFileConvertService fileConvertService)
		{
			_logger = logger;
			_fileConvertService = fileConvertService;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
			try
			{
				_logger.LogInformation(DateTime.Now.ToString() + "BackgroundService启动");
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
							_logger.LogError(ex, DateTime.Now.ToString() + "DeleteAllFile：异常");
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (!stoppingToken.IsCancellationRequested)
				{
					_logger.LogError(ex, DateTime.Now.ToString() + "BackgroundService：异常");
				}
				else
				{
					
					_logger.LogError(ex, DateTime.Now.ToString() + "BackgroundService：停止");
				}
				throw ex;
			}
        }
    }
}
