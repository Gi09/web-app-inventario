﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using StackExchange.Redis;
using web_app_inventario.Model;

namespace web_app_inventario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static ConnectionMultiplexer redis;

        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            string key = "getvalues";
            redis = ConnectionMultiplexer.Connect("localhost:3306");
            IDatabase db = redis.GetDatabase();
            await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
            string user = await db.StringGetAsync(key);

            if (!string.IsNullOrEmpty(user))
            {
                return Ok(user);
            }

            string connectionString = "Server=localhost;Database=sys;User=root;Password=1234;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "select id, nome, preco, qtd_estoque, data_criacao from produtos;";
            var produtos = await connection.QueryAsync<Values>(query);
            string produtosJson = JsonConvert.SerializeObject(produtos);
            await db.StringSetAsync(key, produtosJson);

            return Ok(produtos);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Values produto)
        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=1234;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"insert into produtos(nome, preco, qtd_estoque, data_criacao) 
                            values(@nome, @preco, @qtd_estoque, @data_criacao);";
            await connection.ExecuteAsync(sql, produto);

            //apagar o cachê
            string key = "getvalues";
            redis = ConnectionMultiplexer.Connect("localhost:3306");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Values produto)
        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=1234;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"update produtos 
                            set nome = @nome, 
	                            preco = @preco,
                                qtd_estoque = @qtd_estoque,
                                data_criacao = @data_criacao
                            where id = @id;";

            await connection.ExecuteAsync(sql, produto);

            //apagar o cachê
            string key = "getvalues";
            redis = ConnectionMultiplexer.Connect("localhost:3306");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=1234;";
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"delete from produtos where id = @id;";

            await connection.ExecuteAsync(sql, new { id });

            //apagar o cachê
            string key = "getvalues";
            redis = ConnectionMultiplexer.Connect("localhost:3306");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }
    }
}