using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PPE_Models
{
    public class Stock
    {
        public static Stock BuildModel(DbDataReader reader)
        {
            return new Stock()
            {
                StockID = reader["StockID"] as string,
                Category = reader["Category"] as string,
                StockNumber = reader["StockNumber"] as string,
                Description = reader["Description"] as string,
                Inventory = reader["Inventory"] as string,
                Location = reader["Location"] as string,
                Manufacturer = reader["ManufacturerDescription"] as string,
                Packaging = reader["Packaging"] as string,
                UnitOfIssue = reader["UnitOfIssue"] as string,
                Vendor = reader["VendorDescription"] as string,
                Quantity = Convert.ToInt64(reader["Quantity"] as int? ?? default(long)),
                QuantityBackordered = Convert.ToInt64(reader["QuantityOnBackorder"] as int? ?? default(long)),
                UnitOfIssueUnitSmallest = Convert.ToInt64(reader["UnitOfIssueUs"] as int? ?? default(long))
            };
        }

        // IJSRuntime doesn't expose JsonSerializerOptions to turn off camelCasing
        [JsonPropertyName("StockID")]
        public string StockID { get; set; }
        [JsonPropertyName("Category")]
        public string Category { get; set; }
        [JsonPropertyName("StockNumber")]
        public string StockNumber { get; set; }
        [JsonPropertyName("Description")]
        public string Description { get; set; }
        [JsonPropertyName("Inventory")]
        public string Inventory { get; set; }
        [JsonPropertyName("Location")]
        public string Location { get; set; }
        [JsonPropertyName("Manufacturer")]
        public string Manufacturer { get; set; }
        [JsonPropertyName("Packaging")]
        public string Packaging { get; set; }
        [JsonPropertyName("UnitOfIssue")]
        public string UnitOfIssue { get; set; }
        [JsonPropertyName("Quantity")]
        public long Quantity { get; set; }
        [JsonPropertyName("QuantityBackordered")]
        public long QuantityBackordered { get; set; }
        [JsonPropertyName("UnitOfIssueUnitSmallest")]
        public long UnitOfIssueUnitSmallest { get; set; }
        [JsonPropertyName("Vendor")]
        public string Vendor { get; set; }

        // Datatables specific attribute
        [NotMapped]
        [JsonPropertyName("DT_RowAttr")]
        public object DataTables_RowAttr { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string[] SearchFields {
            get
            {
                return new string[] { StockID, StockNumber, Description, Inventory, Location, Manufacturer, Vendor };
            }
        }

        [NotMapped]
        [JsonIgnore]
        public object this[string propertyName]
        {
            get
            {
                try
                {
                    PropertyInfo property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                    return property.GetValue(this, null);
                }
                catch
                {
                    return String.Empty;
                }
            }

        }

    }
}



