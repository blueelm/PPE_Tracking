using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PPE_Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PPE_Worker_Service
{
    public class Worker : BackgroundService
    {
        protected readonly ILogger<Worker> _logger;
        private int _refreshInterval = 3600000;
        private SQLiteDBContext dBContext = new SQLiteDBContext();

        protected string _connectionString;
        protected string _commandText;

        public static ConcurrentDictionary<string, Stock> TrackedStocks = new ConcurrentDictionary<string, Stock>();
        public static Dictionary<string, Stock> AllStocks = new Dictionary<string, Stock>();
        public static DateTime LastUpdated = DateTime.Now;


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _connectionString = string.Empty;
        }

        public Worker(ILogger<Worker> logger, string connectionString, int refresh)
        {
            _logger = logger;
            _connectionString = connectionString;
            _refreshInterval = refresh;
            _commandText = PPE_Models.QueryFactory.SqlQuery;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrEmpty(_connectionString))
                return;

            dBContext.Database.Migrate();
            await InitialDictionaryLoad();
            await DoWork(stoppingToken);

        }

        protected virtual async Task InitialDictionaryLoad()
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand _cmd = new SqlCommand(_commandText, _conn))
                    {

                        await _conn.OpenAsync();
                        _cmd.CommandTimeout = 100000;
                        using (var reader = await _cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var stock = Stock.BuildModel(reader);
                                AllStocks.TryAdd(stock.StockID, stock);
                            }
                        }
                    }
                }
                var tracked = await dBContext.TrackedStocks.ToListAsync();
                foreach (var trackedStock in tracked)
                {
                    if (AllStocks.TryGetValue(trackedStock.StockID, out var stock))
                        TrackedStocks.TryAdd(stock.StockID, stock);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{nameof(ex)}: {ex.Message}");
            }

        }

        private async Task DoWork(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    if (TrackedStocks.Count > 0)
                    {
                        ConcurrentDictionary<string, Stock> stocks = new ConcurrentDictionary<string, Stock>();
                        await GatherData(stocks);

                        LastUpdated = DateTime.Now;
                        TrackedStocks = stocks;

                        using (var transaction = dBContext.Database.BeginTransaction())
                        {
                            foreach (var stock in TrackedStocks.Values)
                            {
                                dBContext.StockHistory.Add(new StockHistory() { StockID = stock.StockID, Quantity = stock.Quantity, DateTime = LastUpdated });
                            }
                            await dBContext.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"{nameof(ex)}: {ex.Message}");
                }

                await Task.Delay(_refreshInterval, stoppingToken);
            }
        }



        protected virtual async Task GatherData(ConcurrentDictionary<string, Stock> stocks)
        {
            using (SqlConnection _conn = new SqlConnection(_connectionString))
            {
                var stockIds = String.Join(',', TrackedStocks.Keys.Select(i => $"'{i}'"));
                var whereClause = $"{_commandText} AND StockID IN({stockIds})";
                using (SqlCommand _cmd = new SqlCommand(whereClause, _conn))
                {
                    await _conn.OpenAsync();
                    _cmd.CommandTimeout = 100000;
                    using (var reader = await _cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var stock = Stock.BuildModel(reader);
                            stocks.TryAdd(stock.StockID, stock);
                        }
                    }
                }
            }
        }


    }
}
