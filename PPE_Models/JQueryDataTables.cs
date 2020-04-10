using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PPE_Models
{
    // server-side objects for datatables jquery plugin
    // https://datatables.net/manual/server-side

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataTablesOrderDirection { ASC, DESC }

    public class DataTablesResult<T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<T> data { get; set; }
    }

    public abstract class DataTablesRow
    {
        public virtual string DT_RowId
        {
            get { return null; }
        }

        public virtual string DT_RowClass
        {
            get { return null; }
        }

        public virtual string DT_RowData
        {
            get { return null; }
        }
    }

    public class DataTablesParameters
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }

        public DataTablesColumn[] Columns { get; set; }
        public DataTablesOrder[] Order { get; set; }

        public DataTablesSearch Search { get; set; }

        public string SortOrder
        {
            get
            {
                if (Columns != null && Order != null && Order.Length > 0)
                {
                    return String.Concat(Columns[Order[0].Column].Data,
                            (Order[0].Dir == DataTablesOrderDirection.DESC) ? " " + Order[0].Dir : string.Empty);
                }
                return null;
            }
        }
    }

    public class DataTablesColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTablesSearch Search { get; set; }
    }

    public class DataTablesOrder
    {
        public int Column { get; set; }
        public DataTablesOrderDirection Dir { get; set; }
    }

    public class DataTablesSearch
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }


}
