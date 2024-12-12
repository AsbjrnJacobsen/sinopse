using System;
using System.Net.Http.Headers;

HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

System.Console.WriteLine("before");
var response = await client.GetAsync("http://orderservice:8082/test/get");
System.Console.WriteLine("after");

Console.WriteLine(response.StatusCode);
string rawIp = await response.Content.ReadAsStringAsync();
string newIp = rawIp.Trim('\"'); // Remove double quotes

Console.WriteLine($"Received IP: {newIp}");

HttpClient client2 = new HttpClient();
client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

System.Console.WriteLine("before");
var response2 = await client2.GetAsync($"http://{newIp}:8082/test/get");
System.Console.WriteLine("after");

Console.WriteLine(response2.StatusCode);
Console.WriteLine(await response2.Content.ReadAsStringAsync());