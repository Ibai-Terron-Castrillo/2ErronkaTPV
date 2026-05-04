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
    public class LangileakControllerTests
    {
        private readonly Mock<LangileakRepository> _repoMock;
        private readonly LangileakController _controller;

        public LangileakControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<LangileakRepository>(sessionFactoryMock.Object);
            _controller = new LangileakController(_repoMock.Object);
        }

        [Fact]
        public void P1_GetAllOndo_Ok()
        {
            var langileak = new List<Langileak>
            {
                new Langileak
                {
                    Id = 1,
                    Izena = "Langile1",
                    Erabiltzailea = "erabiltzailea1",
                    Pasahitza = "pasahitza1",
                    Aktibo = "Bai",
                    ErregistroData = DateTime.Now,
                    RolaId = 1
                },
                new Langileak
                {
                    Id = 2,
                    Izena = "Langile2",
                    Erabiltzailea = "erabiltzailea2",
                    Pasahitza = "pasahitza2",
                    Aktibo = "Bai",
                    ErregistroData = DateTime.Now,
                    RolaId = 2
                }
            };
            _repoMock.Setup(r => r.GetAll()).Returns(langileak);

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoList = Assert.IsAssignableFrom<IEnumerable<LangileakDto>>(okResult.Value);
            Assert.Equal(2, dtoList.Count());
        }

        [Fact]
        public void P2_GetAllSalbuespena_InternalServerError()
        {
            _repoMock.Setup(r => r.GetAll()).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.GetAll());
        }

        [Fact]
        public void P3_LangileaExistitzenDa_Ok()
        {
            var langilea = new Langileak
            {
                Id = 1,
                Izena = "Langile1",
                Erabiltzailea = "erabiltzailea1",
                Pasahitza = "pasahitza1",
                Aktibo = "Bai",
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);

            var result = _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<LangileakDto>(okResult.Value);
            Assert.Equal(1, dto.Id);
        }

        [Fact]
        public void P4_LangileaEzExistitzen_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Langileak)null);

            var result = _controller.Get(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void P5_GetSalbuespena_InternalServerError()
        {
            _repoMock.Setup(r => r.Get(1)).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Get(1));
        }

        [Fact]
        public void P6_LoginEskeraBalioezina_BadRequest()
        {
            LoginRequest loginNull = null;
            var loginEmpty = new LoginRequest { Erabiltzailea = "", Pasahitza = "pass" };
            var loginNoPass = new LoginRequest { Erabiltzailea = "user", Pasahitza = "" };

            var result1 = _controller.Login(loginNull);
            var badRequest1 = Assert.IsType<BadRequestObjectResult>(result1);
            dynamic value1 = badRequest1.Value;
            Assert.Equal("Erabiltzailea eta pasahitza beharrezkoak dira.", value1.mezua);

            var result2 = _controller.Login(loginEmpty);
            var badRequest2 = Assert.IsType<BadRequestObjectResult>(result2);
            dynamic value2 = badRequest2.Value;
            Assert.Equal("Erabiltzailea eta pasahitza beharrezkoak dira.", value2.mezua);

            var result3 = _controller.Login(loginNoPass);
            var badRequest3 = Assert.IsType<BadRequestObjectResult>(result3);
            dynamic value3 = badRequest3.Value;
            Assert.Equal("Erabiltzailea eta pasahitza beharrezkoak dira.", value3.mezua);
        }

        [Fact]
        public void P7_KredentzialOkerrak_Unauthorized()
        {
            var langileak = new List<Langileak>
            {
                new Langileak
                {
                    Izena = "Langile1",
                    Erabiltzailea = "erabiltzailea1",
                    Pasahitza = "pasahitza1",
                    Aktibo = "Bai",
                    ErregistroData = DateTime.Now,
                    RolaId = 1
                }
            };
            _repoMock.Setup(r => r.GetAll()).Returns(langileak);
            var login = new LoginRequest { Erabiltzailea = "erabiltzailea1", Pasahitza = "wrongpass" };

            var result = _controller.Login(login);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            dynamic value = unauthorized.Value;
            Assert.Equal("Erabiltzailea edo pasahitza oker", value.mezua);
        }

        [Fact]
        public void P8_KredentzialZuzenak_Ok()
        {
            var langilea = new Langileak
            {
                Id = 1,
                Izena = "Langile1",
                Erabiltzailea = "erabiltzailea1",
                Pasahitza = "pasahitza1",
                Aktibo = "Bai",
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            var langileak = new List<Langileak> { langilea };
            _repoMock.Setup(r => r.GetAll()).Returns(langileak);
            var login = new LoginRequest { Erabiltzailea = "erabiltzailea1", Pasahitza = "pasahitza1" };

            var result = _controller.Login(login);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<LangileakDto>(okResult.Value);
            Assert.Equal(langilea.Id, dto.Id);
        }

        [Fact]
        public void P9_LoginSalbuespena_InternalServerError()
        {
            _repoMock.Setup(r => r.GetAll()).Throws(new Exception("Database error"));
            var login = new LoginRequest { Erabiltzailea = "user", Pasahitza = "pass" };

            Assert.Throws<Exception>(() => _controller.Login(login));
        }

        [Fact]
        public void P10_LangileaSortuOndo_Ok()
        {
            var dto = new LangileakSortuDto
            {
                Izena = "Langile",
                Erabiltzailea = "user",
                Pasahitza = "pass",
                Aktibo = "Bai",
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            Langileak savedLangile = null;
            _repoMock.Setup(r => r.Add(It.IsAny<Langileak>())).Callback<Langileak>(l => savedLangile = l);

            var result = _controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Langilea sortuta", value.mezua);
            Assert.NotNull(savedLangile);
        }

        [Fact]
        public void P11_SortuSalbuespena_InternalServerError()
        {
            var dto = new LangileakSortuDto
            {
                Izena = "Langile",
                Erabiltzailea = "user",
                Pasahitza = "pass",
                Aktibo = "Bai",
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            _repoMock.Setup(r => r.Add(It.IsAny<Langileak>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Sortu(dto));
        }

        [Fact]
        public void P12_LangileaEzExistitzen_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Langileak)null);
            var dto = new LangileakSortuDto();  // DTO hutsa, ez da erabiliko

            var result = _controller.Eguneratu(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void P13_LangileaEguneratuOndo_Ok()
        {
            var langilea = new Langileak
            {
                Id = 1,
                Izena = "Langile",
                Erabiltzailea = "user",
                Pasahitza = "pass",
                Aktibo = "Bai",
                ErregistroData = DateTime.Now,
                RolaId = 1
            };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);
            var dto = new LangileakSortuDto
            {
                Izena = "LangileUpdated",
                Erabiltzailea = "userUpdated",
                Pasahitza = "passUpdated",
                Aktibo = "Ez",
                ErregistroData = DateTime.Now,
                RolaId = 2
            };

            var result = _controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Eguneratuta", value.mezua);
            _repoMock.Verify(r => r.Update(langilea), Times.Once);
        }

        [Fact]
        public void P14_EguneratuSalbuespena_InternalServerError()
        {
            var langilea = new Langileak { Id = 1 };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);
            _repoMock.Setup(r => r.Update(It.IsAny<Langileak>())).Throws(new Exception("Database error"));
            var dto = new LangileakSortuDto();   // DTO hutsa, ez da erabiliko

            Assert.Throws<Exception>(() => _controller.Eguneratu(1, dto));
        }

        [Fact]
        public void P15_LangileaEzExistitzen_NotFound_Ezabatu()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Langileak)null);

            var result = _controller.Ezabatu(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void P16_LangileaEzabatuOndo_Ok()
        {
            var langilea = new Langileak { Id = 1 };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);

            var result = _controller.Ezabatu(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Ezabatuta", value.mezua);
            _repoMock.Verify(r => r.Delete(langilea), Times.Once);
        }

        [Fact]
        public void P17_EzabatuSalbuespena_InternalServerError()
        {
            var langilea = new Langileak { Id = 1 };
            _repoMock.Setup(r => r.Get(1)).Returns(langilea);
            _repoMock.Setup(r => r.Delete(It.IsAny<Langileak>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Ezabatu(1));
        }
    }
}