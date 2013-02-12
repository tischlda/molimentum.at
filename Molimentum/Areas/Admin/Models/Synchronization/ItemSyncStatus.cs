namespace Molimentum.Areas.Admin.Models.Synchronization
{
    public class ItemSyncStatus<T>
    {
        public string ItemId { get; set; }
        public string SourceETag { get; set; }
    }
}
