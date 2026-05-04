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
    public class ZerbitzuakControllerTests
    {
        private readonly Mock<ZerbitzuakRepository> _repoMock;
        private readonly ZerbitzuakController _controller;

        public ZerbitzuakControllerTests()
        {
            var sessionFactoryMock = new Mock<ISessionFactory>();
            _repoMock = new Mock<ZerbitzuakRepository>(sessionFactoryMock.Object);
            _controller = new ZerbitzuakController(_repoMock.Object);
        }

        private static object GetAnonymousProperty(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
        }

        [Fact]
        public void Z1_GetAllOndo_Ok()
        {
            var data = new DateTime(2026, 3, 16);

            var zerbitzuak = new List<Zerbitzuak>
            {
                new Zerbitzuak(1, 2, 3, data, "Eskatuta", 20m) { Id = 1 },
                new Zerbitzuak(4, 5, 6, data, "Amaituta", 35m) { Id = 2 }
            };

            _repoMock.Setup(r => r.GetAll()).Returns(zerbitzuak);

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoList = Assert.IsAssignableFrom<IEnumerable<ZerbitzuakDto>>(okResult.Value);
            Assert.Equal(2, dtoList.Count());
            Assert.Equal(1, dtoList.First().Id);
        }

        [Fact]
        public void Z2_GetAllSalbuespena_InternalServerError()
        {
            _repoMock.Setup(r => r.GetAll()).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.GetAll());
        }

        [Fact]
        public void Z3_ZerbitzuaExistitzenDa_Ok()
        {
            var data = new DateTime(2026, 3, 16);
            var zerbitzua = new Zerbitzuak(1, 2, 3, data, "Eskatuta", 20m) { Id = 1 };

            _repoMock.Setup(r => r.Get(1)).Returns(zerbitzua);

            var result = _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ZerbitzuakDto>(okResult.Value);

            Assert.Equal(1, dto.Id);
            Assert.Equal(1, dto.LangileId);
            Assert.Equal(2, dto.MahaiaId);
            Assert.Equal(3, dto.ErreserbaId);
            Assert.Equal("Eskatuta", dto.Egoera);
            Assert.Equal(20m, dto.Guztira);
        }

        [Fact]
        public void Z4_ZerbitzuaEzExistitzen_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Zerbitzuak)null);

            var result = _controller.Get(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Ez da aurkitu", GetAnonymousProperty(notFound.Value, "mezua"));
        }

        [Fact]
        public void Z5_GetSalbuespena_InternalServerError()
        {
            _repoMock.Setup(r => r.Get(1)).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Get(1));
        }

        [Fact]
        public void Z6_SortuOndo_Ok()
        {
            var dto = new ZerbitzuakSortuDto
            {
                LangileId = 2,
                MahaiaId = 3,
                ErreserbaId = 4,
                Egoera = "Eskatuta",
                Guztira = 42m
            };

            Zerbitzuak gordeta = null;

            _repoMock.Setup(r => r.Add(It.IsAny<Zerbitzuak>()))
                .Callback<Zerbitzuak>(z =>
                {
                    z.Id = 7;
                    gordeta = z;
                });

            var result = _controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Zerbitzuak sortuta", GetAnonymousProperty(okResult.Value, "mezua"));
            Assert.Equal(7, GetAnonymousProperty(okResult.Value, "id"));

            Assert.NotNull(gordeta);
            Assert.Equal(2, gordeta.LangileId);
            Assert.Equal(3, gordeta.MahaiaId);
            Assert.Equal(4, gordeta.ErreserbaId);
            Assert.Equal("Eskatuta", gordeta.Egoera);
            Assert.Equal(42m, gordeta.Guztira);
        }

        [Fact]
        public void Z7_SortuBodyNull_InternalServerError()
        {
            Assert.Throws<NullReferenceException>(() => _controller.Sortu(null));
            _repoMock.Verify(r => r.Add(It.IsAny<Zerbitzuak>()), Times.Never);
        }

        [Fact]
        public void Z8_SortuSalbuespena_InternalServerError()
        {
            var dto = new ZerbitzuakSortuDto
            {
                LangileId = 2,
                MahaiaId = 3,
                ErreserbaId = 4,
                Egoera = "Eskatuta",
                Guztira = 42m
            };

            _repoMock.Setup(r => r.Add(It.IsAny<Zerbitzuak>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Sortu(dto));
        }

        [Fact]
        public void Z9_ZerbitzuaEzExistitzen_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Zerbitzuak)null);
            var dto = new ZerbitzuakSortuDto
            {
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = 3,
                Egoera = "Eskatuta",
                Guztira = 10m
            };

            var result = _controller.Eguneratu(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Ez da aurkitu", GetAnonymousProperty(notFound.Value, "mezua"));
        }

        [Fact]
        public void Z10_EguneratuOndo_Ok()
        {
            var data = new DateTime(2026, 3, 16);
            var zerbitzua = new Zerbitzuak(1, 2, 3, data, "Eskatuta", 20m) { Id = 1 };

            _repoMock.Setup(r => r.Get(1)).Returns(zerbitzua);

            var dto = new ZerbitzuakSortuDto
            {
                LangileId = 9,
                MahaiaId = 8,
                ErreserbaId = 7,
                EskaeraData = data.AddDays(1),
                Egoera = "Amaituta",
                Guztira = 99m
            };

            var result = _controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Eguneratuta", GetAnonymousProperty(okResult.Value, "mezua"));

            Assert.Equal(9, zerbitzua.LangileId);
            Assert.Equal(8, zerbitzua.MahaiaId);
            Assert.Equal(7, zerbitzua.ErreserbaId);
            Assert.Equal(data.AddDays(1), zerbitzua.EskaeraData);
            Assert.Equal("Amaituta", zerbitzua.Egoera);
            Assert.Equal(99m, zerbitzua.Guztira);

            _repoMock.Verify(r => r.Update(zerbitzua), Times.Once);
        }

        [Fact]
        public void Z11_EguneratuBodyNull_InternalServerError()
        {
            var data = new DateTime(2026, 3, 16);
            var zerbitzua = new Zerbitzuak(1, 2, 3, data, "Eskatuta", 20m) { Id = 1 };

            _repoMock.Setup(r => r.Get(1)).Returns(zerbitzua);

            Assert.Throws<NullReferenceException>(() => _controller.Eguneratu(1, null));
            _repoMock.Verify(r => r.Update(It.IsAny<Zerbitzuak>()), Times.Never);
        }

        [Fact]
        public void Z12_EguneratuSalbuespena_InternalServerError()
        {
            var data = new DateTime(2026, 3, 16);
            var zerbitzua = new Zerbitzuak(1, 2, 3, data, "Eskatuta", 20m) { Id = 1 };

            _repoMock.Setup(r => r.Get(1)).Returns(zerbitzua);
            _repoMock.Setup(r => r.Update(It.IsAny<Zerbitzuak>())).Throws(new Exception("Database error"));

            var dto = new ZerbitzuakSortuDto
            {
                LangileId = 9,
                MahaiaId = 8,
                ErreserbaId = 7,
                EskaeraData = data.AddDays(1),
                Egoera = "Amaituta",
                Guztira = 99m
            };

            Assert.Throws<Exception>(() => _controller.Eguneratu(1, dto));
        }

        [Fact]
        public void Z13_ZerbitzuaEzExistitzen_NotFound()
        {
            _repoMock.Setup(r => r.Get(999)).Returns((Zerbitzuak)null);

            var result = _controller.Ezabatu(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Ez da aurkitu", GetAnonymousProperty(notFound.Value, "mezua"));
        }

        [Fact]
        public void Z14_EzabatuOndo_Ok()
        {
            var data = new DateTime(2026, 3, 16);
            var zerbitzua = new Zerbitzuak(1, 2, 3, data, "Eskatuta", 20m) { Id = 1 };

            _repoMock.Setup(r => r.Get(1)).Returns(zerbitzua);

            var result = _controller.Ezabatu(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Ezabatuta", GetAnonymousProperty(okResult.Value, "mezua"));
            _repoMock.Verify(r => r.Delete(zerbitzua), Times.Once);
        }

        [Fact]
        public void Z15_EzabatuSalbuespena_InternalServerError()
        {
            var data = new DateTime(2026, 3, 16);
            var zerbitzua = new Zerbitzuak(1, 2, 3, data, "Eskatuta", 20m) { Id = 1 };

            _repoMock.Setup(r => r.Get(1)).Returns(zerbitzua);
            _repoMock.Setup(r => r.Delete(It.IsAny<Zerbitzuak>())).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.Ezabatu(1));
        }

        [Fact]
        public void Z16_ZerbitzuaEginBerriaOndo_Ok()
        {
            var dto = new ZerbitzuaEskariaDto
            {
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = 10,
                EskaeraData = new DateTime(2026, 3, 16),
                Platerak = new List<PlateraEskariaDto>
                {
                    new PlateraEskariaDto { PlateraId = 100, Kantitatea = 2 }
                }
            };

            var emaitza = new ZerbitzuaEmaitzaDto
            {
                Ondo = true,
                ZerbitzuaId = 50,
                Erroreak = new List<ZerbitzuErroreaDto>()
            };

            _repoMock.Setup(r => r.ZerbitzuaEgin(dto)).Returns(emaitza);

            var result = _controller.ZerbitzuaEgin(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ZerbitzuaEmaitzaDto>(okResult.Value);

            Assert.True(value.Ondo);
            Assert.Equal(50, value.ZerbitzuaId);
        }

        [Fact]
        public void Z17_ZerbitzuaEginPlaterBerriaOndo_Ok()
        {
            var dto = new ZerbitzuaEskariaDto
            {
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = 11,
                EskaeraData = new DateTime(2026, 3, 16),
                Platerak = new List<PlateraEskariaDto>
                {
                    new PlateraEskariaDto { PlateraId = 101, Kantitatea = 1 }
                }
            };

            var emaitza = new ZerbitzuaEmaitzaDto
            {
                Ondo = true,
                ZerbitzuaId = 51,
                Erroreak = new List<ZerbitzuErroreaDto>()
            };

            _repoMock.Setup(r => r.ZerbitzuaEgin(dto)).Returns(emaitza);

            var result = _controller.ZerbitzuaEgin(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ZerbitzuaEmaitzaDto>(okResult.Value);

            Assert.True(value.Ondo);
            Assert.Equal(51, value.ZerbitzuaId);
        }

        [Fact]
        public void Z18_ZerbitzuaEginKantitateaHanditu_Ok()
        {
            var dto = new ZerbitzuaEskariaDto
            {
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = 12,
                EskaeraData = new DateTime(2026, 3, 16),
                Platerak = new List<PlateraEskariaDto>
                {
                    new PlateraEskariaDto { PlateraId = 102, Kantitatea = 5 }
                }
            };

            var emaitza = new ZerbitzuaEmaitzaDto
            {
                Ondo = true,
                ZerbitzuaId = 52,
                Erroreak = new List<ZerbitzuErroreaDto>()
            };

            _repoMock.Setup(r => r.ZerbitzuaEgin(dto)).Returns(emaitza);

            var result = _controller.ZerbitzuaEgin(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ZerbitzuaEmaitzaDto>(okResult.Value);

            Assert.True(value.Ondo);
            Assert.Equal(52, value.ZerbitzuaId);
        }

        [Fact]
        public void Z19_ZerbitzuaEginKantitateaMurriztu_Ok()
        {
            var dto = new ZerbitzuaEskariaDto
            {
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = 13,
                EskaeraData = new DateTime(2026, 3, 16),
                Platerak = new List<PlateraEskariaDto>
                {
                    new PlateraEskariaDto { PlateraId = 103, Kantitatea = 1 }
                }
            };

            var emaitza = new ZerbitzuaEmaitzaDto
            {
                Ondo = true,
                ZerbitzuaId = 53,
                Erroreak = new List<ZerbitzuErroreaDto>()
            };

            _repoMock.Setup(r => r.ZerbitzuaEgin(dto)).Returns(emaitza);

            var result = _controller.ZerbitzuaEgin(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ZerbitzuaEmaitzaDto>(okResult.Value);

            Assert.True(value.Ondo);
            Assert.Equal(53, value.ZerbitzuaId);
        }

        [Fact]
        public void Z20_ZerbitzuaEginXehetasunaEzabatu_Ok()
        {
            var dto = new ZerbitzuaEskariaDto
            {
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = 14,
                EskaeraData = new DateTime(2026, 3, 16),
                Platerak = new List<PlateraEskariaDto>
                {
                    new PlateraEskariaDto { PlateraId = 104, Kantitatea = 0 }
                }
            };

            var emaitza = new ZerbitzuaEmaitzaDto
            {
                Ondo = true,
                ZerbitzuaId = 54,
                Erroreak = new List<ZerbitzuErroreaDto>()
            };

            _repoMock.Setup(r => r.ZerbitzuaEgin(dto)).Returns(emaitza);

            var result = _controller.ZerbitzuaEgin(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ZerbitzuaEmaitzaDto>(okResult.Value);

            Assert.True(value.Ondo);
            Assert.Equal(54, value.ZerbitzuaId);
        }

        [Fact]
        public void Z21_ZerbitzuaEginZerbitzatutaMurriztuSaiakera_Ok()
        {
            var dto = new ZerbitzuaEskariaDto
            {
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = 15,
                EskaeraData = new DateTime(2026, 3, 16),
                Platerak = new List<PlateraEskariaDto>
                {
                    new PlateraEskariaDto { PlateraId = 105, Kantitatea = 1 }
                }
            };

            var emaitza = new ZerbitzuaEmaitzaDto
            {
                Ondo = true,
                ZerbitzuaId = 55,
                Erroreak = new List<ZerbitzuErroreaDto>()
            };

            _repoMock.Setup(r => r.ZerbitzuaEgin(dto)).Returns(emaitza);

            var result = _controller.ZerbitzuaEgin(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ZerbitzuaEmaitzaDto>(okResult.Value);

            Assert.True(value.Ondo);
            Assert.Equal(55, value.ZerbitzuaId);
        }

        [Fact]
        public void Z22_ZerbitzuaEginBodyNull_InternalServerError()
        {
            _repoMock.Setup(r => r.ZerbitzuaEgin((ZerbitzuaEskariaDto)null))
                .Throws(new NullReferenceException());

            Assert.Throws<NullReferenceException>(() => _controller.ZerbitzuaEgin(null));
        }

        [Fact]
        public void Z23_ZerbitzuaEginSalbuespena_InternalServerError()
        {
            var dto = new ZerbitzuaEskariaDto
            {
                LangileId = 1,
                MahaiaId = 2,
                ErreserbaId = 16,
                EskaeraData = new DateTime(2026, 3, 16),
                Platerak = new List<PlateraEskariaDto>
                {
                    new PlateraEskariaDto { PlateraId = 106, Kantitatea = 2 }
                }
            };

            _repoMock.Setup(r => r.ZerbitzuaEgin(dto)).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.ZerbitzuaEgin(dto));
        }

        [Fact]
        public void Z24_GetPlaterakByErreserbaOndo_Ok()
        {
            var zerbitzua = new Zerbitzuak
            {
                Id = 30,
                ErreserbaId = 99
            };

            var platerak = new List<object>
            {
                new { PlateraId = 1, Kantitatea = 2, Zerbitzatuta = false },
                new { PlateraId = 2, Kantitatea = 1, Zerbitzatuta = true }
            };

            _repoMock.Setup(r => r.GetByErreserbaId(99)).Returns(zerbitzua);
            _repoMock.Setup(r => r.GetPlaterakLaburpenaByZerbitzuaId(30)).Returns(platerak);

            var result = _controller.GetPlaterakByErreserba(99);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);

            Assert.Equal(2, value.Count());
        }

        [Fact]
        public void Z25_GetPlaterakByErreserbaNotFound_NotFound()
        {
            _repoMock.Setup(r => r.GetByErreserbaId(999)).Returns((Zerbitzuak)null);

            var result = _controller.GetPlaterakByErreserba(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Ez dago zerbitzurik erreserba honekin", GetAnonymousProperty(notFound.Value, "mezua"));
        }

        [Fact]
        public void Z26_GetPlaterakByErreserbaSalbuespena_InternalServerError()
        {
            _repoMock.Setup(r => r.GetByErreserbaId(50)).Throws(new Exception("Database error"));

            Assert.Throws<Exception>(() => _controller.GetPlaterakByErreserba(50));
        }
    }
}