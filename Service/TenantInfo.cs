namespace Service
{
    public interface ITenantInfo
    {
        string TenantName { get; set; }
    }

    public class TenantInfo : ITenantInfo
    {
        public string TenantName { get; set; }
    }
}
