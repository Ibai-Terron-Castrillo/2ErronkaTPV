// InbentarioaControllerTests.cs
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
    public class InbentarioaControllerTests
    {
        private readonly Mock<InbentarioaRepository> _repoMock;
        private readonly InbentarioaController _controller;

        public InbentarioaControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<InbentarioaRepository>(sessionFactoryMock.Object);
            _controller = new InbentarioaController(_repoMock.Object);
        }

        [Fact]
        public void I1_KontsultaOndo_InbentarioaItzultzenDu()
        {
            var elementuak = new List<Inbentarioa>
            {
                new Inbentarioa
                {
                    Id = 1,
                    Izena = "Arroza",
                    Deskribapena = "Luzea",
                    Kantitatea = 10,
                    NeurriaUnitatea = "kg",
                    StockMinimoa = 2,
                    AzkenEguneratzea = DateTime.Now
                },
                new Inbentarioa
                {
                    Id = 2,
                    Izena = "Esnea",
                    Deskribapena = "Osoa",
                    Kantitatea = 20,
                    NeurriaUnitatea = "l",
                    StockMinimoa = 5,
                    AzkenEguneratzea = DateTime.Now
                }
            };

            _repoMock.Setup(r => r.GetAll()).Returns(elementuak);

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoList = Assert.IsAssignableFrom<IEnumerable<InbentarioaDto>>(okResult.Value);
            Assert.Equal(2, dtoList.Count());
        }

        [Fact]
        public void I2_KontsultaSalbuespena_Propagatu()
        {
            _repoMock.Setup(r => r.GetAll()).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.GetAll());
        }

        [Fact]
        public void I3_SorreraOndo_Ok()
        {
            var dto = new InbentarioaSortuDto
            {
                Izena = "Irina",
                Deskribapena = "Garia",
                Kantitatea = 15,
                NeurriaUnitatea = "kg",
                StockMinimoa = 3
            };

            Inbentarioa saved = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Inbentarioa>()))
                .Callback<Inbentarioa>(e =>
                {
                    e.Id = 7;
                    saved = e;
                });

            var result = _controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Elementua sortuta", value.mezua);
            Assert.Equal(7, (int)value.id);
            Assert.NotNull(saved);
            Assert.Equal("Irina", saved.Izena);
        }

        [Fact]
        public void I4_SorreraSalbuespena_Propagatu()
        {
            var dto = new InbentarioaSortuDto
            {
                Izena = "Irina",
                Deskribapena = "Garia",
                Kantitatea = 15,
                NeurriaUnitatea = "kg",
                StockMinimoa = 3
            };

            _repoMock.Setup(r => r.Add(It.IsAny<Inbentarioa>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Sortu(dto));
        }

        [Fact]
        public void I5_ElementuaExistitzenDa_Ok()
        {
            var elementua = new Inbentarioa
            {
                Id = 1,
                Izena = "Arroza",
                Deskribapena = "Luzea",
                Kantitatea = 10,
                NeurriaUnitatea = "kg",
                StockMinimoa = 2,
                AzkenEguneratzea = DateTime.Now
            };

            _repoMock.Setup(r => r.Get(1)).Returns(elementua);

            var result = _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<InbentarioaDto>(okResult.Value);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Arroza", dto.Izena);
        }

        [Fact]
        public void I6_ElementuaEzExistitzen_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Inbentarioa)null);

            var result = _controller.Get(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void I7_GetSalbuespena_Propagatu()
        {
            _repoMock.Setup(r => r.Get(1)).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Get(1));
        }

        [Fact]
        public void I8_ElementuaEzExistitzenEguneratu_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Inbentarioa)null);
            var dto = new InbentarioaSortuDto();

            var result = _controller.Eguneratu(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void I9_EguneratzeOndo_Ok()
        {
            var elementua = new Inbentarioa
            {
                Id = 1,
                Izena = "Arroza",
                Deskribapena = "Luzea",
                Kantitatea = 10,
                NeurriaUnitatea = "kg",
                StockMinimoa = 2,
                AzkenEguneratzea = DateTime.Now
            };

            _repoMock.Setup(r => r.Get(1)).Returns(elementua);

            var dto = new InbentarioaSortuDto
            {
                Izena = "Arroza berria",
                Deskribapena = "Integrala",
                Kantitatea = 25,
                NeurriaUnitatea = "kg",
                StockMinimoa = 6
            };

            var result = _controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Eguneratuta", value.mezua);
            Assert.Equal("Arroza berria", elementua.Izena);
            Assert.Equal(25, elementua.Kantitatea);
            _repoMock.Verify(r => r.Update(elementua), Times.Once);
        }

        [Fact]
        public void I10_EguneratzeSalbuespena_Propagatu()
        {
            var elementua = new Inbentarioa
            {
                Id = 1,
                Izena = "Arroza",
                Deskribapena = "Luzea",
                Kantitatea = 10,
                NeurriaUnitatea = "kg",
                StockMinimoa = 2,
                AzkenEguneratzea = DateTime.Now
            };

            _repoMock.Setup(r => r.Get(1)).Returns(elementua);
            _repoMock.Setup(r => r.Update(It.IsAny<Inbentarioa>())).Throws(new Exception("Database error"));

            var dto = new InbentarioaSortuDto
            {
                Izena = "Arroza berria",
                Deskribapena = "Integrala",
                Kantitatea = 25,
                NeurriaUnitatea = "kg",
                StockMinimoa = 6
            };

            Assert.Throws<Exception>(() => _controller.Eguneratu(1, dto));
        }

        [Fact]
        public void I11_ElementuaEzExistitzenPatch_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Inbentarioa)null);
            var dto = new InbentarioaPatchDto();

            var result = _controller.EguneratuZatia(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void I12_PatchPartzialaOndo_Ok()
        {
            var elementua = new Inbentarioa
            {
                Id = 1,
                Izena = "Arroza",
                Deskribapena = "Luzea",
                Kantitatea = 10,
                NeurriaUnitatea = "kg",
                StockMinimoa = 2,
                AzkenEguneratzea = DateTime.Now.AddDays(-2)
            };

            _repoMock.Setup(r => r.Get(1)).Returns(elementua);

            var dto = new InbentarioaPatchDto
            {
                Izena = "Arroza berria",
                Kantitatea = 30
            };

            var result = _controller.EguneratuZatia(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Zati batean eguneratuta", value.mezua);
            Assert.Equal("Arroza berria", elementua.Izena);
            Assert.Equal(30, elementua.Kantitatea);
            _repoMock.Verify(r => r.Update(elementua), Times.Once);
        }

        [Fact]
        public void I13_PatchBalioAldaketarikGabe_Ok()
        {
            var dataZaharra = DateTime.Now.AddDays(-5);
            var elementua = new Inbentarioa
            {
                Id = 1,
                Izena = "Arroza",
                Deskribapena = "Luzea",
                Kantitatea = 10,
                NeurriaUnitatea = "kg",
                StockMinimoa = 2,
                AzkenEguneratzea = dataZaharra
            };

            _repoMock.Setup(r => r.Get(1)).Returns(elementua);

            var dto = new InbentarioaPatchDto();

            var result = _controller.EguneratuZatia(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Zati batean eguneratuta", value.mezua);
            Assert.True(elementua.AzkenEguneratzea > dataZaharra);
            _repoMock.Verify(r => r.Update(elementua), Times.Once);
        }

        [Fact]
        public void I14_PatchSalbuespena_Propagatu()
        {
            var elementua = new Inbentarioa
            {
                Id = 1,
                Izena = "Arroza",
                Deskribapena = "Luzea",
                Kantitatea = 10,
                NeurriaUnitatea = "kg",
                StockMinimoa = 2,
                AzkenEguneratzea = DateTime.Now
            };

            _repoMock.Setup(r => r.Get(1)).Returns(elementua);
            _repoMock.Setup(r => r.Update(It.IsAny<Inbentarioa>())).Throws(new Exception("Database error"));

            var dto = new InbentarioaPatchDto
            {
                Izena = "Arroza berria"
            };

            Assert.Throws<Exception>(() => _controller.EguneratuZatia(1, dto));
        }

        [Fact]
        public void I15_ElementuaEzExistitzenEzabatu_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Inbentarioa)null);

            var result = _controller.Ezabatu(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void I16_EzabatzeOndo_Ok()
        {
            var elementua = new Inbentarioa
            {
                Id = 1,
                Izena = "Arroza",
                Deskribapena = "Luzea",
                Kantitatea = 10,
                NeurriaUnitatea = "kg",
                StockMinimoa = 2,
                AzkenEguneratzea = DateTime.Now
            };

            _repoMock.Setup(r => r.Get(1)).Returns(elementua);

            var result = _controller.Ezabatu(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Ezabatuta", value.mezua);
            _repoMock.Verify(r => r.Delete(elementua), Times.Once);
        }

        [Fact]
        public void I17_EzabatzeSalbuespena_Propagatu()
        {
            var elementua = new Inbentarioa
            {
                Id = 1,
                Izena = "Arroza",
                Deskribapena = "Luzea",
                Kantitatea = 10,
                NeurriaUnitatea = "kg",
                StockMinimoa = 2,
                AzkenEguneratzea = DateTime.Now
            };

            _repoMock.Setup(r => r.Get(1)).Returns(elementua);
            _repoMock.Setup(r => r.Delete(It.IsAny<Inbentarioa>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Ezabatu(1));
        }
    }
}