using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PPE_Models
{
    public class TrackedStock
    {
        [Key]
        public string StockID { get; set; }
    }
}
