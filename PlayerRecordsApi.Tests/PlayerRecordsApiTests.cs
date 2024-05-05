using System.Reflection;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PlayerRecordsApi.Models;
using PlayerRecordsApi.Controllers;
using Xunit;

namespace PlayerRecordsApi.Tests;


// Attach [ApiController] attribe
// then don't have to 
// if (! ModelState.IsValid) then return error response ...
 
/*
Modele Binding and Attribute Validate will return 400 automatically
return ValidationProblemDetails
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "|7fb5e16a-4c8f23bbfc974667.",
  "errors": {
    "": [
      "A non-empty request body is required."
    ]
  }
}
https://learn.microsoft.com/zh-tw/aspnet/core/web-api/?view=aspnetcore-8.0#automatic-http-400-responses
https://learn.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.mvc.validationproblemdetails?view=aspnetcore-8.0
Disclaimer: here, not yet implement the test code about this
*/
 
/*
Another exceptiion response is to return
HttpResponseException or HttpResponseMessage or HttpError
, which can also work with ModelState
{
  "Message": "The request is invalid.",
  "ModelState": {
    "item": [
      "Required property 'Name' not found in JSON. Path '', line 1, position 14."
    ],
    "item.Name": [
      "The Name field is required."
    ],
    "item.Price": [
      "The field Price must be between 0 and 999."
    ]
  }
}
https://learn.microsoft.com/zh-tw/aspnet/web-api/overview/error-handling/exception-handling
https://learn.microsoft.com/en-us/dotnet/api/system.web.http.httpresponseexception?view=aspnet-webapi-5.2
https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-8.0
Disclaimer: here, not yet implement the code about this
*/

public class PlayerRecordsApiTests : PrivateTests
{
// test: GetPlayer()
    [Fact]
    public async Task GetPlayer_ReturnsListOfPublicPlayers()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        // Act
        var result = await controller.GetPlayer();
        // Assert
        var okResult = Assert.IsType<ActionResult<IEnumerable<PublicPlayer>>>(result);
        Assert.NotNull(okResult.Value);
        Assert.NotEmpty(okResult.Value);
    }


// test: GetPlayer(long)
    [Fact]
    public async Task GetPlayer_WithValidId()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        long validId = 0;
        // Act
        var result = await controller.GetPlayer(validId);
        // Assert
        var okResult = Assert.IsType<ActionResult<PublicPlayer>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(validId, okResult.Value.Id);
    }


// test: GetPlayer(long)
// invalid condition: Id < 0
    [Fact]
    public async Task GetPlayer_WithInValidId()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        long validId = - 1;
        // Act
        var result = await controller.GetPlayer(validId);


        // Assert
        var errorResult = Assert.IsType<ActionResult<PublicPlayer>>(result);
        Assert.NotNull(errorResult.Result);
        var ms = new ModelStateDictionary();
        ms.AddModelError("Id", $"Id: {validId} < 0");
        var se = new SerializableError(ms);
        Assert.Equal(se["Id"], ((SerializableError)(errorResult.Result as BadRequestObjectResult).Value)["Id"]);
    }


// test: GetPlayer(long)
// invalid condition: player record with Id that cannot found
    [Fact]
    public async Task GetPlayer_WithValidId_NotFound()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        long validId = 9;
        // Act
        var result = await controller.GetPlayer(validId);


        // Assert
        var errorResult = Assert.IsType<ActionResult<PublicPlayer>>(result);
        Assert.NotNull(errorResult.Result);
        Assert.Equal($"Player record with Id: {validId} not exist", (errorResult.Result as NotFoundObjectResult).Value);
    }


// test: PutPlayer(long, PublicPlayer)
    [Fact]
    public async Task PutPlayer_WithValidIdAndPlayer()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        long validId = 0;
        var player = new PublicPlayer(validId, "TestPlayer", gameState.Win, 5, DateTime.Now);
        // Act
        var result = await controller.PutPlayer(validId, player);
        var exam = await controller.GetPlayer(validId);
        
        // Assert
        var okResult = Assert.IsType<ActionResult<PublicPlayer>>(exam);
        Assert.NotNull(okResult.Value);
        Assert.Equal(player.Id, okResult.Value.Id);
        Assert.Equal(player.Name, okResult.Value.Name);
    }


