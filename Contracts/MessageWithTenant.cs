namespace Contracts;

public interface IMessageWithTenant
{
    int OrderId { get; }
    string Tenant { get; }
}
