using Microsoft.AspNetCore.Mvc;

namespace GisProxy.Models
{
	public class UpdateEndpointRequestModel
	{
		public string EndpointId { get; set; } = null!;
		public int Limit { get; set; }
	}
}
