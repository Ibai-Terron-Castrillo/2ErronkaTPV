using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using JatetxeaApi.Controllerrak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.DTOak;
using JatetxeaApi.Repositorioak;

namespace JatetxeaApi.Testak
{
    public class ZerbitzuXehetasunakControllerTests
    {
        [Fact]
        public void ZX1_GetAllOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var elementuak = new List<ZerbitzuXehetasunak>
            {
                new ZerbitzuXehetasunak
                {
                    Id = 1,
                    ZerbitzuaId = 10,
                    PlateraId = 100,
                    Kantitatea = 2,
                    PrezioUnitarioa = 12.5m,
                    Zerbitzatuta = false
                },
                new ZerbitzuXehetasunak
                {
                    Id = 2,
                    ZerbitzuaId = 11,
                    PlateraId = 101,
                    Kantitatea = 3,
                    PrezioUnitarioa = 15m,
                    Zerbitzatuta = true
                }
            };

            mockRepo.Setup(r => r.GetAll()).Returns(elementuak);
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act
            var result = controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedElementuak = Assert.IsAssignableFrom<IEnumerable<ZerbitzuXehetasunakDto>>(okResult.Value);
            var lista = returnedElementuak.ToList();

            Assert.Equal(2, lista.Count);
            Assert.Equal(10, lista[0].ZerbitzuaId);
            Assert.Equal(100, lista[0].PlateraId);
            Assert.Equal(2, lista[0].Kantitatea);
            Assert.Equal(12.5m, lista[0].PrezioUnitarioa);
            Assert.False(lista[0].Zerbitzatuta);
        }

