namespace CheckupMedico.Infrastructure.Repository.LocalFile.Base
{
    using CheckupMedico.Domain.Repository.Interface.LocalFile.Base;
    using ClosedXML.Excel;
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Collections.Generic;

    public abstract class RepoLocalBase<TEntity> : IRepoLocalFile
    {
        private readonly IMemoryCache _cache; 
        private readonly string _filePath; 
        private readonly string _cacheKey;

        protected RepoLocalBase(IMemoryCache cache, string relativePath) 
        { 
            _cache = cache; 
            _filePath = Path.Combine(AppContext.BaseDirectory, relativePath); 
            _cacheKey = $"{typeof(TEntity).Name}_excel_cache"; 
        }

        protected List<TEntity> Load(Func<IXLRow, TEntity> map)
        {
            if (_cache.TryGetValue(_cacheKey, out List<TEntity> cached))
                return cached;

            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Archivo de excel no encontrado: {_filePath}");

            using var workbook = new XLWorkbook(_filePath);
            var worksheet = workbook.Worksheet(1);

            var data = worksheet
                .RowsUsed()
                .Skip(1)
                .Select(map)
                .ToList();

            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(12),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };

            _cache.Set(_cacheKey, data, options);

            return data;
        }

        public void ClearCache() => _cache.Remove(_cacheKey);
    }
}
