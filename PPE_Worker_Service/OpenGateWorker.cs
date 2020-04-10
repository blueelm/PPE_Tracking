using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using PPE_Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPE_Worker_Service
{
    public class OpenGateWorker : Worker
    {
        private string _ogConnectionString = string.Empty;
        private SQLiteDBContext dBContext = new SQLiteDBContext();

        public OpenGateWorker(ILogger<Worker> logger) : base(logger)
        {

        }

        public OpenGateWorker(ILogger<Worker> logger, string connectionString, int refresh) : base(logger, connectionString, refresh)
        {

        }

        public OpenGateWorker(ILogger<Worker> logger, string connectionString, int refresh, string ogConnectionString) : base(logger, connectionString, refresh)
        {
            _ogConnectionString = ogConnectionString;
            _commandText = PPE_Models.QueryFactory.OgQuery;
        }

        protected override async Task InitialDictionaryLoad()
        {
            try
            {
                using (SqlConnection _conn = new SqlConnection(_connectionString))
                {
                    string ogCommand = _commandText;

                    using (SqlCommand _cmd = new SqlCommand($"EXEC [dbo].[ExecuteOpenGateQuery] @conString = N'{_ogConnectionString}', @sql = N'{ogCommand}', @parameters = NULL",
                        _conn))
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

        protected override async Task GatherData(ConcurrentDictionary<string, Stock> stocks)
        {
            using (SqlConnection _conn = new SqlConnection(_connectionString))
            {
                string ogCommand = _commandText;
                StringBuilder commandBuilder = new StringBuilder();
                commandBuilder.AppendLine(@"INITIALIZE { O(/S,""P""),");
                foreach (var stock in TrackedStocks.Keys)
                {
                    commandBuilder.AppendLine($".^/STOCKS[\"{stock}\"],");
                }
                commandBuilder.AppendLine("}");
                commandBuilder.Append(ogCommand);
                commandBuilder.Append("EXIT { O(/U) }");
                ogCommand = commandBuilder.ToString().Replace("FROM MM.STOCK.main", "FROM /STOCKS[ms] JOIN MM.STOCK.main");
                using (SqlCommand _cmd = new SqlCommand($"EXEC [dbo].[ExecuteOpenGateQuery] @conString = N'{_ogConnectionString}', @sql = N'{ogCommand}', @parameters = NULL",
                    _conn))
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
