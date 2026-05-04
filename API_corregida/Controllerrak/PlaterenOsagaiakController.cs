﻿using JatetxeaApi.Modeloak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaterenOsagaiakController : ControllerBase
    {
        private readonly PlaterenOsagaiakRepository _repo;

        public PlaterenOsagaiakController(PlaterenOsagaiakRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Plateren osagaiak guztiak lortzen ditu
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var elementuak = _repo.GetAll();
            return Ok(elementuak);
        }

        /// <summary>
        /// Plateren osagai zehatz bat lortzen du plateraId eta inbentarioaId erabiliz
        /// </summary>
        [HttpGet("{plateraId}/{inbentarioaId}")]
        public IActionResult Get(int plateraId, int inbentarioaId)
        {
            var e = _repo.Get(plateraId, inbentarioaId);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            return Ok(e);
        }

        /// <summary>
        /// Plateren osagai berria sortzen du plateraId eta inbentarioaId erabiliz
        /// </summary>
        [HttpPost]
        public IActionResult Sortu([FromBody] PlaterenOsagaiak dto)
        {
            var elementua = new PlaterenOsagaiak
            {
                PlateraId = dto.PlateraId,
                InbentarioaId = dto.InbentarioaId,
                Kantitatea = dto.Kantitatea
            };

            _repo.Add(elementua);

            return Ok(new
            {
                mezua = "Elementua sortuta",
                platareaId = elementua.PlateraId,
                inbentarioaId = elementua.InbentarioaId
            });
        }

        /// <summary>
        /// Plateren osagai zehatz bat eguneratzen du plateraId eta inbentarioaId erabiliz
        /// </summary>
        [HttpPut("{plateraId}/{inbentarioaId}")]
        public IActionResult Eguneratu(int plateraId, int inbentarioaId, [FromBody] PlaterenOsagaiak dto)
        {
            var e = _repo.Get(plateraId, inbentarioaId);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            e.Kantitatea = dto.Kantitatea;
            _repo.Update(e);

            return Ok(new { mezua = "Eguneratuta" });
        }

        /// <summary>
        /// Plateren osagai zehatz bat ezabatzen du plateraId eta inbentarioaId erabiliz
        /// </summary>
        [HttpDelete("{plateraId}/{inbentarioaId}")]
        public IActionResult Ezabatu(int plateraId, int inbentarioaId)
        {
            var e = _repo.Get(plateraId, inbentarioaId);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            _repo.Delete(e);
            return Ok(new { mezua = "Ezabatuta" });
        }
    }
}