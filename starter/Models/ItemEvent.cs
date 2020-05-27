using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ItemEvent
    {
        public string id { get; set; }
        public string divisionId { get; set; }
        public string storeId { get; set; }
        public string upc { get; set; }
        public int countAdjustment { get; set; }
        public string type { get; set; }
        public string productName { get; set; }
        public string description { get; set; }
        public DateTime lastShipmentTimestamp { get; set; }
        public DateTime lastUpdateTimestamp { get; set; }
    }
}
