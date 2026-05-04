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
    public class RolakControllerTests
    {
        [Fact]
        public void R1_GetAllOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var rolak = new List<Rolak>
            {
                new Rolak("Admin"),
                new Rolak("Langilea")
            };

            mockRepo.Setup(r => r.GetAll()).Returns(rolak);
            var controller = new RolakController(mockRepo.Object);

            // Act
            var result = controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRolak = Assert.IsAssignableFrom<IEnumerable<RolakDto>>(okResult.Value);
            var lista = returnedRolak.ToList();

            Assert.Equal(2, lista.Count);
            Assert.Equal("Admin", lista[0].Izena);
            Assert.Equal("Langilea", lista[1].Izena);
        }

        [Fact]
        public void R2_GetAllSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            mockRepo.Setup(r => r.GetAll()).Throws(new Exception("Errorea"));
            var controller = new RolakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.GetAll());
        }

        [Fact]
        public void R3_RolaExistitzenDa_Ok()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var rola = new Rolak("Admin");

            mockRepo.Setup(r => r.Get(1)).Returns(rola);
            var controller = new RolakController(mockRepo.Object);

            // Act
            var result = controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRola = Assert.IsType<RolakDto>(okResult.Value);
            Assert.Equal("Admin", returnedRola.Izena);
        }

        [Fact]
        public void R4_RolaEzExistitzen_NotFound()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            mockRepo.Setup(r => r.Get(1)).Returns((Rolak)null);
            var controller = new RolakController(mockRepo.Object);

            // Act
            var result = controller.Get(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var mezua = notFoundResult.Value.GetType().GetProperty("mezua")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("Ez da aurkitu", mezua);
        }

        [Fact]
        public void R5_GetSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            mockRepo.Setup(r => r.Get(1)).Throws(new Exception("Errorea"));
            var controller = new RolakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Get(1));
        }

        [Fact]
        public void R6_SortuOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var dto = new RolakSortuDto
            {
                Izena = "Sukaldaria"
            };

            mockRepo.Setup(r => r.Add(It.IsAny<Rolak>()));
            var controller = new RolakController(mockRepo.Object);

            // Act
            var result = controller.Sortu(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;

            var mezua = value.GetType().GetProperty("mezua")?.GetValue(value, null);
            Assert.Equal("Rola sortuta", mezua);
        }

        [Fact]
        public void R7_SortuBodyNull_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var controller = new RolakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => controller.Sortu(null));
        }

        [Fact]
        public void R8_SortuSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var dto = new RolakSortuDto
            {
                Izena = "Sukaldaria"
            };

            mockRepo.Setup(r => r.Add(It.IsAny<Rolak>())).Throws(new Exception("Errorea"));
            var controller = new RolakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Sortu(dto));
        }

        [Fact]
        public void R9_RolaEzExistitzen_NotFound()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            mockRepo.Setup(r => r.Get(1)).Returns((Rolak)null);
            var controller = new RolakController(mockRepo.Object);

            // Act
            var result = controller.Eguneratu(1, new RolakSortuDto { Izena = "Berria" });

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var mezua = notFoundResult.Value.GetType().GetProperty("mezua")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("Ez da aurkitu", mezua);
        }

        [Fact]
        public void R10_EguneratuOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var rola = new Rolak("Zaharra");
            var dto = new RolakSortuDto
            {
                Izena = "Berria"
            };

            mockRepo.Setup(r => r.Get(1)).Returns(rola);
            mockRepo.Setup(r => r.Update(It.IsAny<Rolak>()));
            var controller = new RolakController(mockRepo.Object);

            // Act
            var result = controller.Eguneratu(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var mezua = okResult.Value.GetType().GetProperty("mezua")?.GetValue(okResult.Value, null);

            Assert.Equal("Eguneratuta", mezua);
            Assert.Equal("Berria", rola.Izena);
            mockRepo.Verify(r => r.Update(It.IsAny<Rolak>()), Times.Once);
        }

        [Fact]
        public void R11_EguneratuBodyNull_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var rola = new Rolak("Admin");

            mockRepo.Setup(r => r.Get(1)).Returns(rola);
            var controller = new RolakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => controller.Eguneratu(1, null));
        }

        [Fact]
        public void R12_EguneratuSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var rola = new Rolak("Zaharra");
            var dto = new RolakSortuDto
            {
                Izena = "Berria"
            };

            mockRepo.Setup(r => r.Get(1)).Returns(rola);
            mockRepo.Setup(r => r.Update(It.IsAny<Rolak>())).Throws(new Exception("Errorea"));
            var controller = new RolakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Eguneratu(1, dto));
        }

        [Fact]
        public void R13_RolaEzExistitzen_NotFound()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            mockRepo.Setup(r => r.Get(1)).Returns((Rolak)null);
            var controller = new RolakController(mockRepo.Object);

            // Act
            var result = controller.Ezabatu(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var mezua = notFoundResult.Value.GetType().GetProperty("mezua")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("Ez da aurkitu", mezua);
        }

        [Fact]
        public void R14_EzabatuOndo_Ok()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var rola = new Rolak("Admin");

            mockRepo.Setup(r => r.Get(1)).Returns(rola);
            mockRepo.Setup(r => r.Delete(It.IsAny<Rolak>()));
            var controller = new RolakController(mockRepo.Object);

            // Act
            var result = controller.Ezabatu(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var mezua = okResult.Value.GetType().GetProperty("mezua")?.GetValue(okResult.Value, null);

            Assert.Equal("Ezabatuta", mezua);
            mockRepo.Verify(r => r.Delete(It.IsAny<Rolak>()), Times.Once);
        }

        [Fact]
        public void R15_EzabatuSalbuespena_InternalServerError()
        {
            // Arrange
            var mockRepo = new Mock<RolakRepository>();
            var rola = new Rolak("Admin");

            mockRepo.Setup(r => r.Get(1)).Returns(rola);
            mockRepo.Setup(r => r.Delete(It.IsAny<Rolak>())).Throws(new Exception("Errorea"));
            var controller = new RolakController(mockRepo.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => controller.Ezabatu(1));
        }
    }
}