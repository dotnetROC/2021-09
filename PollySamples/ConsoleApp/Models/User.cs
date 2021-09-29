﻿using Newtonsoft.Json;

namespace ConsoleApp.Models
{
	public class User
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		/*
		"address": {
            "street": "Kulas Light",
            "suite": "Apt. 556",
            "city": "Gwenborough",
            "zipcode": "92998-3874",
            "geo": {
                "lat": "-37.3159",
                "lng": "81.1496"
            }
        },
        "phone": "1-770-736-8031 x56442",
        "website": "hildegard.org",
        "company": {
            "name": "Romaguera-Crona",
            "catchPhrase": "Multi-layered client-server neural-net",
            "bs": "harness real-time e-markets"
        }
        */

		public override string ToString()
		{
			return $"User #{Id} - {Name} ({Email})";
		}
	}
}
