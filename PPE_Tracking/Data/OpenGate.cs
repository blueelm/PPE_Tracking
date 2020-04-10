using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PPE_Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PPE_Tracking.Data
{
    public class OpenGate : IDataSource
    {

        private string _connectionString;
        private string _ogConnectionString;
        private string _commandText;
        private SQLiteDBContext dBContext = new SQLiteDBContext();

        public OpenGate(string connectionString, string ogConnectionString)
        {
            _connectionString = connectionString;
            _ogConnectionString = ogConnectionString;
            _commandText = PPE_Models.QueryFactory.OgQuery;
        }
        public async Task<Stock> GetStock(string stockID)
        {

            using (SqlConnection _conn = new SqlConnection(_connectionString))
            {
                string ogCommand = _commandText;
                var whereClause = $"{_commandText} AND MM.STOCK.urn = URN";
                using (SqlCommand _cmd = new SqlCommand($"EXEC [dbo].[ExecuteOpenGateQuery] @conString = N'{_ogConnectionString}', @sql = N'{whereClause}', @parameters = N'URN={stockID}'",
                    _conn))
                {

                    await _conn.OpenAsync();
                    _cmd.CommandTimeout = 100000;
                    using (var rdr = await _cmd.ExecuteReaderAsync())
                    {
                        await rdr.ReadAsync();
                        return Stock.BuildModel(rdr);
                    }
                }
            }
        }

        public async Task<Tuple<long, long>[]> GetHistory(string URN, DateTime? fromDate, DateTime? thruDate, bool qohUi)
        {

            List<Tuple<long, long>> tuples = new List<Tuple<long, long>>();
            if (!fromDate.HasValue)
                fromDate = DateTime.Now.AddDays(0 - 7);
            if (!thruDate.HasValue)
                thruDate = DateTime.Now;

            if (PPE_Worker_Service.Worker.TrackedStocks.TryGetValue(URN, out var stock))
            {

                var matches = await dBContext.StockHistory.Where(sh => sh.DateTime >= fromDate.Value
                                                             && sh.DateTime <= thruDate.Value
                                                             && sh.StockID == URN).ToListAsync();
                foreach (var match in matches)
                {
                    tuples.Add(new Tuple<long, long>((long)PPE_Worker_Service.Worker.ConvertToUnixTimestamp(match.DateTime), !qohUi ? match.Quantity : (match.Quantity / stock.UnitOfIssueUnitSmallest)));
                }
            }
       
            return tuples.ToArray();
        }
        public async Task SaveChanges()
        {
            var existingStocks = await dBContext.TrackedStocks.ToListAsync();
            foreach (var dbstock in existingStocks)
            {
                if (PPE_Worker_Service.Worker.TrackedStocks.ContainsKey(dbstock.StockID))
                    continue;
                else
                    dBContext.Remove(dbstock);
            }
            foreach (var stock in PPE_Worker_Service.Worker.TrackedStocks.Values)
            {
                if (existingStocks.FirstOrDefault(s => s.StockID == stock.StockID) != null)
                    continue;
                else
                    dBContext.Add(new PPE_Models.TrackedStock() { StockID = stock.StockID });
            }
            await dBContext.SaveChangesAsync();
        }
    }
}

