using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

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

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, args) =>
{
    var body = args.Body.ToArray();
    var mensagemJson = Encoding.UTF8.GetString(body);

    // Desserializando a mensagem
    var produto = JsonSerializer.Deserialize<Produto>(mensagemJson);

    // Atualizando o inventário
    if (produto != null)
    {
        Console.WriteLine($"Produto cadastrado: {produto.nomeProduto}, Quantidade inicial: {produto.quantidadeInicial}");
    }
};

channel.BasicConsume(queue: fila,
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine("Aguardando mensagem");
Console.ReadLine();

 //Definição da classe Produto
public class Produto
{
    public string produtoId { get; set; }
    public string nomeProduto { get; set; }
    public int quantidadeInicial { get; set; }
}
