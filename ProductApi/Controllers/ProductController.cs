using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using ProductApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ProductApi.Controllers
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly IConfiguration _configuration;

        public ProductController(ProductContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            if (_context.Products.Count() == 0)
            {
                // Add a default product if the database is empty
                _context.Products.Add(new Product { Name = "Product1", Description = "This is product 1", Price = 9.99m });
                _context.SaveChanges();
            }
        }

        // public ProductController(ProductContext context)
        // {
        //     _context = context;

        //     if (_context.Products.Count() == 0)
        //     {
        //         // Add a default product if the database is empty
        //         _context.Products.Add(new Product { Name = "Product1", Description = "This is product 1", Price = 9.99m });
        //         _context.SaveChanges();
        //     }
        // }

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _context.Products.ToList();
        }

        [HttpPost]
        public ActionResult<Product> Post(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult GetToken([FromBody] LoginModel login)
        {
            // This is a simple example. Do not store passwords in plaintext in a real application.
            if (login.Username == "admin" && login.Password == "password")
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, login.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                    _configuration["Jwt:Issuer"],
                    claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return BadRequest("Invalid username or password.");
        }

    }
}