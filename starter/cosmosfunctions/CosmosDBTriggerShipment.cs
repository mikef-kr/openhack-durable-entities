using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace team01.onhandChangefeed
{
    public static class CosmosDBTriggerShipment
    {
        [FunctionName("CosmosDBTriggerShipment")]
        
        public static async Task Run([CosmosDBTrigger(
            databaseName: "inventory",
            collectionName: "shipments",
            ConnectionStringSetting = "cosmoskimopenhack001_DOCUMENTDB",
            CreateLeaseCollectionIfNotExists  = true,
            LeaseCollectionName = "leasesShipment")]IReadOnlyList<Document> input, ILogger log, 
            [DurableClient] IDurableOrchestrationClient starter)
        {
            if (input != null && input.Count > 0)
            {
                foreach (var doc in input)
                {
                    //Raise Event to Orchestrator
                    ItemEvent itemEvent = new ItemEvent();
                    var item = JsonConvert.DeserializeObject<Shipment>(doc.ToString());
                    
                    itemEvent.divisionId = item.divisionId;
                    itemEvent.storeId = item.storeId;
                    itemEvent.type = "shipmentUpdate";
                    itemEvent.lastUpdateTimestamp = item.arrivalTimestamp;
                    itemEvent.lastShipmentTimestamp = item.arrivalTimestamp;

                    foreach (var rec in item.ToString())
                    {
                    var itemEnum = JsonConvert.DeserializeObject<ShipmentItem>(rec.ToString());
                    itemEvent.id = ($"{item.divisionId}:{item.storeId}:{itemEnum.upc}");
                    itemEvent.upc = itemEnum.upc;
                    itemEvent.countAdjustment = itemEnum.shipmentAmount;
                   
                    // Function input comes from the request content.
                    string instanceId = await starter.StartNewAsync<ItemEvent>("StoreItemsOrchestration", null, itemEvent).ConfigureAwait(false);
                    }

                    log.LogInformation("First Shipping document Id " + item.id);
                    log.LogInformation("Documents modified " + input.Count);
                    log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
}
