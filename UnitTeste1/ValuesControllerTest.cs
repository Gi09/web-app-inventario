using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using web_app_domain;
using web_app_inventario.Controllers;
using web_app_repository;

namespace UnitTeste1
{
    public class ValuesControllerTest
    {
        private readonly Mock<IValueRepository> _userRepositoryMock;
        private readonly ValuesController _controller;

        public ValuesControllerTest()
        {
            _userRepositoryMock = new Mock<IValueRepository>();
            _controller = new ValuesController(_userRepositoryMock.Object );
        }
        [Fact]
        public async Task Get_ListarValuesOk()
        {
            //arrange
            var produtos = new List<Values>()
            {
                new Values()
                {
                    id = 1,
                    nome = "Giovanna",
                    preco = 22,
                    qtd_estoque = 2,
                    data_criacao = "hoje"
                }
            };
            _userRepositoryMock.Setup(r => r.ListValues()).ReturnsAsync(produtos);


            //Act
            var result = await _controller.GetValues();

            //Asserts
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(JsonConvert.SerializeObject(produtos), JsonConvert.SerializeObject(okResult.Value));
        }

        [Fact]
        public async Task GetListarRetornaNotFound()
        {
            _userRepositoryMock.Setup(u => u.ListValues()).ReturnsAsync((IEnumerable<Values>)null);

            var result = await _controller.GetValues();

            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
        }

        public async Task PostSalvaValues()
        {
            // arrange
            var produtos = new Values()
             {
                id = 1,
                nome = "Giovanna",
                preco = 22,
                qtd_estoque = 2,
                data_criacao = "hoje"
            };
            _userRepositoryMock.Setup(u => u.SaveValues(It.IsAny<Values>())).Returns(Task.CompletedTask);

            //Act
            var result = await _controller.Post(produtos);

            //Asserts
            _userRepositoryMock.Verify(u =>  u.SaveValues(It.IsAny<Values>()), Times.Once);
            Assert.IsType<OkResult>(result);
        }
    }
}
