// ErreserbakControllerTests.cs
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
    public class ErreserbakControllerTests
    {
        private readonly Mock<ErreserbakRepository> _repoMock;
        private readonly Mock<ZerbitzuakRepository> _zerbitzuRepoMock;
        private readonly ErreserbakController _controller;

        public ErreserbakControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<ErreserbakRepository>(sessionFactoryMock.Object);
            _zerbitzuRepoMock = new Mock<ZerbitzuakRepository>(sessionFactoryMock.Object);
            _controller = new ErreserbakController(_repoMock.Object, _zerbitzuRepoMock.Object);
        }

        [Fact]
        public void E1_KontsultaOndo_ErreserbakItzultzenDu()
        {
            var erreserbak = new List<Erreserbak>
            {
                new Erreserbak
                {
                    Id = 1,
                    MahaiaId = 2,
                    Izena = "Ane",
                    Telefonoa = 123456789,
                    ErreserbaData = DateTime.Now,
                    PertsonaKop = 4,
                    Egoera = "Itxaropean",
                    Oharrak = "Leiho ondoan"
                },
                new Erreserbak
                {
                    Id = 2,
                    MahaiaId = 3,
                    Izena = "Iker",
                    Telefonoa = 987654321,
                    ErreserbaData = DateTime.Now,
                    PertsonaKop = 2,
                    Egoera = "Baieztatua",
                    Oharrak = "Ez du oharrik"
                }
            };

            _repoMock.Setup(r => r.GetAll()).Returns(erreserbak);

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoList = Assert.IsAssignableFrom<IEnumerable<ErreserbakDto>>(okResult.Value);
            Assert.Equal(2, dtoList.Count());
        }

        [Fact]
        public void E2_KontsultaSalbuespena_Propagatu()
        {
            _repoMock.Setup(r => r.GetAll()).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.GetAll());
        }

        [Fact]
        public void E3_ErreserbaExistitzenDa_Ok()
        {
            var erreserba = new Erreserbak
            {
                Id = 1,
                MahaiaId = 2,
                Izena = "Ane",
                Telefonoa = 123456789,
                ErreserbaData = DateTime.Now,
                PertsonaKop = 4,
                Egoera = "Itxaropean",
                Oharrak = "Leiho ondoan"
            };

            _repoMock.Setup(r => r.Get(1)).Returns(erreserba);

            var result = _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ErreserbakDto>(okResult.Value);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Ane", dto.Izena);
        }

        [Fact]
        public void E4_ErreserbaEzExistitzen_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Erreserbak)null);

            var result = _controller.Get(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void E5_GetSalbuespena_Propagatu()
        {
            _repoMock.Setup(r => r.Get(1)).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Get(1));
        }

        [Fact]
        public void E6_SorreraOndo_Ok()
        {
            var dto = new ErreserbakSortuDto
            {
                MahaiaId = 5,
                Izena = "Mikel",
                Telefonoa = 111222333,
                ErreserbaData = DateTime.Now.AddDays(1),
                PertsonaKop = 6,
                Egoera = "Itxaropean",
                Oharrak = "Urtebetetzea"
            };

            Erreserbak savedErreserba = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Erreserbak>()))
                .Callback<Erreserbak>(e =>
                {
                    e.Id = 10;
                    savedErreserba = e;
                });

            var result = _controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Erreserba sortuta", value.mezua);
            Assert.Equal(10, (int)value.id);
            Assert.NotNull(savedErreserba);
            Assert.Equal("Mikel", savedErreserba.Izena);
        }

        [Fact]
        public void E7_SorreraSalbuespena_Propagatu()
        {
            var dto = new ErreserbakSortuDto
            {
                MahaiaId = 5,
                Izena = "Mikel",
                Telefonoa = 111222333,
                ErreserbaData = DateTime.Now.AddDays(1),
                PertsonaKop = 6,
                Egoera = "Itxaropean",
                Oharrak = "Urtebetetzea"
            };

            _repoMock.Setup(r => r.Add(It.IsAny<Erreserbak>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Sortu(dto));
        }

        [Fact]
        public void E8_ErreserbaEzExistitzenEguneratu_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Erreserbak)null);

            var dto = new ErreserbakSortuDto
            {
                MahaiaId = 4,
                Izena = "Ane",
                Telefonoa = 123123123,
                ErreserbaData = DateTime.Now,
                PertsonaKop = 2,
                Egoera = "Baieztatua",
                Oharrak = "Proba"
            };

            var result = _controller.Eguneratu(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void E9_EguneratzeOndo_Ok()
        {
            var erreserba = new Erreserbak
            {
                Id = 1,
                MahaiaId = 2,
                Izena = "Ane",
                Telefonoa = 123456789,
                ErreserbaData = DateTime.Now,
                PertsonaKop = 4,
                Egoera = "Itxaropean",
                Oharrak = "Leiho ondoan"
            };

            _repoMock.Setup(r => r.Get(1)).Returns(erreserba);

            var dto = new ErreserbakSortuDto
            {
                MahaiaId = 9,
                Izena = "Aneberri",
                Telefonoa = 999888777,
                ErreserbaData = DateTime.Now.AddDays(2),
                PertsonaKop = 8,
                Egoera = "Baieztatua",
                Oharrak = "Aldatuta"
            };

            var result = _controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Eguneratuta", value.mezua);
            Assert.Equal("Aneberri", erreserba.Izena);
            Assert.Equal(9, erreserba.MahaiaId);
            _repoMock.Verify(r => r.Update(erreserba), Times.Once);
        }

        [Fact]
        public void E10_EguneratzeSalbuespena_Propagatu()
        {
            var erreserba = new Erreserbak
            {
                Id = 1,
                MahaiaId = 2,
                Izena = "Ane",
                Telefonoa = 123456789,
                ErreserbaData = DateTime.Now,
                PertsonaKop = 4,
                Egoera = "Itxaropean",
                Oharrak = "Leiho ondoan"
            };

            _repoMock.Setup(r => r.Get(1)).Returns(erreserba);
            _repoMock.Setup(r => r.Update(It.IsAny<Erreserbak>())).Throws(new Exception("Database error"));

            var dto = new ErreserbakSortuDto
            {
                MahaiaId = 9,
                Izena = "Aneberri",
                Telefonoa = 999888777,
                ErreserbaData = DateTime.Now.AddDays(2),
                PertsonaKop = 8,
                Egoera = "Baieztatua",
                Oharrak = "Aldatuta"
            };

            Assert.Throws<Exception>(() => _controller.Eguneratu(1, dto));
        }

        [Fact]
        public void E11_ErreserbaEzExistitzenEzabatu_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Erreserbak)null);

            var result = _controller.Ezabatu(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void E12_EzabatuLoturarikGabe_Ok()
        {
            var erreserba = new Erreserbak
            {
                Id = 1,
                MahaiaId = 2,
                Izena = "Ane",
                Telefonoa = 123456789,
                ErreserbaData = DateTime.Now,
                PertsonaKop = 4,
                Egoera = "Itxaropean",
                Oharrak = "Leiho ondoan"
            };

            _repoMock.Setup(r => r.Get(1)).Returns(erreserba);
            _zerbitzuRepoMock.Setup(r => r.GetAll()).Returns(new List<Zerbitzuak>());

            var result = _controller.Ezabatu(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Ezabatuta", value.mezua);
            _zerbitzuRepoMock.Verify(r => r.Update(It.IsAny<Zerbitzuak>()), Times.Never);
            _repoMock.Verify(r => r.Delete(erreserba), Times.Once);
        }

        [Fact]
        public void E13_EzabatuLoturakGarbituta_Ok()
        {
            var erreserba = new Erreserbak
            {
                Id = 1,
                MahaiaId = 2,
                Izena = "Ane",
                Telefonoa = 123456789,
                ErreserbaData = DateTime.Now,
                PertsonaKop = 4,
                Egoera = "Itxaropean",
                Oharrak = "Leiho ondoan"
            };

            var zerbitzuak = new List<Zerbitzuak>
            {
                new Zerbitzuak
                {
                    Id = 10,
                    LangileId = 1,
                    MahaiaId = 2,
                    ErreserbaId = 1,
                    EskaeraData = DateTime.Now,
                    Egoera = "Irekita",
                    Guztira = 50
                },
                new Zerbitzuak
                {
                    Id = 11,
                    LangileId = 2,
                    MahaiaId = 3,
                    ErreserbaId = 1,
                    EskaeraData = DateTime.Now,
                    Egoera = "Irekita",
                    Guztira = 25
                }
            };

            _repoMock.Setup(r => r.Get(1)).Returns(erreserba);
            _zerbitzuRepoMock.Setup(r => r.GetAll()).Returns(zerbitzuak);

            var result = _controller.Ezabatu(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Ezabatuta", value.mezua);
            Assert.All(zerbitzuak, z => Assert.Null(z.ErreserbaId));
            _zerbitzuRepoMock.Verify(r => r.Update(It.IsAny<Zerbitzuak>()), Times.Exactly(2));
            _repoMock.Verify(r => r.Delete(erreserba), Times.Once);
        }

        [Fact]
        public void E14_EzabatzeSalbuespena_Propagatu()
        {
            var erreserba = new Erreserbak
            {
                Id = 1,
                MahaiaId = 2,
                Izena = "Ane",
                Telefonoa = 123456789,
                ErreserbaData = DateTime.Now,
                PertsonaKop = 4,
                Egoera = "Itxaropean",
                Oharrak = "Leiho ondoan"
            };

            var zerbitzuak = new List<Zerbitzuak>
            {
                new Zerbitzuak
                {
                    Id = 10,
                    LangileId = 1,
                    MahaiaId = 2,
                    ErreserbaId = 1,
                    EskaeraData = DateTime.Now,
                    Egoera = "Irekita",
                    Guztira = 50
                }
            };

            _repoMock.Setup(r => r.Get(1)).Returns(erreserba);
            _zerbitzuRepoMock.Setup(r => r.GetAll()).Returns(zerbitzuak);
            _zerbitzuRepoMock.Setup(r => r.Update(It.IsAny<Zerbitzuak>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Ezabatu(1));
        }
    }
}