﻿using JatetxeaApi.Repositorioak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.DTOak;
using Microsoft.AspNetCore.Mvc;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class InbentarioaController : ControllerBase
    {
        private readonly InbentarioaRepository _repo;

        public InbentarioaController(InbentarioaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Inbentarioan dauden elementu guztiak itzultzen ditu.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var elementuak = _repo.GetAll();

            var dtoLista = elementuak.Select(e => new InbentarioaDto
            {
                Id = e.Id,
                Izena = e.Izena,
                Deskribapena = e.Deskribapena,
                Kantitatea = e.Kantitatea,
                NeurriaUnitatea = e.NeurriaUnitatea,
                StockMinimoa = e.StockMinimoa,
                AzkenEguneratzea = e.AzkenEguneratzea
            });

            return Ok(dtoLista);
        }

        /// <summary>
        /// Inbentarioan elementu berria sortzen du.
        /// </summary>
        [HttpPost]
        public IActionResult Sortu([FromBody] InbentarioaSortuDto dto)
        {
            var elementua = new Inbentarioa
            {
                Izena = dto.Izena,
                Deskribapena = dto.Deskribapena,
                Kantitatea = dto.Kantitatea,
                NeurriaUnitatea = dto.NeurriaUnitatea,
                StockMinimoa = dto.StockMinimoa,
                AzkenEguneratzea = DateTime.Now
            };

            _repo.Add(elementua);

            return Ok(new
            {
                mezua = "Elementua sortuta",
                id = elementua.Id
            });
        }

        /// <summary>
        /// Inbentarioan elementu zehatz bat itzultzen du IDaren arabera.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var e = _repo.Get(id);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            var dto = new InbentarioaDto
            {
                Id = e.Id,
                Izena = e.Izena,
                Deskribapena = e.Deskribapena,
                Kantitatea = e.Kantitatea,
                NeurriaUnitatea = e.NeurriaUnitatea,
                StockMinimoa = e.StockMinimoa,
                AzkenEguneratzea = e.AzkenEguneratzea
            };

            return Ok(dto);
        }

        /// <summary>
        /// Inbentarioan elementu zehatz bat eguneratzen du IDaren arabera. Datu guztiak eguneratzen ditu.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Eguneratu(int id, [FromBody] InbentarioaSortuDto dto)
        {
            var e = _repo.Get(id);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            e.Izena = dto.Izena;
            e.Deskribapena = dto.Deskribapena;
            e.Kantitatea = dto.Kantitatea;
            e.NeurriaUnitatea = dto.NeurriaUnitatea;
            e.StockMinimoa = dto.StockMinimoa;
            e.AzkenEguneratzea = DateTime.Now;

            _repo.Update(e);

            return Ok(new { mezua = "Eguneratuta" });
        }

        /// <summary>
        /// Inbentarioan elementu zehatz bat eguneratzen du IDaren arabera. Datu batzuk eguneratzen ditu, ez denak.
        /// </summary>  
        [HttpPatch("{id}")]
        public IActionResult EguneratuZatia(int id, [FromBody] InbentarioaPatchDto dto)
        {
            var e = _repo.Get(id);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            if (dto.Izena != null) e.Izena = dto.Izena;
            if (dto.Deskribapena != null) e.Deskribapena = dto.Deskribapena;
            if (dto.Kantitatea.HasValue) e.Kantitatea = dto.Kantitatea.Value;
            if (dto.NeurriaUnitatea != null) e.NeurriaUnitatea = dto.NeurriaUnitatea;
            if (dto.StockMinimoa.HasValue) e.StockMinimoa = dto.StockMinimoa.Value;

            e.AzkenEguneratzea = DateTime.Now;

            _repo.Update(e);

            return Ok(new { mezua = "Zati batean eguneratuta" });
        }

        /// <summary>
        /// Inbentarioan elementu zehatz bat ezabatzen du IDaren arabera.
        /// </summary>  
        [HttpDelete("{id}")]
        public IActionResult Ezabatu(int id)
        {
            var e = _repo.Get(id);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            _repo.Delete(e);

            return Ok(new { mezua = "Ezabatuta" });
        }

        /// <summary>
        /// Inbentarioan elementu zehatz baten kantitatea aldatzen du IDaren arabera. Aldaketa positiboa edo negatiboa izan daiteke, eta kantitatea eguneratuko da horren arabera. Adibidez, -5 baldin bada, kantitatea 5 unitate murriztuko da. Arrakastaz eguneratzen bada, eguneratutako elementuaren informazioa itzuliko da. Ez bada aurkitzen, "Ez da aurkitu" mezua itzuliko da. Ez bada ondo aldatzen, "Ezin izan da kantitatea aldatu" mezua itzuliko da.
        /// </summary>
        [HttpPatch("{id}/kantitatea")]
        public IActionResult AldatuKantitatea(int id, [FromBody] KantitateaAldatuDto dto)
        {
            var emaitza = _repo.AldatuKantitatea(id, dto.Aldaketa);

            if (!emaitza.Ondo)
                return BadRequest(new { mezua = emaitza.Mezua });

            return Ok(emaitza);
        }
    }
}