namespace GisProxy.Entities
{
	public class User
	{
		public string Id { get; set; } = null!;
		public ICollection<UserEndpoint> UserLimits { get; set; } = [];
	}
}
