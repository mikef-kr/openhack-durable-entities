using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using Models;


namespace InventoryManagement
{
    [JsonObject(MemberSerialization.OptIn)]
    public class StoreItemEntity : IStoreItemEntity
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        public Dictionary<string,StoreItems> storeItems;

        [FunctionName("Foo")]
        public static Task counter([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Counter>();

        [FunctionName("ProcessEvents")]

        public Task<MDS> ProcessEvents(ItemEvent itemEvent)
         {
            StoreItems currentStoreItem;
            MDS currentMDS = new MDS();
            if(storeItems == null){
                storeItems = new Dictionary<string, StoreItems>();
            }
            
            if(itemEvent != null){

                string key = $"{itemEvent.divisionId}:{itemEvent.storeId}";

                if(!storeItems.ContainsKey(key)){
                    currentStoreItem = new StoreItems();
                    currentStoreItem.storeId = itemEvent.storeId;
                    currentStoreItem.divisionId = itemEvent.divisionId;
                    currentStoreItem.items = new Dictionary<string,MDS>();
                    storeItems[key] = currentStoreItem;
                }
                currentStoreItem = storeItems[key];
                    
                if(!currentStoreItem.items.ContainsKey(itemEvent.upc)){
                    currentMDS = new MDS();
                    currentMDS.id = $"{itemEvent.divisionId}:{itemEvent.storeId}:{itemEvent.upc}";
                    currentMDS.storeId = itemEvent.storeId;
                    currentMDS.divisionId = itemEvent.divisionId;
                    currentMDS.upc = itemEvent.upc;
                    currentMDS.inventoryCount = 0;
                    currentMDS.description = itemEvent.description;
                    currentMDS.productName = itemEvent.productName;
                    currentMDS.lastUpdateTimestamp = itemEvent.lastUpdateTimestamp;
                    currentMDS.lastShipmentTimestamp = itemEvent.lastShipmentTimestamp;
                    currentStoreItem.items[itemEvent.upc] = currentMDS;
                }
                currentMDS = currentStoreItem.items[itemEvent.upc];

                if(itemEvent.type == "onHandUpdate"){
                    
                    currentMDS.inventoryCount = itemEvent.countAdjustment;
                    currentMDS.description = itemEvent.description;
                    currentMDS.productName = itemEvent.productName;
                    currentMDS.lastUpdateTimestamp = itemEvent.lastUpdateTimestamp;
                    currentMDS.lastShipmentTimestamp = itemEvent.lastShipmentTimestamp;

                }else if (itemEvent.type == "shipmentUpdate"){

                    currentMDS.inventoryCount += itemEvent.countAdjustment;
                    currentMDS.description = itemEvent.description;
                    currentMDS.productName = itemEvent.productName;
                    currentMDS.lastUpdateTimestamp = itemEvent.lastUpdateTimestamp;
                    currentMDS.lastShipmentTimestamp = itemEvent.lastShipmentTimestamp;

                }
            }
            //return currentMDS;
            return Task.FromResult(currentMDS);
         }
    }

}