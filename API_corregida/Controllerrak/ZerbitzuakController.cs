using GastuakApi.DTOak;
using JatetxeaApi.DTOak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZerbitzuakController : ControllerBase
    {
        private readonly ZerbitzuakRepository _repo;

        public ZerbitzuakController(ZerbitzuakRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Zerbitzuen zerrenda osoa lortzen du eta datuak ZerbitzuakDto formatuan itzultzen ditu
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var lista = _repo.GetAll().Select(z => new ZerbitzuakDto
            {
                Id = z.Id,
                LangileId = z.LangileId,
                MahaiaId = z.MahaiaId,
                ErreserbaId = z.ErreserbaId,
                EskaeraData = z.EskaeraData,
                Egoera = z.Egoera,
                Guztira = z.Guztira
            });

            return Ok(lista);
        }

        /// <summary>
        /// Zerbitzu bat lortzen du IDaren arabera, eta datuak ZerbitzuakDto formatuan itzultzen ditu
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var z = _repo.Get(id);
            if (z == null) return NotFound(new { mezua = "Ez da aurkitu" });

            return Ok(new ZerbitzuakDto
            {
                Id = z.Id,
                LangileId = z.LangileId,
                MahaiaId = z.MahaiaId,
                ErreserbaId = z.ErreserbaId,
                EskaeraData = z.EskaeraData,
                Egoera = z.Egoera,
                Guztira = z.Guztira
            });
        }

        /// <summary>
        /// Zerbitzu bat sortzen du, datuak gorputzean jasotzen ditu eta eskaera data gaurkoa jartzen du automatikoki
        /// </summary>
        [HttpPost]
        public IActionResult Sortu([FromBody] ZerbitzuakSortuDto dto)
        {
            var z = new Zerbitzuak(dto.LangileId, dto.MahaiaId, dto.ErreserbaId, DateTime.Now, dto.Egoera, dto.Guztira);
            _repo.Add(z);
            return Ok(new { mezua = "Zerbitzuak sortuta", id = z.Id });
        }

        /// <summary>
        /// Zerbitzu bat eguneratzen du IDaren arabera, datuak gorputzean jasotzen ditu
        /// </summary>  
        [HttpPut("{id}")]
        public IActionResult Eguneratu(int id, [FromBody] ZerbitzuakSortuDto dto)
        {
            var z = _repo.Get(id);
            if (z == null) return NotFound(new { mezua = "Ez da aurkitu" });

            z.LangileId = dto.LangileId;
            z.MahaiaId = dto.MahaiaId;
            z.EskaeraData = dto.EskaeraData;
            z.Egoera = dto.Egoera;
            z.Guztira = dto.Guztira;
            z.ErreserbaId = dto.ErreserbaId;

            _repo.Update(z);
            return Ok(new { mezua = "Eguneratuta" });
        }

        /// <summary>
        /// Zerbitzu bat ezabatzen du IDaren arabera
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Ezabatu(int id)
        {
            var z = _repo.Get(id);
            if (z == null) return NotFound(new { mezua = "Ez da aurkitu" });

            _repo.Delete(z);
            return Ok(new { mezua = "Ezabatuta" });
        }

        [HttpPost("egin")]
        public IActionResult ZerbitzuaEgin([FromBody] ZerbitzuaEskariaDto dto)
        {
            var emaitza = _repo.ZerbitzuaEgin(dto);
            return Ok(emaitza);
        }

        /// <summary>
        /// Erreserba baten platerak lortzen ditu zerbitzuaren xehetasunekin batera 
        /// </summary>
        [HttpGet("erreserba/{erreserbaId}/platerak")]
        public IActionResult GetPlaterakByErreserba(int erreserbaId)
        {
            var zerbitzua = _repo.GetByErreserbaId(erreserbaId);
            if (zerbitzua == null) return NotFound(new { mezua = "Ez dago zerbitzurik erreserba honekin" });

            var xehetasunak = _repo.GetPlaterakLaburpenaByZerbitzuaId(zerbitzua.Id);
            return Ok(xehetasunak);
        }

        /// <summary>
        /// Gaurko zerbitzuak lortzen ditu eta datuak ZerbitzuakDto formatuan itzultzen ditu
        /// </summary>
        [HttpGet("gaur")]
        public IActionResult GetGaur()
        {
            var lista = _repo.GetGaur().Select(z => new ZerbitzuakDto
            {
                Id = z.Id,
                LangileId = z.LangileId,
                MahaiaId = z.MahaiaId,
                ErreserbaId = z.ErreserbaId,
                EskaeraData = z.EskaeraData,
                Egoera = z.Egoera,
                Guztira = z.Guztira
            });

            return Ok(lista);
        }

        [HttpGet("egunekoak")]
        public IActionResult GetEgunekoak()
        {
            var lista = _repo.GetEgunekoak().Select(z => new ZerbitzuakDto
            {
                Id = z.Id,
                LangileId = z.LangileId,
                MahaiaId = z.MahaiaId,
                ErreserbaId = z.ErreserbaId,
                EskaeraData = z.EskaeraData,
                Egoera = z.Egoera,
                Guztira = z.Guztira
            });

            return Ok(lista);
        }

        [HttpGet("erreserba/{erreserbaId}")]
        public IActionResult GetByErreserba(int erreserbaId)
        {
            var z = _repo.GetByErreserbaId(erreserbaId);
            if (z == null) return NotFound(new { mezua = "Ez dago zerbitzurik erreserba honekin" });

            return Ok(new ZerbitzuakDto
            {
                Id = z.Id,
                LangileId = z.LangileId,
                MahaiaId = z.MahaiaId,
                ErreserbaId = z.ErreserbaId,
                EskaeraData = z.EskaeraData,
                Egoera = z.Egoera,
                Guztira = z.Guztira
            });
        }

        [HttpGet("erreserba/{erreserbaId}/laburpena")]
        public IActionResult GetLaburpenaByErreserba(int erreserbaId)
        {
            var lista = _repo.GetLaburpenaByErreserbaId(erreserbaId);
            return Ok(lista);
        }

        [HttpPatch("{id}/egoera")]
        public IActionResult AldatuEgoera(int id, [FromBody] ZerbitzuEgoeraPatchDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Egoera))
                return BadRequest(new { mezua = "Egoera beharrezkoa da" });

            var ondo = _repo.AldatuEgoera(id, dto.Egoera);
            if (!ondo) return NotFound(new { mezua = "Ez da aurkitu" });

            return Ok(new { mezua = "Egoera eguneratuta" });
        }
    }
}