// JatetxekoInfoControllerTests.cs
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
    public class JatetxekoInfoControllerTests
    {
        private readonly Mock<JatetxekoInfoRepository> _repoMock;
        private readonly JatetxekoInfoController _controller;

        public JatetxekoInfoControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<JatetxekoInfoRepository>(sessionFactoryMock.Object);
            _controller = new JatetxekoInfoController(_repoMock.Object);
        }

        [Fact]
        public void J1_KontsultaOndo_InfoItzultzenDu()
        {
            var infoList = new List<JatetxekoInfo>
            {
                new JatetxekoInfo
                {
                    Id = 1,
                    Izena = "Jatetxea 1",
                    KaxaTotal = 1500.50m,
                    Helbidea = "Kalea 1",
                    TelefonoZenbakia = 123456789
                },
                new JatetxekoInfo
                {
                    Id = 2,
                    Izena = "Jatetxea 2",
                    KaxaTotal = 2200.75m,
                    Helbidea = "Kalea 2",
                    TelefonoZenbakia = 987654321
                }
            };

            _repoMock.Setup(r => r.GetAll()).Returns(infoList);

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoList = Assert.IsAssignableFrom<IEnumerable<JatetxekoInfoDto>>(okResult.Value);
            Assert.Equal(2, dtoList.Count());
        }

        [Fact]
        public void J2_KontsultaSalbuespena_Propagatu()
        {
            _repoMock.Setup(r => r.GetAll()).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.GetAll());
        }

        [Fact]
        public void J3_InfoExistitzenDa_Ok()
        {
            var info = new JatetxekoInfo
            {
                Id = 1,
                Izena = "Jatetxea 1",
                KaxaTotal = 1500.50m,
                Helbidea = "Kalea 1",
                TelefonoZenbakia = 123456789
            };

            _repoMock.Setup(r => r.Get(1)).Returns(info);

            var result = _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<JatetxekoInfoDto>(okResult.Value);
            Assert.Equal(1, dto.Id);
            Assert.Equal("Jatetxea 1", dto.Izena);
        }

        [Fact]
        public void J4_InfoEzExistitzen_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((JatetxekoInfo)null);

            var result = _controller.Get(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void J5_GetSalbuespena_Propagatu()
        {
            _repoMock.Setup(r => r.Get(1)).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Get(1));
        }

        [Fact]
        public void J6_SorreraOndo_Ok()
        {
            var dto = new JatetxekoInfoSortuDto
            {
                Izena = "Jatetxe berria",
                KaxaTotal = 999.99m,
                Helbidea = "Kale Nagusia 10",
                TelefonoZenbakia = 555666777
            };

            JatetxekoInfo saved = null;
            _repoMock.Setup(r => r.Add(It.IsAny<JatetxekoInfo>()))
                .Callback<JatetxekoInfo>(j =>
                {
                    j.Id = 8;
                    saved = j;
                });

            var result = _controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Informazioa sortuta", value.mezua);
            Assert.Equal(8, (int)value.id);
            Assert.NotNull(saved);
            Assert.Equal("Jatetxe berria", saved.Izena);
        }

        [Fact]
        public void J7_SorreraSalbuespena_Propagatu()
        {
            var dto = new JatetxekoInfoSortuDto
            {
                Izena = "Jatetxe berria",
                KaxaTotal = 999.99m,
                Helbidea = "Kale Nagusia 10",
                TelefonoZenbakia = 555666777
            };

            _repoMock.Setup(r => r.Add(It.IsAny<JatetxekoInfo>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Sortu(dto));
        }

        [Fact]
        public void J8_InfoEzExistitzenEguneratu_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((JatetxekoInfo)null);
            var dto = new JatetxekoInfoSortuDto();

            var result = _controller.Eguneratu(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void J9_EguneratzeOndo_Ok()
        {
            var info = new JatetxekoInfo
            {
                Id = 1,
                Izena = "Jatetxea 1",
                KaxaTotal = 1500.50m,
                Helbidea = "Kalea 1",
                TelefonoZenbakia = 123456789
            };

            _repoMock.Setup(r => r.Get(1)).Returns(info);

            var dto = new JatetxekoInfoSortuDto
            {
                Izena = "Jatetxea berritua",
                KaxaTotal = 3000m,
                Helbidea = "Helbide berria",
                TelefonoZenbakia = 111222333
            };

            var result = _controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Eguneratuta", value.mezua);
            Assert.Equal("Jatetxea berritua", info.Izena);
            Assert.Equal(3000m, info.KaxaTotal);
            _repoMock.Verify(r => r.Update(info), Times.Once);
        }

        [Fact]
        public void J10_EguneratzeSalbuespena_Propagatu()
        {
            var info = new JatetxekoInfo
            {
                Id = 1,
                Izena = "Jatetxea 1",
                KaxaTotal = 1500.50m,
                Helbidea = "Kalea 1",
                TelefonoZenbakia = 123456789
            };

            _repoMock.Setup(r => r.Get(1)).Returns(info);
            _repoMock.Setup(r => r.Update(It.IsAny<JatetxekoInfo>())).Throws(new Exception("Database error"));

            var dto = new JatetxekoInfoSortuDto
            {
                Izena = "Jatetxea berritua",
                KaxaTotal = 3000m,
                Helbidea = "Helbide berria",
                TelefonoZenbakia = 111222333
            };

            Assert.Throws<Exception>(() => _controller.Eguneratu(1, dto));
        }

        [Fact]
        public void J11_InfoEzExistitzenEzabatu_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((JatetxekoInfo)null);

            var result = _controller.Ezabatu(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void J12_EzabatzeOndo_Ok()
        {
            var info = new JatetxekoInfo
            {
                Id = 1,
                Izena = "Jatetxea 1",
                KaxaTotal = 1500.50m,
                Helbidea = "Kalea 1",
                TelefonoZenbakia = 123456789
            };

            _repoMock.Setup(r => r.Get(1)).Returns(info);

            var result = _controller.Ezabatu(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Ezabatuta", value.mezua);
            _repoMock.Verify(r => r.Delete(info), Times.Once);
        }

        [Fact]
        public void J13_EzabatzeSalbuespena_Propagatu()
        {
            var info = new JatetxekoInfo
            {
                Id = 1,
                Izena = "Jatetxea 1",
                KaxaTotal = 1500.50m,
                Helbidea = "Kalea 1",
                TelefonoZenbakia = 123456789
            };

            _repoMock.Setup(r => r.Get(1)).Returns(info);
            _repoMock.Setup(r => r.Delete(It.IsAny<JatetxekoInfo>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Ezabatu(1));
        }
    }
}