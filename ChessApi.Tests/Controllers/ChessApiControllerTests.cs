using System.Text.Json;
using ChessApi;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace ChessApi.Tests.Controllers;

public class ChessApiControllerTests
{
  protected readonly HttpClient _httpClient;
  protected readonly TestServer _server;

  public ChessApiControllerTests()
  {
    var serverArgs = new string[0];
    _server = new TestServer(ChessApi.Program.CreateWebHostBuilder(serverArgs));
    _httpClient = _server.CreateClient();
  }

  protected long ExtractGameId(string responseString)
  {
    var responseJson = JsonSerializer.Deserialize<dynamic>(responseString);
    return responseJson.GetProperty("id").GetInt64();
  }
}
