using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;
using web_app_domain;
using web_app_repository;
using RabbitMQ.Client;
using System.Text;

namespace web_app_inventario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static ConnectionMultiplexer redis;
        private readonly IValueRepository _repository;

        public ValuesController(IValueRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            //string key = "getvalues";
            //redis = ConnectionMultiplexer.Connect("localhost:3306");
            //IDatabase db = redis.GetDatabase();
            //await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
            //string user = await db.StringGetAsync(key);

            //if (!string.IsNullOrEmpty(user))
            //{
            //    return Ok(user);
            //}

            var produto = await _repository.ListValues();
            if(produto == null)
                return NotFound();

            string produtosJson = JsonConvert.SerializeObject(produto);
            //await db.StringSetAsync(key, produtosJson);
            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Values produto)
        {
            await _repository.SaveValues(produto);

            //apagar o cachê
            //string key = "getvalues";
            //redis = ConnectionMultiplexer.Connect("localhost:3306");
            //IDatabase db = redis.GetDatabase();
            //await db.KeyDeleteAsync(key);

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            const string fila = "produto_cadastrado";

            channel.QueueDeclare(queue: fila,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Criando a mensagem JSON
            //var mensagemJson = JsonSerializer.Serialize(new
            //{
            //produtoId = produto.id,
            //nomeProduto = produto.nome,
            //quantidadeInicial = produto.quantidadeInicial
            //});
            string mensagem = $"Mensagem:" +
                $" Produto: ;" +
                $"Nome: ;" +
                $"Quantidade:";

            var body = Encoding.UTF8.GetBytes(mensagem);

            // Publicando a mensagem
            channel.BasicPublish(exchange: "",
                                 routingKey: fila,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($" ");

            Console.WriteLine("Mensagem postada com sucesso!");

            return Ok(new { mensagem = "Criado com sucesso!" });
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Values produto)
        {
            await _repository.UpdateValues(produto);

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
            await _repository.DeleteValues(id);

            //apagar o cachê
            string key = "getvalues";
            redis = ConnectionMultiplexer.Connect("localhost:3306");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }
    }
}