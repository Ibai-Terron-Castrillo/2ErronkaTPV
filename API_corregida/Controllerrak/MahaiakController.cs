using JatetxeaApi.Modeloak;

using JatetxeaApi.DTOak;

using JatetxeaApi.Repositorioak;

using Microsoft.AspNetCore.Mvc;

using System;

using System.Linq;
 
namespace JatetxeaApi.Controllerrak

{

    [ApiController]

    [Route("api/[controller]")]

    public class MahaiakController : ControllerBase

    {

        private readonly MahaiakRepository _repo;

        private readonly ErreserbakRepository _erreserbakRepo;
 
        public MahaiakController(MahaiakRepository repo, ErreserbakRepository erreserbakRepo)

        {

            _repo = repo;

            _erreserbakRepo = erreserbakRepo;

        }
 
        [HttpGet]

        public IActionResult GetAll()

        {

            var lista = _repo.GetAll().Select(m => new MahaiakDto

            {

                Id = m.Id,

                MahaiaZbk = m.MahaiaZbk,

                Edukiera = m.Edukiera,

                Egoera = m.Egoera

            });
 
            return Ok(lista);

        }
 
        [HttpGet("{id}")]

        public IActionResult Get(int id)

        {

            var m = _repo.Get(id);

            if (m == null) return NotFound(new { mezua = "Ez da aurkitu" });
 
            return Ok(new MahaiakDto

            {

                Id = m.Id,

                MahaiaZbk = m.MahaiaZbk,

                Edukiera = m.Edukiera,

                Egoera = m.Egoera

            });

        }
 
        [HttpPost]

        public IActionResult Sortu([FromBody] MahaiakSortuDto dto)

        {

            var m = new Mahaiak(dto.MahaiaZbk, dto.Edukiera, dto.Egoera);

            _repo.Add(m);

            return Ok(new { mezua = "Mahai sortuta", id = m.Id });

        }
 
        [HttpPut("{id}")]

        public IActionResult Eguneratu(int id, [FromBody] MahaiakSortuDto dto)

        {

            var m = _repo.Get(id);

            if (m == null) return NotFound(new { mezua = "Ez da aurkitu" });
 
            m.MahaiaZbk = dto.MahaiaZbk;

            m.Edukiera = dto.Edukiera;

            m.Egoera = dto.Egoera;
 
            _repo.Update(m);

            return Ok(new { mezua = "Eguneratuta" });

        }
 
        [HttpDelete("{id}")]

        public IActionResult Ezabatu(int id)

        {

            var m = _repo.Get(id);

            if (m == null) return NotFound(new { mezua = "Ez da aurkitu" });
 
            _repo.Delete(m);

            return Ok(new { mezua = "Ezabatuta" });

        }
 
        [HttpGet("libre")]

        public IActionResult GetLibre([FromQuery] string dataOrdua)

        {

            if (string.IsNullOrWhiteSpace(dataOrdua))

                return BadRequest(new { mezua = "dataOrdua beharrezkoa da" });
 
            if (!DateTime.TryParse(dataOrdua, out var data))

                return BadRequest(new { mezua = "dataOrdua ez da zuzena" });
 
            var erreserbak = _erreserbakRepo.GetAll()

                .Where(e =>

                    e.Egoera != "Kantzelatuta" &&

                    e.ErreserbaData.HasValue &&

                    e.ErreserbaData.Value == data)

                .ToList();
 
            var okupatutakoMahaiIdak = erreserbak

                .Select(e => e.MahaiaId)

                .Distinct()

                .ToHashSet();
 
            var libre = _repo.GetAll()

                .Where(m => !okupatutakoMahaiIdak.Contains(m.Id))

                .Select(m => new MahaiakDto

                {

                    Id = m.Id,

                    MahaiaZbk = m.MahaiaZbk,

                    Edukiera = m.Edukiera,

                    Egoera = m.Egoera

                })

                .OrderBy(m => m.MahaiaZbk);
 
            return Ok(libre);

        }

    }

}
 