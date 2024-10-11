
using Moq;
using web_app_domain;
using web_app_repository;

namespace UnitTeste1
{
    public class ValuesRepositoryTest
    {
        [Fact]
        public async Task ListarValues()
        {
            var produtos = new List<Values>()
            {
                new Values()
                {
                    id = 1,
                    nome = "Giovanna",
                    preco = 22,
                    qtd_estoque = 2,
                    data_criacao = "hoje"
                },
                new Values()
                {
                    id = 2,
                    nome = "Ilan",
                    preco = 20,
                    qtd_estoque = 1,
                    data_criacao = "hoje"
                }
            };

            var userRepositoryMock = new Mock<IValueRepository>();
            userRepositoryMock.Setup(u => u.ListValues()).ReturnsAsync(produtos);
            var userRepository = userRepositoryMock.Object;

            //Act
            var result = await userRepository.ListValues();

            //Assert
            Assert.Equal(produtos, result);
        }

        [Fact]
        public async Task SaveValues()
        {
            var produtos = new Values()
            {
                id = 3,
                nome = "FIAP",
                preco = 21,
                qtd_estoque = 3,
                data_criacao = "hoje"
            };
            var userRepositoryMock = new Mock<IValueRepository>();
            userRepositoryMock.Setup(u => u.SaveValues(It.IsAny<Values>())).Returns(Task.CompletedTask);
            var userRepository = userRepositoryMock.Object;

            //Act
            await userRepository.SaveValues(produtos);

            //Assert
            userRepositoryMock.Verify(u => u.SaveValues(It.IsAny<Values>()), Times.Once);
        }
    }
}
