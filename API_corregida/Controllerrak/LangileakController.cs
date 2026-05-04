using JatetxeaApi.DTOak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class LangileakController : ControllerBase
    {
        private readonly LangileakRepository _repo;

        public LangileakController(LangileakRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Langile guztien zerrenda lortzen du.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var lista = _repo.GetAll().Select(l => new LangileakDto
            {
                Id = l.Id,
                Izena = l.Izena,
                Erabiltzailea = l.Erabiltzailea,
                Pasahitza = l.Pasahitza,
                Aktibo = l.Aktibo,
                ErregistroData = l.ErregistroData,
                RolaId = l.RolaId,
                TxatBaimena = l.TxatBaimena
            });

            return Ok(lista);
        }

        /// <summary>
        /// ID zehatz bateko langilea lortzen du.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var l = _repo.Get(id);
            if (l == null) return NotFound(new { mezua = "Ez da aurkitu" });

            return Ok(new LangileakDto
            {
                Id = l.Id,
                Izena = l.Izena,
                Erabiltzailea = l.Erabiltzailea,
                Pasahitza = l.Pasahitza,
                Aktibo = l.Aktibo,
                ErregistroData = l.ErregistroData,
                RolaId = l.RolaId,
                TxatBaimena = l.TxatBaimena
            });
        }

        /// <summary>
        /// Langile baten login saioa egiten du.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            if (login == null || string.IsNullOrEmpty(login.Erabiltzailea) || string.IsNullOrEmpty(login.Pasahitza))
                return BadRequest(new { mezua = "Erabiltzailea eta pasahitza beharrezkoak dira." });

            var l = _repo.GetAll()
                .FirstOrDefault(x =>
                    x.Erabiltzailea.Equals(login.Erabiltzailea, System.StringComparison.OrdinalIgnoreCase) &&
                    x.Pasahitza.Trim() == login.Pasahitza.Trim());

            if (l == null)
                return Unauthorized(new { mezua = "Erabiltzailea edo pasahitza oker" });

            return Ok(new LangileakDto
            {
                Id = l.Id,
                Izena = l.Izena,
                Erabiltzailea = l.Erabiltzailea,
                Aktibo = l.Aktibo,
                ErregistroData = l.ErregistroData,
                RolaId = l.RolaId,
                TxatBaimena = l.TxatBaimena
            });
        }

        /// <summary>
        /// Langile berria sortzen du.
        /// </summary>
        [HttpPost]
        public IActionResult Sortu([FromBody] LangileakSortuDto dto)
        {
            var l = new Langileak(dto.Izena, dto.Erabiltzailea, dto.Pasahitza, dto.Aktibo, dto.ErregistroData, dto.RolaId, dto.TxatBaimena);
            _repo.Add(l);
            return Ok(new { mezua = "Langilea sortuta", id = l.Id });
        }

        /// <summary>
        /// ID zehatz bateko langilea eguneratzen du.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Eguneratu(int id, [FromBody] LangileakSortuDto dto)
        {
            var l = _repo.Get(id);
            if (l == null) return NotFound(new { mezua = "Ez da aurkitu" });

            l.Izena = dto.Izena;
            l.Erabiltzailea = dto.Erabiltzailea;
            l.Pasahitza = dto.Pasahitza;
            l.Aktibo = dto.Aktibo;
            l.ErregistroData = dto.ErregistroData;
            l.RolaId = dto.RolaId;
            l.TxatBaimena = dto.TxatBaimena;

            _repo.Update(l);
            return Ok(new { mezua = "Eguneratuta" });
        }

        /// <summary>
        /// ID zehatz bateko langilea ezabatzen du.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Ezabatu(int id)
        {
            var l = _repo.Get(id);
            if (l == null) return NotFound(new { mezua = "Ez da aurkitu" });

            _repo.Delete(l);
            return Ok(new { mezua = "Ezabatuta" });
        }
    }
}
