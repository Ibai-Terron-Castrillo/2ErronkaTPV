using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using JatetxeaApi.Controllerrak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.Repositorioak;
using JatetxeaApi.DTOak;
using System.Collections.Generic;
using System.Linq;

namespace JatetxeaApi.Testak
{
    public class PlaterakControllerTests
    {
        [Fact]
        public void GetAll_OkItzultzenDu_PlaterakExistitzenDirenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            var platerak = new List<Platerak>
            {
                new Platerak { Id = 1, Izena = "Plater1", Deskribapena = "Desk1", Prezioa = 10, KategoriaId = 1, Erabilgarri = "Bai", SortzeData = System.DateTime.Now, Irudia = "img1.jpg" },
                new Platerak { Id = 2, Izena = "Plater2", Deskribapena = "Desk2", Prezioa = 15, KategoriaId = 2, Erabilgarri = "Ez", SortzeData = System.DateTime.Now, Irudia = "img2.jpg" }
            };
            mockRepo.Setup(r => r.GetAll()).Returns(platerak);

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtoList = Assert.IsAssignableFrom<IEnumerable<PlaterakDto>>(okResult.Value);
            Assert.Equal(2, dtoList.Count());
        }

        [Fact]
        public void Get_OkItzultzenDu_PlateraExistitzenDenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            var platera = new Platerak { Id = 5, Izena = "Plater5", Prezioa = 20 };
            mockRepo.Setup(r => r.Get(5)).Returns(platera);

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.Get(5);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PlaterakDto>(okResult.Value);
            Assert.Equal(5, dto.Id);
        }

        [Fact]
        public void Get_NotFoundItzultzenDu_PlateraEzExistitzenDenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            mockRepo.Setup(r => r.Get(999)).Returns((Platerak)null);

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.Get(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void Sortu_OkItzultzenDu_PlateraOndoSortzenDenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            var dto = new PlaterakSortuDto
            {
                Izena = "Plater Berria",
                Deskribapena = "Deskribapena",
                Prezioa = 12.5m,
                KategoriaId = 1,
                Erabilgarri = "Bai",
                Irudia = "irudia.jpg"
            };
            Platerak savedPlater = null;
            mockRepo.Setup(r => r.Add(It.IsAny<Platerak>())).Callback<Platerak>(p => savedPlater = p);

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Platera sortuta", value.mezua);
            Assert.NotNull(savedPlater);
            Assert.Equal(dto.Izena, savedPlater.Izena);
        }

        [Fact]
        public void Eguneratu_OkItzultzenDu_PlateraExistitzenDenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            var platera = new Platerak { Id = 5, Izena = "Plater5", Deskribapena = "Desk", Prezioa = 20, KategoriaId = 1, Erabilgarri = "Bai", Irudia = "img.jpg" };
            mockRepo.Setup(r => r.Get(5)).Returns(platera);
            var dto = new PlaterakSortuDto
            {
                Izena = "PlaterUpdated",
                Deskribapena = "DeskUpdated",
                Prezioa = 25,
                KategoriaId = 2,
                Erabilgarri = "Ez",
                Irudia = "img2.jpg"
            };

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.Eguneratu(5, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Eguneratuta", value.mezua);
            mockRepo.Verify(r => r.Update(platera), Times.Once);
        }

        [Fact]
        public void Eguneratu_NotFoundItzultzenDu_PlateraEzExistitzenDenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            mockRepo.Setup(r => r.Get(999)).Returns((Platerak)null);
            var dto = new PlaterakSortuDto();

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.Eguneratu(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void EguneratuZatia_OkItzultzenDu_PlateraExistitzenDenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            var platera = new Platerak { Id = 5, Izena = "Plater5", Deskribapena = "Desk", Prezioa = 20, KategoriaId = 1, Erabilgarri = "Bai", Irudia = "img.jpg" };
            mockRepo.Setup(r => r.Get(5)).Returns(platera);
            var dto = new PlaterakPatchDto
            {
                Izena = "IzenaBerria",
                Prezioa = 30,
                KategoriaId = 3
            };

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.EguneratuZatia(5, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Zati batean eguneratuta", value.mezua);
            mockRepo.Verify(r => r.Update(platera), Times.Once);
            Assert.Equal("IzenaBerria", platera.Izena);
            Assert.Equal(30, platera.Prezioa);
            Assert.Equal(3, platera.KategoriaId);
            Assert.Equal("Desk", platera.Deskribapena);
        }

        [Fact]
        public void EguneratuZatia_NotFoundItzultzenDu_PlateraEzExistitzenDenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            mockRepo.Setup(r => r.Get(999)).Returns((Platerak)null);
            var dto = new PlaterakPatchDto { Izena = "NewName" };

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.EguneratuZatia(999, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }

        [Fact]
        public void Ezabatu_OkItzultzenDu_PlateraExistitzenDenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            var platera = new Platerak { Id = 5, Izena = "Plater5" };
            mockRepo.Setup(r => r.Get(5)).Returns(platera);

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.Ezabatu(5);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value;
            Assert.Equal("Ezabatuta", value.mezua);
            mockRepo.Verify(r => r.Delete(platera), Times.Once);
        }

        [Fact]
        public void Ezabatu_NotFoundItzultzenDu_PlateraEzExistitzenDenean()
        {
            var mockRepo = new Mock<PlaterakRepository>();
            mockRepo.Setup(r => r.Get(999)).Returns((Platerak)null);

            var controller = new PlaterakController(mockRepo.Object);

            var result = controller.Ezabatu(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            dynamic value = notFound.Value;
            Assert.Equal("Ez da aurkitu", value.mezua);
        }
    }
}  