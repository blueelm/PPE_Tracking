using PPE_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPE_Tracking.Data
{
    public interface IDataSource
    {
        public Task<Stock> GetStock(string stockID);
        public Task<Tuple<long, long>[]> GetHistory(string URN, DateTime? fromDate, DateTime? thruDate, bool qohUi);

        public Task SaveChanges();
    }
}
