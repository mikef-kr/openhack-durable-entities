using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace team01.onhandChangefeed
{
    public static class CosmosDBTrigger
    {
        [FunctionName("CosmosDBTrigger")]

        public static async Task Run([CosmosDBTrigger(
            databaseName: "inventory",
            collectionName: "onHand",
            ConnectionStringSetting = "cosmoskimopenhack001_DOCUMENTDB",
            CreateLeaseCollectionIfNotExists  = true,
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log,
            [DurableClient] IDurableOrchestrationClient starter)
        {
            if (input != null && input.Count > 0)
            {
                //raise event to orchestrator
                log.LogInformation("Documents OnHand modified " + input.Count);
                foreach (var doc in input)
                {
                    ItemEvent itemEvent = new ItemEvent();
                    var item = JsonConvert.DeserializeObject<OnHand>(doc.ToString());

                    itemEvent.id = ($"{item.divisionId}:{item.storeId}:{item.upc}");
                    itemEvent.divisionId = item.divisionId;
                    itemEvent.storeId = item.storeId;
                    itemEvent.upc = item.upc;
                    itemEvent.countAdjustment = item.inventoryCount;
                    itemEvent.type = "onHandUpdate";
                    itemEvent.description = item.description;
                    itemEvent.productName = item.productName;
                    itemEvent.lastUpdateTimestamp = item.lastUpdateTimestamp;
                   // itemEvent.lastShipmentTimestamp = null;


                    // Function input comes from the request content.
                    string instanceId = await starter.StartNewAsync<ItemEvent>("StoreItemsOrchestration", null, itemEvent).ConfigureAwait(false);
   
                    //send it itemEvent
                    //await client.RaiseEventAsync("0", "StoreItemEvent", itemEvent);
                    //client.RaiseEventAsync("0", "StoreItemEvent", itemEvent);
                     log.LogInformation("First OnHand document Id " + item.id);

                     
             //       var mgmtUrls = client.CreateHttpManagementPayload(instanceId);
              //      log.LogInformation(inputItem.Id, mgmtUrls.ToString());
                   
                }


               
            }
        }
    }
}