namespace GisProxy.Entities
{
	public class UserEndpoint
	{
		public string UserId { get; set; } = null!;
		public User User { get; set; } = null!;
		public string EndpointId { get; set; } = null!;
		public Endpoint Endpoint { get; set; } = null!;
		public int RequestsUsed { get; set; }
	}
}
