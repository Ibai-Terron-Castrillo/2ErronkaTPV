// KategoriaControllerTests.cs
using JatetxeaApi.Controllerrak;
using JatetxeaApi.DTOak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JatetxeaApi.Testak.Controllerrak
{
    public class KategoriaControllerTests
    {
        private readonly Mock<KategoriaRepository> _repoMock;
        private readonly KategoriaController _controller;

        public KategoriaControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<KategoriaRepository>(sessionFactoryMock.Object);
            _controller = new KategoriaController(_repoMock.Object);
        }

        [Fact]
        public void K1_KontsultaOndo_KategoriakItzultzenDu()
        {
            var kategoriak = new List<Kategoria>
            {
                new Kategoria { Id = 1, Izena = "Hasierakoak" },
                new Kategoria { Id = 2, Izena = "Postreak" }
            };

            _repoMock.Setup(r => r.GetAll()).Returns(kategoriak);

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoList = Assert.IsAssignableFrom<IEnumerable<KategoriaDto>>(okResult.Value);
            Assert.Equal(2, dtoList.Count());
        }

        [Fact]
        public void K2_KontsultaSalbuespena_Propagatu()
        {
            _repoMock.Setup(r => r.GetAll()).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.GetAll());
        }

        [Fact]
        public void K3_KategoriaExistitzenDa_Ok()
        {
            var kategoria = new Kategoria { Id = 1, Izena = "Hasierakoak" };
            _repoMock.Setup(r => r.Get(1)).Returns(kategoria);

            var result = _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<KategoriaDto>(okResult.Value);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Hasierakoak", dto.Izena);
        }

        [Fact]
        public void K4_KategoriaEzExistitzen_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Kategoria)null);

            var result = _controller.Get(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void K5_GetSalbuespena_Propagatu()
        {
            _repoMock.Setup(r => r.Get(1)).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Get(1));
        }

        [Fact]
        public void K6_SorreraOndo_Ok()
        {
            var dto = new KategoriaSortuDto
            {
                Izena = "Edariak"
            };

            Kategoria saved = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Kategoria>()))
                .Callback<Kategoria>(k =>
                {
                    k.Id = 5;
                    saved = k;
                });

            var result = _controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Kategoria sortuta", value.mezua);
            Assert.Equal(5, (int)value.id);
            Assert.NotNull(saved);
            Assert.Equal("Edariak", saved.Izena);
        }

        [Fact]
        public void K7_SorreraSalbuespena_Propagatu()
        {
            var dto = new KategoriaSortuDto
            {
                Izena = "Edariak"
            };

            _repoMock.Setup(r => r.Add(It.IsAny<Kategoria>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Sortu(dto));
        }

        [Fact]
        public void K8_KategoriaEzExistitzenEguneratu_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Kategoria)null);
            var dto = new KategoriaSortuDto { Izena = "Berria" };

            var result = _controller.Eguneratu(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void K9_EguneratzeOndo_Ok()
        {
            var kategoria = new Kategoria { Id = 1, Izena = "Hasierakoak" };
            _repoMock.Setup(r => r.Get(1)).Returns(kategoria);

            var dto = new KategoriaSortuDto { Izena = "Plater nagusiak" };

            var result = _controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Eguneratuta", value.mezua);
            Assert.Equal("Plater nagusiak", kategoria.Izena);
            _repoMock.Verify(r => r.Update(kategoria), Times.Once);
        }

        [Fact]
        public void K10_EguneratzeSalbuespena_Propagatu()
        {
            var kategoria = new Kategoria { Id = 1, Izena = "Hasierakoak" };
            _repoMock.Setup(r => r.Get(1)).Returns(kategoria);
            _repoMock.Setup(r => r.Update(It.IsAny<Kategoria>())).Throws(new Exception("Database error"));

            var dto = new KategoriaSortuDto { Izena = "Plater nagusiak" };

            Assert.Throws<Exception>(() => _controller.Eguneratu(1, dto));
        }

        [Fact]
        public void K11_KategoriaEzExistitzenEzabatu_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Kategoria)null);

            var result = _controller.Ezabatu(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void K12_EzabatzeOndo_Ok()
        {
            var kategoria = new Kategoria { Id = 1, Izena = "Hasierakoak" };
            _repoMock.Setup(r => r.Get(1)).Returns(kategoria);

            var result = _controller.Ezabatu(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Ezabatuta", value.mezua);
            _repoMock.Verify(r => r.Delete(kategoria), Times.Once);
        }

        [Fact]
        public void K13_EzabatzeSalbuespena_Propagatu()
        {
            var kategoria = new Kategoria { Id = 1, Izena = "Hasierakoak" };
            _repoMock.Setup(r => r.Get(1)).Returns(kategoria);
            _repoMock.Setup(r => r.Delete(It.IsAny<Kategoria>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Ezabatu(1));
        }
    }
}