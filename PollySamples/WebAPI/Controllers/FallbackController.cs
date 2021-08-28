using Microsoft.AspNetCore.Mvc;

using System;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class FallbackController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			return DateTime.Now.Second % 3 == 0
					? throw new Exception("Random exception!")
					: new OkObjectResult("Success");
		}
	}
}
