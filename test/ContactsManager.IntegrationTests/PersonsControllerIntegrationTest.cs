using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

namespace CRUDTests;

public class PersonsControllerIntegrationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Index_ToReturnView()
    {
        //Act
        HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK); //200 OK

        string responseBody = await response.Content.ReadAsStringAsync();

        var html = new HtmlDocument();
        html.LoadHtml(responseBody);
        var document = html.DocumentNode;

        document.QuerySelectorAll("table.persons").Should().NotBeNull();
    }
}
