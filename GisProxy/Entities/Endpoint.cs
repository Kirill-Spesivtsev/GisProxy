namespace GisProxy.Entities;

public class Endpoint
{
	public string Id { get; set; } = null!;

	public string Title { get; set; } = null!;
	public int Limit { get; set; }

	public ICollection<UserEndpoint> UserLimits { get; set; } = [];
}
