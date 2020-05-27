using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class StoreItems
    {
        
        public string divisionId { get; set; }
        public string storeId { get; set; }
        public Dictionary<string, MDS> items { get; set; }        
       
    }
}
