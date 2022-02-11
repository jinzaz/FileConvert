using FileConvert.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileConvert.Common
{
    public class HealthCheck :IHealthCheck
    {
        private readonly VideoCompressSettings _videioCompressSettings;
        private readonly ImageCompressSettings _imageCompressSettings;
        private readonly JwtSettings _jwtSettings;
        private readonly List<OriginWhiteModel> _originWhiteModellist;
        private readonly List<GDCAInfoModel> _gdcaInfoList;

        private Exception exception;
        public HealthCheck(IOptions<VideoCompressSettings> videioOptions, 
                           IOptions<ImageCompressSettings> imageOptions,
                           IOptions<JwtSettings> jwtSettings,
                           IOptions<List<OriginWhiteModel>> originWhiteModelList,
                           IOptions<List<GDCAInfoModel>> gdcainfoList)
        {
            try
            {
                _videioCompressSettings = videioOptions.Value;
                _imageCompressSettings = imageOptions.Value;
                _jwtSettings = jwtSettings.Value;
                _originWhiteModellist = originWhiteModelList.Value;
                _gdcaInfoList = gdcainfoList.Value;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

        }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (exception !=null)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(exception:exception));
            }
            return Task.FromResult(HealthCheckResult.Healthy());
        }

    }
}
