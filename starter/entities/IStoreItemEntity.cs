namespace InventoryManagement
{
    using System.Threading.Tasks;
    using Models;

    public interface IStoreItemEntity
    {
        Task<MDS> ProcessEvents(ItemEvent itemEvent);
    }
}