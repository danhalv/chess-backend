using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using ChessApi.Models;
using ChessApi.Models.Chess;
using Xunit;

namespace ChessApi.Tests.Controllers;

public class ChessApiHttpControllerTests : ChessApiControllerTests
{
  public ChessApiHttpControllerTests()
    : base()
  { }

  [Fact]
  public async Task CreateChessGame_Returns_Created_Game()
  {
    // Act
    var response = await _httpClient.PostAsync("/api/chessgames", null);

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var responseString = await response.Content.ReadAsStringAsync();
    Assert.Contains("id", responseString);
  }

  [Fact]
  public async Task GetChessGame_Returns_Game_With_Id()
  {
    // Arrange: Create new game
    var createResponse = await _httpClient.PostAsync("/api/chessgames/", null);

    // Assert: Game is created
    Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
    var createResponseContentString =
      await createResponse.Content.ReadAsStringAsync();
    var gameId = ExtractGameId(createResponseContentString);

    // Act: Retrieve game by id
    var getResponse = await _httpClient.GetAsync($"/api/chessgames/{gameId}");

    // Assert: Created game is retrieved
    getResponse.EnsureSuccessStatusCode();
    var getResponseContentString =
      await getResponse.Content.ReadAsStringAsync();
    Assert.Contains("id", getResponseContentString);
    Assert.Contains("turn", getResponseContentString);
    Assert.Contains("moves", getResponseContentString);
    var retrievedGameId = ExtractGameId(getResponseContentString);
    Assert.Equal(retrievedGameId, gameId);
  }

  [Fact]
  public async Task GetChessGames_Returns_All_Games()
  {
    // Arrange: Create new games
    var createResponse1 = await _httpClient.PostAsync("/api/chessgames/", null);
    var createResponse2 = await _httpClient.PostAsync("/api/chessgames/", null);

    // Assert: Games are created
    Assert.Equal(HttpStatusCode.Created, createResponse1.StatusCode);
    Assert.Equal(HttpStatusCode.Created, createResponse2.StatusCode);
    var createResponse1ContentString =
      await createResponse1.Content.ReadAsStringAsync();
    var createResponse2ContentString =
      await createResponse2.Content.ReadAsStringAsync();

    // Act: Retrieve all games (including the created ones above)
    var getResponse = await _httpClient.GetAsync("/api/chessgames/");

    // Assert: All games are retrieved
    var getResponseContentString =
      await getResponse.Content.ReadAsStringAsync();
    var getResponseJson =
      JsonSerializer.Deserialize<dynamic>(getResponseContentString);
    Assert.Equal(JsonValueKind.Array, getResponseJson.ValueKind);
    Assert.Contains(createResponse1ContentString, getResponseContentString);
    Assert.Contains(createResponse2ContentString, getResponseContentString);
  }
}
