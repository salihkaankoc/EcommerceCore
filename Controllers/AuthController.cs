using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using EcommerceCore.Models;

namespace EcommerceCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly string _connectionString;

        public AuthController()
        {
            _connectionString = "server=localhost;database=yourdatabase;user=youruser;password=yourpassword";
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            try
            {
                if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                {
                    return BadRequest("Kullanıcı adı veya şifre boş olamaz.");
                }

                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT id, username FROM users WHERE username = @username AND password = @password";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@password", user.Password);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();
                                int userId = reader.GetInt32("id");
                                string username = reader.GetString("username");
                                var token = GenerateToken(userId, username);
                                return Ok(new { Token = token });
                            } else
                            {
                                return Unauthorized("Kullanıcı adı veya şifre yanlış.");
                            }
                        }
                    }
                }

               
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private string GenerateToken(int userId, string username)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userId}:{username}:${username}"));
        }
    }
}
