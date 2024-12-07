using System.Net;
using System.Web;
using DreamTeamOptimizer.Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace DreamTeamOptimizer.MsEmployee.Tests.Integration.Controller;

public class WishListControllerTests : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _httpClient;

    public WishListControllerTests(WebAppFactory factory)
    {
        var clientOptions = new WebApplicationFactoryClientOptions();
        clientOptions.AllowAutoRedirect = false;

        _httpClient = factory.CreateClient(clientOptions);
    }

    [Theory]
    [InlineData("/api/v1/wishlists", 2)]
    [InlineData("/api/v1/wishlists", 0)]
    public async Task GetWishlist_ShouldReturnWishlist_WhenDesiredEmployeeIdsAreProvided(string url,
        int desiredEmployeeCount)
    {
        // Arrange
        var desiredEmployeeIds = Enumerable.Range(10, desiredEmployeeCount).ToList();
        
        var builder = new UriBuilder();
        builder.Path = url;

        var query = HttpUtility.ParseQueryString(builder.Query);
        desiredEmployeeIds.ForEach(id => query.Add("desiredEmployeeIds", id.ToString()));
        builder.Query = query.ToString();
        
        // Act
        var response = await _httpClient.GetAsync(builder.ToString());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.ToString().Should().Contain("application/json");

        var content = await response.Content.ReadAsStringAsync();
        var wishlist = JsonConvert.DeserializeObject<WishList>(content);

        wishlist.Should().NotBeNull();
        wishlist!.EmployeeId.Should().Be(1);
        wishlist.DesiredEmployees.Should().HaveCount(desiredEmployeeCount);
        wishlist.DesiredEmployees.Should().BeEquivalentTo(desiredEmployeeIds);
    }
}