﻿using JatetxeaApi.Modeloak;
using JatetxeaApi.DTOak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolakController : ControllerBase
    {
        private readonly RolakRepository _repo;

        public RolakController(RolakRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Rolak guztiak lortzen ditu
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var lista = _repo.GetAll().Select(r => new RolakDto
            {
                Id = r.Id,
                Izena = r.Izena
            });

            return Ok(lista);
        }

        /// <summary>
        /// Id bidez rola bat lortzen du
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var r = _repo.Get(id);
            if (r == null) return NotFound(new { mezua = "Ez da aurkitu" });

            return Ok(new RolakDto
            {
                Id = r.Id,
                Izena = r.Izena
            });
        }

        /// <summary>
        /// Rola bat sortzen du
        /// </summary>
        [HttpPost]
        public IActionResult Sortu([FromBody] RolakSortuDto dto)
        {
            var r = new Rolak(dto.Izena);
            _repo.Add(r);
            return Ok(new { mezua = "Rola sortuta", id = r.Id });
        }

        /// <summary>
        /// Id bidez rola bat eguneratzen du
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Eguneratu(int id, [FromBody] RolakSortuDto dto)
        {
            var r = _repo.Get(id);
            if (r == null) return NotFound(new { mezua = "Ez da aurkitu" });

            r.Izena = dto.Izena;
            _repo.Update(r);
            return Ok(new { mezua = "Eguneratuta" });
        }

        /// <summary>
        /// Id bidez rola bat ezabatzen du
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Ezabatu(int id)
        {
            var r = _repo.Get(id);
            if (r == null) return NotFound(new { mezua = "Ez da aurkitu" });

            _repo.Delete(r);
            return Ok(new { mezua = "Ezabatuta" });
        }
    }
}