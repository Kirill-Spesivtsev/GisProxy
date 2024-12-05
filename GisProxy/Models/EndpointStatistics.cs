namespace GisProxy.Models
{
	public class EndpointStatistics
	{
		public string EndpointId { get; set; } = null!;
		public int RequestsLimit { get; set; }
		public int RequestsUsed { get; set; }
	}
}
