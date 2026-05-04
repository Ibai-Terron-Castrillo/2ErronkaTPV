﻿
using JatetxeaApi.Repositorioak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.DTOak;
using Microsoft.AspNetCore.Mvc;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaterakController : ControllerBase
    {
        private readonly PlaterakRepository _repo;

        public PlaterakController(PlaterakRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Plater guztiak lortzen ditu
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var elementuak = _repo.GetAll();

            var dtoLista = elementuak.Select(e => new PlaterakDto
            {
                Id = e.Id,
                Izena = e.Izena,
                Deskribapena = e.Deskribapena,
                Prezioa = e.Prezioa,
                KategoriaId = e.KategoriaId,
                Erabilgarri = e.Erabilgarri,
                SortzeData = e.SortzeData,
                Irudia = e.Irudia
            });

            return Ok(dtoLista);
        }

        /// <summary>
        /// Platera berri bat sortzen du
        /// </summary>
        [HttpPost]
        public IActionResult Sortu([FromBody] PlaterakSortuDto dto)
        {
            var elementua = new Platerak
            {
                Izena = dto.Izena,
                Deskribapena = dto.Deskribapena,
                Prezioa = dto.Prezioa,
                KategoriaId = dto.KategoriaId,
                Erabilgarri = dto.Erabilgarri,
                SortzeData = DateTime.Now,
                Irudia = dto.Irudia
            };

            _repo.Add(elementua);

            return Ok(new
            {
                mezua = "Platera sortuta",
                id = elementua.Id
            });
        }

        /// <summary>
        /// Id zehatz bateko platera lortzen du
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var e = _repo.Get(id);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            var dto = new PlaterakDto
            {
                Id = e.Id,
                Izena = e.Izena,
                Deskribapena = e.Deskribapena,
                Prezioa = e.Prezioa,
                KategoriaId = e.KategoriaId,
                Erabilgarri = e.Erabilgarri,
                SortzeData = e.SortzeData,
                Irudia = e.Irudia
            };

            return Ok(dto);
        }

        /// <summary>
        /// Id zehatz bateko platera eguneratzen du
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Eguneratu(int id, [FromBody] PlaterakSortuDto dto)
        {
            var e = _repo.Get(id);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            e.Izena = dto.Izena;
            e.Deskribapena = dto.Deskribapena;
            e.Prezioa = dto.Prezioa;
            e.KategoriaId = dto.KategoriaId;
            e.Erabilgarri = dto.Erabilgarri;
            e.Irudia = dto.Irudia;

            _repo.Update(e);

            return Ok(new { mezua = "Eguneratuta" });
        }

        /// <summary>
        /// Id zehatz bateko platera zatika eguneratzen du
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult EguneratuZatia(int id, [FromBody] PlaterakPatchDto dto)
        {
            var e = _repo.Get(id);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            if (dto.Izena != null) e.Izena = dto.Izena;
            if (dto.Deskribapena != null) e.Deskribapena = dto.Deskribapena;
            if (dto.Prezioa.HasValue) e.Prezioa = dto.Prezioa.Value;
            if (dto.KategoriaId.HasValue) e.KategoriaId = dto.KategoriaId.Value;
            if (dto.Erabilgarri != null) e.Erabilgarri = dto.Erabilgarri;
            if (dto.Irudia != null) e.Irudia = dto.Irudia;

            _repo.Update(e);

            return Ok(new { mezua = "Zati batean eguneratuta" });
        }

        /// <summary>
        /// Id zehatz bateko platera ezabatzen du
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
        /// Plateren erabilgarritasuna lortzen du
        /// </summary>
        [HttpGet("disponibilitatea")]
        public IActionResult GetDisponibilitatea()
        {
            var lista = _repo.GetDisponibilitatea();
            return Ok(lista);
        }
    }
}