// test: PutPlayer(long, PublicPlayer)
// invalid condition: Id < 0 (not yet implement now)
// invalid condition: player record with Id that cannot found
    [Fact]
    public async Task PutPlayer_WithNonExistedPlayer()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        long validId = 10;
        var player = new PublicPlayer(validId, "TestPlayer", gameState.Win, 5, DateTime.Now);
        // Act
        var result = await controller.PutPlayer(validId, player);
        
        // Assert
        var errorResult = Assert.IsType<NotFoundObjectResult>(result);
    }


// test: PostPlayer
    [Fact]
    public async Task PostPlayer_WithValidPlayer()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        var player = new PublicPlayer(10, "NewPlayer", gameState.Draw, 3, DateTime.Now);
        // Act
        var result = await controller.PostPlayer(player);
        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(PublicPlayer), createdAtActionResult.ActionName);
        var createdPlayer = Assert.IsType<PublicPlayer>(createdAtActionResult.Value);
        Assert.Equal(player.Id, createdPlayer.Id);
    }


// test:  PostPlayer
// invalid condition: new a player with existed Id
// invalid condition: Id < 0 (not yet implement now)
    [Fact]
    public async Task PostPlayer_WithExistedPlayer()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        var player = new PublicPlayer(0, "NewPlayer", gameState.Draw, 3, DateTime.Now);
        // Act
        var result = await controller.PostPlayer(player);
        // Assert
        var errorResult = Assert.IsType<ActionResult<PublicPlayer>>(result);
        Assert.NotNull(errorResult.Result);
        var ms = new ModelStateDictionary();
        ms.AddModelError("Id", $"Player record with Id: {player.Id} already exist");
        var se = new SerializableError(ms);
        Assert.Equal(se["Id"], ((SerializableError)(errorResult.Result as BadRequestObjectResult).Value)["Id"]);
    }
    
// test:  DeletePlayer
    [Fact]
    public async Task DeletePlayer_WithValidId()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        long validId = 0;
        // Act
        var result = await controller.DeletePlayer(validId);
        var exam = await controller.GetPlayer(validId);
        
        // Assert
        var okResult = Assert.IsType<ActionResult<PublicPlayer>>(exam);
        Assert.Null(okResult.Value);
    }


// test:  MarkRecordSecret
    [Fact]
    public async Task MarkRecordSecret_WithValidId()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRESTfulStyleController();
        long validId = 0;
        // Act
        var result = await controller.MarkRecordSecret(validId);
        ConcurrentDictionary<long, Player> records = (ConcurrentDictionary<long, Player>)CallPrivateStaticMethod(typeof(PlayerRecords), "GetRecords", null);
        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(true, records[validId].IsSecret);
    }


// test: GetSummary()
    [Fact]
    public async Task GetSummary_ReturnsListOfPublicPlayers()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRPCStyleController();
        // Act
        var result = await controller.GetSummary();
        // Assert
        var okResult = Assert.IsType<ActionResult<IEnumerable<PublicPlayer>>>(result);
        Assert.NotNull(okResult.Value);
        Assert.NotEmpty(okResult.Value);
    }


// test: GetGetPlayerPointsSummary(string)
    [Fact]
    public async Task GetGetPlayerPointsSummary_WithValidName()
    {
        CallPrivateStaticVoidMethod(typeof(PlayerRecords), "RestoreRecords", null);
        // Arrange
        var controller = new PlayerRecordsRPCStyleController();
        string validName = "Noah";
        // Act
        var result = await controller.GetGetPlayerPointsSummary(validName);
        // Assert
        var okResult = Assert.IsType<ActionResult<int>>(result);
        Assert.True(okResult.Value == 6);
    }
}
