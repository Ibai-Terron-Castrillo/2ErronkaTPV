﻿using JatetxeaApi.Modeloak;
using JatetxeaApi.DTOak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class KategoriaController : ControllerBase
    {
        private readonly KategoriaRepository _repo;

        public KategoriaController(KategoriaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Kategoria guztiak itzultzen ditu
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var lista = _repo.GetAll()
                .Select(k => new KategoriaDto
                {
                    Id = k.Id,
                    Izena = k.Izena
                });
            return Ok(lista);
        }

        /// <summary>
        /// Id zehatz bateko kategoria itzultzen du
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var k = _repo.Get(id);
            if (k == null) return NotFound(new { mezua = "Ez da aurkitu" });

            return Ok(new KategoriaDto { Id = k.Id, Izena = k.Izena });
        }

        /// <summary>
        /// Kategoria berri bat sortzen du
        /// </summary>
        [HttpPost]
        public IActionResult Sortu([FromBody] KategoriaSortuDto dto)
        {
            var k = new Kategoria(dto.Izena);
            _repo.Add(k);
            return Ok(new { mezua = "Kategoria sortuta", id = k.Id });
        }

        /// <summary>
        /// Id zehatz bateko kategoria eguneratzen du
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Eguneratu(int id, [FromBody] KategoriaSortuDto dto)
        {
            var k = _repo.Get(id);
            if (k == null) return NotFound(new { mezua = "Ez da aurkitu" });

            k.Izena = dto.Izena;
            _repo.Update(k);
            return Ok(new { mezua = "Eguneratuta" });
        }

        /// <summary> 
        /// Id zehatz bateko kategoria ezabatzen du
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Ezabatu(int id)
        {
            var k = _repo.Get(id);
            if (k == null) return NotFound(new { mezua = "Ez da aurkitu" });

            _repo.Delete(k);
            return Ok(new { mezua = "Ezabatuta" });
        }
    }
}