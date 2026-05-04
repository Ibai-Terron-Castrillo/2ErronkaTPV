using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using JatetxeaApi.Controllerrak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.Repositorioak;

namespace JatetxeaApi.Testak
{
    public class PlaterenOsagaiakControllerTests
    {
        [Fact]
        public void PO1_GetAllOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var elementuak = new List<PlaterenOsagaiak>
            {
                new PlaterenOsagaiak { PlateraId = 1, InbentarioaId = 1, Kantitatea = 2 },
                new PlaterenOsagaiak { PlateraId = 1, InbentarioaId = 2, Kantitatea = 3 }
            };

            mockRepo.Setup(r => r.GetAll()).Returns(elementuak);
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act
            var result = controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedElementuak = Assert.IsType<List<PlaterenOsagaiak>>(okResult.Value);
            Assert.Equal(2, returnedElementuak.Count);
        }

        [Fact]
        public void PO2_GetAllSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            mockRepo.Setup(r => r.GetAll()).Throws(new Exception("Errorea"));
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.GetAll());
        }

        [Fact]
        public void PO3_ErlazioaExistitzenDa_Ok()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var elementua = new PlaterenOsagaiak
            {
                PlateraId = 1,
                InbentarioaId = 2,
                Kantitatea = 5
            };

            mockRepo.Setup(r => r.Get(1, 2)).Returns(elementua);
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act
            var result = controller.Get(1, 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedElementua = Assert.IsType<PlaterenOsagaiak>(okResult.Value);
            Assert.Equal(5, returnedElementua.Kantitatea);
        }

        [Fact]
        public void PO4_ErlazioaEzExistitzen_NotFound()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            mockRepo.Setup(r => r.Get(1, 2)).Returns((PlaterenOsagaiak)null);
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act
            var result = controller.Get(1, 2);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var mezua = notFoundResult.Value.GetType().GetProperty("mezua")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("Ez da aurkitu", mezua);
        }

        [Fact]
        public void PO5_GetSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            mockRepo.Setup(r => r.Get(1, 2)).Throws(new Exception("Errorea"));
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Get(1, 2));
        }

        [Fact]
        public void PO6_SortuOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var dto = new PlaterenOsagaiak
            {
                PlateraId = 3,
                InbentarioaId = 4,
                Kantitatea = 7
            };

            mockRepo.Setup(r => r.Add(It.IsAny<PlaterenOsagaiak>()));
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act
            var result = controller.Sortu(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;

            var mezua = value.GetType().GetProperty("mezua")?.GetValue(value, null);
            var plateraId = value.GetType().GetProperty("platareaId")?.GetValue(value, null);
            var inbentarioaId = value.GetType().GetProperty("inbentarioaId")?.GetValue(value, null);

            Assert.Equal("Elementua sortuta", mezua);
            Assert.Equal(3, plateraId);
            Assert.Equal(4, inbentarioaId);
        }

        [Fact]
        public void PO7_SortuBodyNull_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => controller.Sortu(null));
        }

        [Fact]
        public void PO8_SortuSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var dto = new PlaterenOsagaiak
            {
                PlateraId = 3,
                InbentarioaId = 4,
                Kantitatea = 7
            };

            mockRepo.Setup(r => r.Add(It.IsAny<PlaterenOsagaiak>())).Throws(new Exception("Errorea"));
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Sortu(dto));
        }

        [Fact]
        public void PO9_ErlazioaEzExistitzen_NotFound()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            mockRepo.Setup(r => r.Get(1, 2)).Returns((PlaterenOsagaiak)null);
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act
            var result = controller.Eguneratu(1, 2, new PlaterenOsagaiak { Kantitatea = 10 });

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var mezua = notFoundResult.Value.GetType().GetProperty("mezua")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("Ez da aurkitu", mezua);
        }

        [Fact]
        public void PO10_EguneratuOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var existitzenDenElementua = new PlaterenOsagaiak
            {
                PlateraId = 1,
                InbentarioaId = 2,
                Kantitatea = 5
            };

            var dto = new PlaterenOsagaiak
            {
                Kantitatea = 10
            };

            mockRepo.Setup(r => r.Get(1, 2)).Returns(existitzenDenElementua);
            mockRepo.Setup(r => r.Update(It.IsAny<PlaterenOsagaiak>()));
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act
            var result = controller.Eguneratu(1, 2, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var mezua = okResult.Value.GetType().GetProperty("mezua")?.GetValue(okResult.Value, null);

            Assert.Equal("Eguneratuta", mezua);
            Assert.Equal(10, existitzenDenElementua.Kantitatea);
            mockRepo.Verify(r => r.Update(It.IsAny<PlaterenOsagaiak>()), Times.Once);
        }

        [Fact]
        public void PO11_EguneratuBodyNull_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var existitzenDenElementua = new PlaterenOsagaiak
            {
                PlateraId = 1,
                InbentarioaId = 2,
                Kantitatea = 5
            };

            mockRepo.Setup(r => r.Get(1, 2)).Returns(existitzenDenElementua);
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => controller.Eguneratu(1, 2, null));
        }

        [Fact]
        public void PO12_EguneratuSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var existitzenDenElementua = new PlaterenOsagaiak
            {
                PlateraId = 1,
                InbentarioaId = 2,
                Kantitatea = 5
            };

            var dto = new PlaterenOsagaiak
            {
                Kantitatea = 10
            };

            mockRepo.Setup(r => r.Get(1, 2)).Returns(existitzenDenElementua);
            mockRepo.Setup(r => r.Update(It.IsAny<PlaterenOsagaiak>())).Throws(new Exception("Errorea"));
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Eguneratu(1, 2, dto));
        }

        [Fact]
        public void PO13_ErlazioaEzExistitzen_NotFound()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            mockRepo.Setup(r => r.Get(1, 2)).Returns((PlaterenOsagaiak)null);
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act
            var result = controller.Ezabatu(1, 2);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var mezua = notFoundResult.Value.GetType().GetProperty("mezua")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("Ez da aurkitu", mezua);
        }

        [Fact]
        public void PO14_EzabatuOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var existitzenDenElementua = new PlaterenOsagaiak
            {
                PlateraId = 1,
                InbentarioaId = 2,
                Kantitatea = 5
            };

            mockRepo.Setup(r => r.Get(1, 2)).Returns(existitzenDenElementua);
            mockRepo.Setup(r => r.Delete(It.IsAny<PlaterenOsagaiak>()));
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act
            var result = controller.Ezabatu(1, 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var mezua = okResult.Value.GetType().GetProperty("mezua")?.GetValue(okResult.Value, null);

            Assert.Equal("Ezabatuta", mezua);
            mockRepo.Verify(r => r.Delete(It.IsAny<PlaterenOsagaiak>()), Times.Once);
        }

        [Fact]
        public void PO15_EzabatuSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<PlaterenOsagaiakRepository>();
            var existitzenDenElementua = new PlaterenOsagaiak
            {
                PlateraId = 1,
                InbentarioaId = 2,
                Kantitatea = 5
            };

            mockRepo.Setup(r => r.Get(1, 2)).Returns(existitzenDenElementua);
            mockRepo.Setup(r => r.Delete(It.IsAny<PlaterenOsagaiak>())).Throws(new Exception("Errorea"));
            var controller = new PlaterenOsagaiakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Ezabatu(1, 2));
        }
    }
}
