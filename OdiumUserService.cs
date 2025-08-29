using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Odium.Odium;

public class OdiumUserService
{
	private readonly HttpClient _httpClient;

	private readonly string _baseUrl;

	public OdiumUserService(HttpClient httpClient, string baseUrl)
	{
		_httpClient = httpClient;
		_baseUrl = baseUrl;
	}

	public async Task<int> GetUserCountAsync()
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "/api/odium/users/list");
			if (response.IsSuccessStatusCode)
			{
				return AssignedVariables.odiumUsersCount = JsonConvert.DeserializeObject<List<object>>(await response.Content.ReadAsStringAsync())?.Count ?? 0;
			}
			Console.WriteLine($"Error: HTTP {response.StatusCode} - {response.ReasonPhrase}");
			AssignedVariables.odiumUsersCount = 0;
			return 0;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Console.WriteLine("Exception occurred while fetching user count: " + ex2.Message);
			AssignedVariables.odiumUsersCount = 0;
			return 0;
		}
	}
}
