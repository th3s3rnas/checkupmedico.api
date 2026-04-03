namespace CheckupMedico.Application.Service
{
    using CheckupMedico.Application.Service.Interface;
    using CheckupMedico.Domain.Repository.Interface.LocalFile;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class CacheCleaner : ICacheCleaner
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheCleaner> _logger;

        private readonly IRepoLocalFileBillingConfig _repoLocalFileBillingConfig;
        private readonly IRepoLocalFileHospital _repoLocalFileHospital;
        private readonly IRepoLocalFileKit _repoLocalFileKit;

        public CacheCleaner(
            IMemoryCache cache,
            ILogger<CacheCleaner> logger,
            IRepoLocalFileBillingConfig repoLocalFileBillingConfig,
            IRepoLocalFileHospital repoLocalFileHospital,
            IRepoLocalFileKit repoLocalFileKit
            )
        {
            _cache = cache;
            _logger = logger;
            _repoLocalFileBillingConfig = repoLocalFileBillingConfig;
            _repoLocalFileHospital = repoLocalFileHospital;
            _repoLocalFileKit = repoLocalFileKit;
        }

        public void ClearAll()
        {
            if (_cache is MemoryCache memCache)
                memCache.Compact(1.0);

            _repoLocalFileBillingConfig.ClearCache();
            _repoLocalFileHospital.ClearCache();
            _repoLocalFileKit.ClearCache();

            _logger.LogInformation("Se limpió correctamente la memoria caché.");
        }
    }
}
