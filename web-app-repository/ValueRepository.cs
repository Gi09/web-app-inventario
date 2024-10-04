using Dapper;
using MySqlConnector;
using web_app_domain;

namespace web_app_repository
{
    public class ValueRepository : IValueRepository
    {
        private readonly MySqlConnection mySqlConnection;
        public ValueRepository() 
        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=1234;";
            mySqlConnection = new MySqlConnection(connectionString);
        }

        public async Task<IEnumerable<Values>> ListValues()
        {
            await mySqlConnection.OpenAsync();
            string query = "select id, nome, preco, qtd_estoque, data_criacao from produtos;";
            var produtos = await mySqlConnection.QueryAsync<Values>(query);
            await mySqlConnection.CloseAsync();

            return produtos;
        }

        public async Task SaveValues(Values produtos)
        {
            await mySqlConnection.OpenAsync();
            string sql = @"insert into produtos(nome, preco, qtd_estoque, data_criacao) 
                            values(@nome, @preco, @qtd_estoque, @data_criacao);";
            await mySqlConnection.ExecuteAsync(sql, produtos);
            await mySqlConnection.CloseAsync();
        }

        public async Task UpdateValues(Values produtos)
        {
            await mySqlConnection.OpenAsync();
            string sql = @"update produtos 
                            set nome = @nome, 
	                            preco = @preco,
                                qtd_estoque = @qtd_estoque,
                                data_criacao = @data_criacao
                            where id = @id;";

            await mySqlConnection.ExecuteAsync(sql, produtos);
            await mySqlConnection.CloseAsync();
        }

        public async Task DeleteValues(int id)
        {
            await mySqlConnection.OpenAsync();
            string sql = @"delete from produtos where id = @id;";
            await mySqlConnection.ExecuteAsync(sql, new { id });
            await mySqlConnection.CloseAsync();

        }
    }
}
