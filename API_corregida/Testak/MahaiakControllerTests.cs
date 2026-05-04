using JatetxeaApi.Controllerrak;

using JatetxeaApi.DTOak;

using JatetxeaApi.Modeloak;

using JatetxeaApi.Repositorioak;

using Microsoft.AspNetCore.Mvc;

using Moq;

using NHibernate;

using System.Collections.Generic;

using Xunit;

using NHSession = NHibernate.ISession;

using NHSessionFactory = NHibernate.ISessionFactory;
 
namespace GastuakApi.Testak

{

    public class MahaiakControllerTests

    {

        private readonly Mock<NHSessionFactory> _sessionFactoryMock;

        private readonly Mock<NHSession> _sessionMock;

        private readonly Mock<MahaiakRepository> _mahaiakRepoMock;

        private readonly Mock<ErreserbakRepository> _erreserbakRepoMock;

        private readonly MahaiakController _controller;
 
        public MahaiakControllerTests()

        {

            _sessionFactoryMock = new Mock<NHSessionFactory>();

            _sessionMock = new Mock<NHSession>();
 
            _sessionFactoryMock

                .Setup(x => x.GetCurrentSession())

                .Returns(_sessionMock.Object);
 
            _mahaiakRepoMock = new Mock<MahaiakRepository>(_sessionFactoryMock.Object);

            _erreserbakRepoMock = new Mock<ErreserbakRepository>(_sessionFactoryMock.Object);
 
            _controller = new MahaiakController(_mahaiakRepoMock.Object, _erreserbakRepoMock.Object);

        }
 
        [Fact]

        public void GetAll_Ondo_Ok()

        {

            var mahaiak = new List<Mahaiak>

            {

                new Mahaiak(1, 4, "Libre"),

                new Mahaiak(2, 6, "Okupatuta")

            };
 
            _mahaiakRepoMock.Setup(x => x.GetAll()).Returns(mahaiak);
 
            var emaitza = _controller.GetAll();
 
            var okResult = Assert.IsType<OkObjectResult>(emaitza);

            Assert.NotNull(okResult.Value);

        }
 
        [Fact]

        public void Get_IdExistitzenDa_Ok()

        {

            var mahaia = new Mahaiak(1, 4, "Libre");
 
            _mahaiakRepoMock.Setup(x => x.Get(1)).Returns(mahaia);
 
            var emaitza = _controller.Get(1);
 
            var okResult = Assert.IsType<OkObjectResult>(emaitza);

            Assert.NotNull(okResult.Value);

        }
 
        [Fact]

        public void Get_IdEzDaExistitzen_NotFound()

        {

            _mahaiakRepoMock.Setup(x => x.Get(1)).Returns((Mahaiak?)null);
 
            var emaitza = _controller.Get(1);
 
            Assert.IsType<NotFoundObjectResult>(emaitza);

        }
 
        [Fact]

        public void Sortu_Ondo_Ok()

        {

            var dto = new MahaiakSortuDto

            {

                MahaiaZbk = 3,

                Edukiera = 4,

                Egoera = "Libre"

            };
 
            var emaitza = _controller.Sortu(dto);
 
            _mahaiakRepoMock.Verify(x => x.Add(It.IsAny<Mahaiak>()), Times.Once);

            Assert.IsType<OkObjectResult>(emaitza);

        }
 
        [Fact]

        public void Eguneratu_IdExistitzenDa_Ok()

        {

            var mahaia = new Mahaiak(1, 4, "Libre");
 
            var dto = new MahaiakSortuDto

            {

                MahaiaZbk = 5,

                Edukiera = 6,

                Egoera = "Okupatuta"

            };
 
            _mahaiakRepoMock.Setup(x => x.Get(1)).Returns(mahaia);
 
            var emaitza = _controller.Eguneratu(1, dto);
 
            _mahaiakRepoMock.Verify(x => x.Update(It.IsAny<Mahaiak>()), Times.Once);

            Assert.IsType<OkObjectResult>(emaitza);

        }
 
        [Fact]

        public void Eguneratu_IdEzDaExistitzen_NotFound()

        {

            var dto = new MahaiakSortuDto

            {

                MahaiaZbk = 5,

                Edukiera = 6,

                Egoera = "Okupatuta"

            };
 
            _mahaiakRepoMock.Setup(x => x.Get(1)).Returns((Mahaiak?)null);
 
            var emaitza = _controller.Eguneratu(1, dto);
 
            Assert.IsType<NotFoundObjectResult>(emaitza);

        }
 
        [Fact]

        public void Ezabatu_IdExistitzenDa_Ok()

        {

            var mahaia = new Mahaiak(1, 4, "Libre");
 
            _mahaiakRepoMock.Setup(x => x.Get(1)).Returns(mahaia);
 
            var emaitza = _controller.Ezabatu(1);
 
            _mahaiakRepoMock.Verify(x => x.Delete(It.IsAny<Mahaiak>()), Times.Once);

            Assert.IsType<OkObjectResult>(emaitza);

        }
 
        [Fact]

        public void Ezabatu_IdEzDaExistitzen_NotFound()

        {

            _mahaiakRepoMock.Setup(x => x.Get(1)).Returns((Mahaiak?)null);
 
            var emaitza = _controller.Ezabatu(1);
 
            Assert.IsType<NotFoundObjectResult>(emaitza);

        }

    }

}
 