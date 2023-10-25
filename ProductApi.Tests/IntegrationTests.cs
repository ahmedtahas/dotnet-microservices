using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductApi;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Linq;
using ProductApi.Data;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models; // Add this line to use the User class

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly IConfiguration _configuration;

    public IntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
    }

    [Fact]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Assume you have a method to get a User object from your database
        User user = GetUserFromDatabase("user1"); 

        // Act
        var token = GetJwtToken(client, user); // Pass the user object to GetJwtToken
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/product/products");
        if(response != null)
        {
            response.EnsureSuccessStatusCode();
            Xunit.Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
        else
        {
            throw new Exception("Response was null");
        }

        // Assert
        Xunit.Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }

    private string GetJwtToken(HttpClient client, User user) // Add User parameter
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt").GetValue<string>("Key")); 
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), // Use user.Username
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            }),
            Expires = DateTime.UtcNow.AddMinutes(30), 
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration.GetSection("Jwt").GetValue<string>("Issuer")
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        Console.WriteLine($"Token: {tokenHandler.WriteToken(token)}");
        Console.WriteLine($"Authorization Header: {client.DefaultRequestHeaders.Authorization}");
        return tokenHandler.WriteToken(token);
    }

    // Assume you have a method to get a User object from your database
    private User GetUserFromDatabase(string username)
    {
        using (var context = new ApplicationDbContext(_configuration))
        {
            return context.Users.SingleOrDefault(u => u.Username == username);
        }
    }
}