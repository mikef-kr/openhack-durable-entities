using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Models;
using Microsoft.Azure.Cosmos;
using System;

namespace InventoryManagement
{
    public static class StoreItemDBWriter
    {

        private static Container destinationContainer = null;
        private static Database database = null;
        
        [FunctionName("DBWriter")]
        public static async Task DBWriter([ActivityTrigger] IDurableActivityContext dbcontext)
        {
            MDS mdsItem = dbcontext.GetInput<MDS>();
           
            using (CosmosClient  client = new CosmosClient(Environment.GetEnvironmentVariable("cosmoskimopenhack001_DOCUMENTDB",
            EnvironmentVariableTarget.Process)))
                {
                     database = await client.CreateDatabaseIfNotExistsAsync("inventory");

                    // Create with a throughput of 1000 RU/s
                     destinationContainer = database.GetContainer("mds");

                    ItemResponse<MDS> response = await destinationContainer.UpsertItemAsync(
                    partitionKey: new PartitionKey(mdsItem.id),
                    item: mdsItem);
                
                    
               
               }

               //return "done"; 
        }


}
}
