using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace team01.onhandChangefeed
{
    public static class CosmosDBTriggerShipment
    {
        [FunctionName("CosmosDBTriggerShipment")]
        
        public static void Run([CosmosDBTrigger(
            databaseName: "inventory",
            collectionName: "shipment",
            ConnectionStringSetting = "cosmoskimopenhack001_DOCUMENTDB",
            CreateLeaseCollectionIfNotExists  = true,
            LeaseCollectionName = "leasesShipment")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                //raise event to orchestrator arg(input.count)
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
