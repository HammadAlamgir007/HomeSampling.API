namespace Shared.Infrastructure.Services;

public class GuidService : IGuidService
{
    public string NewGuid() => Guid.NewGuid().ToString();
}
