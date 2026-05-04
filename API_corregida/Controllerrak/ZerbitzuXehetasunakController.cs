using JatetxeaApi.Repositorioak;
using JatetxeaApi.Modeloak;
using JatetxeaApi.DTOak;
using Microsoft.AspNetCore.Mvc;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZerbitzuXehetasunakController : ControllerBase
    {
        private readonly ZerbitzuXehetasunakRepository _repo;

        public ZerbitzuXehetasunakController(ZerbitzuXehetasunakRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Zerbitzu xehetasunak guztiak lortzen ditu. Bakoitza ZerbitzuXehetasunakDto formatuan itzuliko da, zeinak honako informazioa edukiko duen: Id, ZerbitzuaId, PlateraId, Kantitatea, PrezioUnitarioa eta Zerbitzatuta.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var elementuak = _repo.GetAll();

            var dtoLista = elementuak.Select(e => new ZerbitzuXehetasunakDto
            {
                Id = e.Id,
                ZerbitzuaId = e.ZerbitzuaId,
                PlateraId = e.PlateraId,
                Kantitatea = e.Kantitatea,
                PrezioUnitarioa = e.PrezioUnitarioa,
                Zerbitzatuta = e.Zerbitzatuta
            });

            return Ok(dtoLista);
        }

        /// <summary>
        /// Zerbitzu xehetasun berria sortzen du. Beharrezkoa da ZerbitzuXehetasunakSortuDto formatuan datuak jasotzea, zeinak honako informazioa edukiko duen: ZerbitzuaId, PlateraId, Kantitatea, PrezioUnitarioa eta Zerbitzatuta. Arrakastaz sortu ondoren, "Xehetasuna sortuta" mezua eta sortutako xehetasunaren Id itzuliko dira.
        /// </summary>
        [HttpPost]
        public IActionResult Sortu([FromBody] ZerbitzuXehetasunakSortuDto dto)
        {
            var elementua = new ZerbitzuXehetasunak
            {
                ZerbitzuaId = dto.ZerbitzuaId,
                PlateraId = dto.PlateraId,
                Kantitatea = dto.Kantitatea,
                PrezioUnitarioa = dto.PrezioUnitarioa,
                Zerbitzatuta = dto.Zerbitzatuta
            };

            _repo.Add(elementua);

            return Ok(new
            {
                mezua = "Xehetasuna sortuta",
                id = elementua.Id
            });
        }

        /// <summary>
        /// Id zehatz bateko zerbitzu xehetasuna lortzen du. Id hori URLaren parte gisa jasotzen da. Arrakastaz aurkitzen bada, xehetasunaren informazioa ZerbitzuXehetasunakDto formatuan itzuliko da, zeinak honako informazioa edukiko duen: Id, ZerbitzuaId, PlateraId, Kantitatea, PrezioUnitarioa eta Zerbitzatuta. Ez bada aurkitzen, "Ez da aurkitu" mezua itzuliko da.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var e = _repo.Get(id);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            var dto = new ZerbitzuXehetasunakDto
            {
                Id = e.Id,
                ZerbitzuaId = e.ZerbitzuaId,
                PlateraId = e.PlateraId,
                Kantitatea = e.Kantitatea,
                PrezioUnitarioa = e.PrezioUnitarioa,
                Zerbitzatuta = e.Zerbitzatuta
            };

            return Ok(dto);
        }

        /// <summary>
        /// Id zehatz bateko zerbitzu xehetasuna eguneratzen du. Id hori URLaren parte gisa jasotzen da, eta eguneratzeko datuak ZerbitzuXehetasunakSortuDto formatuan jasotzen dira, zeinak honako informazioa edukiko duen: ZerbitzuaId, PlateraId, Kantitatea, PrezioUnitarioa eta Zerbitzatuta. Arrakastaz eguneratzen bada, "Eguneratuta" mezua itzuliko da. Ez bada aurkitzen, "Ez da aurkitu" mezua itzuliko da.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Eguneratu(int id, [FromBody] ZerbitzuXehetasunakSortuDto dto)
        {
            var e = _repo.Get(id);
            if (e == null)
                return NotFound(new { mezua = "Ez da aurkitu" });

            e.ZerbitzuaId = dto.ZerbitzuaId;
            e.PlateraId = dto.PlateraId;
            e.Kantitatea = dto.Kantitatea;
            e.PrezioUnitarioa = dto.PrezioUnitarioa;
            e.Zerbitzatuta = dto.Zerbitzatuta;

            _repo.Update(e);

            return Ok(new { mezua = "Eguneratuta" });
        }

        /// <summary>
        /// Id zehatz bateko zerbitzu xehetasuna ezabatzen du. Id hori URLaren parte gisa jasotzen da. Arrakastaz ezabatzen bada, "Ezabatuta" mezua itzuliko da. Ez bada aurkitzen, "Ez da aurkitu" mezua itzuliko da.
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
        /// Zerbitzu zehatz bateko xehetasun guztiak lortzen ditu. Zerbitzuaren Id-a URLaren parte gisa jasotzen da. Arrakastaz aurkitzen bada, xehetasunen zerrenda itzuliko da, non bakoitza ZerbitzuXehetasunakDto formatuan egongo den, zeinak honako informazioa edukiko duen: Id, ZerbitzuaId, PlateraId, Kantitatea, PrezioUnitarioa eta Zerbitzatuta. Ez bada aurkitzen, "Ez da aurkitu" mezua itzuliko da.
        /// </summary>
        [HttpGet("zerbitzua/{zerbitzuaId}")]
        public IActionResult GetByZerbitzua(int zerbitzuaId)
        {
            var elementuak = _repo.GetByZerbitzuaId(zerbitzuaId);

            var dtoLista = elementuak.Select(e => new ZerbitzuXehetasunakDto
            {
                Id = e.Id,
                ZerbitzuaId = e.ZerbitzuaId,
                PlateraId = e.PlateraId,
                Kantitatea = e.Kantitatea,
                PrezioUnitarioa = e.PrezioUnitarioa,
                Zerbitzatuta = e.Zerbitzatuta
            });

            return Ok(dtoLista);
        }

        /// <summary>
        /// Id zehatz bateko zerbitzu xehetasunaren "Zerbitzatuta" egoera aldatzen du. Id hori URLaren parte gisa jasotzen da, eta "Zerbitzatuta" egoera aldatzeko datuak ZerbitzatutaPatchDto formatuan jasotzen dira, zeinak honako informazioa edukiko duen: Zerbitzatuta (boolean). Arrakastaz eguneratzen bada, "Egoera aldatu da" mezua itzuliko da. Ez bada aurkitzen, "Ez da aurkitu" mezua itzuliko da.
        /// </summary>
        [HttpPatch("{id}/zerbitzatuta")]
        public IActionResult AldatuZerbitzatuta(int id, [FromBody] ZerbitzatutaPatchDto dto)
        {
            var emaitza = _repo.AldatuZerbitzatutaEtaStock(id, dto.Zerbitzatuta);

            if (!emaitza.Ondo)
                return BadRequest(new { mezua = emaitza.Mezua });

            return Ok(emaitza);
        }
    }
}