        [Fact]
        public void ZX2_GetAllSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            mockRepo.Setup(r => r.GetAll()).Throws(new Exception("Errorea"));
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.GetAll());
        }

        [Fact]
        public void ZX3_SortuOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var dto = new ZerbitzuXehetasunakSortuDto
            {
                ZerbitzuaId = 10,
                PlateraId = 100,
                Kantitatea = 2,
                PrezioUnitarioa = 12.5m,
                Zerbitzatuta = false
            };

            mockRepo.Setup(r => r.Add(It.IsAny<ZerbitzuXehetasunak>()));
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act
            var result = controller.Sortu(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;

            var mezua = value.GetType().GetProperty("mezua")?.GetValue(value, null);
            Assert.Equal("Xehetasuna sortuta", mezua);

            mockRepo.Verify(r => r.Add(It.IsAny<ZerbitzuXehetasunak>()), Times.Once);
        }

        [Fact]
        public void ZX4_SortuBodyNull_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => controller.Sortu(null));
        }

        [Fact]
        public void ZX5_SortuSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var dto = new ZerbitzuXehetasunakSortuDto
            {
                ZerbitzuaId = 10,
                PlateraId = 100,
                Kantitatea = 2,
                PrezioUnitarioa = 12.5m,
                Zerbitzatuta = false
            };

            mockRepo.Setup(r => r.Add(It.IsAny<ZerbitzuXehetasunak>())).Throws(new Exception("Errorea"));
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Sortu(dto));
        }

        [Fact]
        public void ZX6_XehetasunaExistitzenDa_Ok()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var elementua = new ZerbitzuXehetasunak
            {
                Id = 1,
                ZerbitzuaId = 10,
                PlateraId = 100,
                Kantitatea = 2,
                PrezioUnitarioa = 12.5m,
                Zerbitzatuta = false
            };

            mockRepo.Setup(r => r.Get(1)).Returns(elementua);
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act
            var result = controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedElementua = Assert.IsType<ZerbitzuXehetasunakDto>(okResult.Value);

            Assert.Equal(1, returnedElementua.Id);
            Assert.Equal(10, returnedElementua.ZerbitzuaId);
            Assert.Equal(100, returnedElementua.PlateraId);
            Assert.Equal(2, returnedElementua.Kantitatea);
            Assert.Equal(12.5m, returnedElementua.PrezioUnitarioa);
            Assert.False(returnedElementua.Zerbitzatuta);
        }

        [Fact]
        public void ZX7_XehetasunaEzExistitzen_NotFound()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            mockRepo.Setup(r => r.Get(1)).Returns((ZerbitzuXehetasunak)null);
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act
            var result = controller.Get(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var mezua = notFoundResult.Value.GetType().GetProperty("mezua")?.GetValue(notFoundResult.Value, null);

            Assert.Equal("Ez da aurkitu", mezua);
        }

        [Fact]
        public void ZX8_GetSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            mockRepo.Setup(r => r.Get(1)).Throws(new Exception("Errorea"));
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Get(1));
        }

        [Fact]
        public void ZX9_XehetasunaEzExistitzen_NotFound()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            mockRepo.Setup(r => r.Get(1)).Returns((ZerbitzuXehetasunak)null);
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act
            var result = controller.Eguneratu(1, new ZerbitzuXehetasunakSortuDto());

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var mezua = notFoundResult.Value.GetType().GetProperty("mezua")?.GetValue(notFoundResult.Value, null);

            Assert.Equal("Ez da aurkitu", mezua);
        }

        [Fact]
        public void ZX10_EguneratuOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var elementua = new ZerbitzuXehetasunak
            {
                Id = 1,
                ZerbitzuaId = 10,
                PlateraId = 100,
                Kantitatea = 2,
                PrezioUnitarioa = 12.5m,
                Zerbitzatuta = false
            };

            var dto = new ZerbitzuXehetasunakSortuDto
            {
                ZerbitzuaId = 20,
                PlateraId = 200,
                Kantitatea = 5,
                PrezioUnitarioa = 30m,
                Zerbitzatuta = true
            };

            mockRepo.Setup(r => r.Get(1)).Returns(elementua);
            mockRepo.Setup(r => r.Update(It.IsAny<ZerbitzuXehetasunak>()));
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act
            var result = controller.Eguneratu(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var mezua = okResult.Value.GetType().GetProperty("mezua")?.GetValue(okResult.Value, null);

            Assert.Equal("Eguneratuta", mezua);
            Assert.Equal(20, elementua.ZerbitzuaId);
            Assert.Equal(200, elementua.PlateraId);
            Assert.Equal(5, elementua.Kantitatea);
            Assert.Equal(30m, elementua.PrezioUnitarioa);
            Assert.True(elementua.Zerbitzatuta);

            mockRepo.Verify(r => r.Update(It.IsAny<ZerbitzuXehetasunak>()), Times.Once);
        }

        [Fact]
        public void ZX11_EguneratuBodyNull_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var elementua = new ZerbitzuXehetasunak
            {
                Id = 1,
                ZerbitzuaId = 10,
                PlateraId = 100,
                Kantitatea = 2,
                PrezioUnitarioa = 12.5m,
                Zerbitzatuta = false
            };

            mockRepo.Setup(r => r.Get(1)).Returns(elementua);
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => controller.Eguneratu(1, null));
        }

        [Fact]
        public void ZX12_EguneratuSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var elementua = new ZerbitzuXehetasunak
            {
                Id = 1,
                ZerbitzuaId = 10,
                PlateraId = 100,
                Kantitatea = 2,
                PrezioUnitarioa = 12.5m,
                Zerbitzatuta = false
            };

            var dto = new ZerbitzuXehetasunakSortuDto
            {
                ZerbitzuaId = 20,
                PlateraId = 200,
                Kantitatea = 5,
                PrezioUnitarioa = 30m,
                Zerbitzatuta = true
            };

            mockRepo.Setup(r => r.Get(1)).Returns(elementua);
            mockRepo.Setup(r => r.Update(It.IsAny<ZerbitzuXehetasunak>())).Throws(new Exception("Errorea"));
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Eguneratu(1, dto));
        }

        [Fact]
        public void ZX13_XehetasunaEzExistitzen_NotFound()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            mockRepo.Setup(r => r.Get(1)).Returns((ZerbitzuXehetasunak)null);
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act
            var result = controller.Ezabatu(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var mezua = notFoundResult.Value.GetType().GetProperty("mezua")?.GetValue(notFoundResult.Value, null);

            Assert.Equal("Ez da aurkitu", mezua);
        }

        [Fact]
        public void ZX14_EzabatuOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var elementua = new ZerbitzuXehetasunak
            {
                Id = 1,
                ZerbitzuaId = 10,
                PlateraId = 100,
                Kantitatea = 2,
                PrezioUnitarioa = 12.5m,
                Zerbitzatuta = false
            };

            mockRepo.Setup(r => r.Get(1)).Returns(elementua);
            mockRepo.Setup(r => r.Delete(It.IsAny<ZerbitzuXehetasunak>()));
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act
            var result = controller.Ezabatu(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var mezua = okResult.Value.GetType().GetProperty("mezua")?.GetValue(okResult.Value, null);

            Assert.Equal("Ezabatuta", mezua);
            mockRepo.Verify(r => r.Delete(It.IsAny<ZerbitzuXehetasunak>()), Times.Once);
        }

        [Fact]
        public void ZX15_EzabatuSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<ZerbitzuXehetasunakRepository>();
            var elementua = new ZerbitzuXehetasunak
            {
                Id = 1,
                ZerbitzuaId = 10,
                PlateraId = 100,
                Kantitatea = 2,
                PrezioUnitarioa = 12.5m,
                Zerbitzatuta = false
            };

            mockRepo.Setup(r => r.Get(1)).Returns(elementua);
            mockRepo.Setup(r => r.Delete(It.IsAny<ZerbitzuXehetasunak>())).Throws(new Exception("Errorea"));
            var controller = new ZerbitzuXehetasunakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Ezabatu(1));
        }
    }
}