using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EcommerceCore.Models;

namespace EcommerceCore.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase {
        private readonly string _connectionString;

        public ProductController() {
            _connectionString = "server=localhost;database=yourdatabase;user=youruser;password=yourpassword";
        }
        
        

        [HttpGet]
        [Route("get-products")]
        public async Task<IActionResult> GetProducts() {
            try {
                using (var conn = new MySqlConnection(_connectionString)) {
                    await conn.OpenAsync();
                    string query = "SELECT * FROM products";
                    using (var cmd = new MySqlCommand(query, conn)) {
                        using (var reader = await cmd.ExecuteReaderAsync()) {
                            var products = new List<Product>();
                            while (await reader.ReadAsync()) {
                                var product = new Product {
                                    ProductId = Convert.ToInt32(reader["product_id"]),
                                    ProductCode = reader["product_code"].ToString(),
                                    ProductName = reader["product_name"].ToString(),
                                    Price = Convert.ToDecimal(reader["price"]),
                                    StockQuantity = Convert.ToInt32(reader["stockquantity"]),
                                    CreatedAt = Convert.ToDateTime(reader["createAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["updateAt"]),
                                    Description = reader["description"].ToString(),
                                    CategoryId = Convert.ToInt32(reader["category_id"]),
                                    Brand = reader["brand"].ToString(),
                                    ImageUrl = reader["image_url"].ToString()
                                };
                                products.Add(product);
                            }
                            return Ok(products);
                        }
                    }
                }
            } catch (Exception error) {
                return StatusCode(500, error.Message);
                
            }
        }
    }
}
