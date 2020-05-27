using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Models;

namespace InventoryManagement
{
    public static class StoreItemsOrchestration
    {
        [FunctionName("StoreItemsOrchestration")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var eventItem = await context.WaitForExternalEvent<ItemEvent>("StoreItemEvent");
    
            string data = context.GetInput<string>();

            //Dictionary <string,StoreItems> storeItems = new Dictionary<string, StoreItems>();

            var entityId = new EntityId("StoreItems", "myStoreItems");
            var proxy = context.CreateEntityProxy<IStoreItemEntity>(entityId);

            MDS mdsItem = await proxy.ProcessEvents(eventItem);
  
        }

        
    }
}