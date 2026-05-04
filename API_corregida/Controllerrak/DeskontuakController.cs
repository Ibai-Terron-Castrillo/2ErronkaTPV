using JatetxeaApi.DTOak;
using JatetxeaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;

namespace JatetxeaApi.Controllerrak
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeskontuakController : ControllerBase
    {
        private readonly DeskuntuakRepository _repo;

        public DeskontuakController(DeskuntuakRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("validate")]
        [ProducesResponseType(typeof(DeskuntuEmaitzaDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DeskuntuEmaitzaDto>> Validate([FromBody] DeskuntuKodeaDto? dto)
        {
            try
            {
                return Ok(await _repo.ValidateAsync(dto?.Code));
            }
            catch (Exception ex)
            {
                return Ok(Invalid($"Errorea deskontua balidatzean: {ex.Message}"));
            }
        }

        [HttpPost("apply")]
        [ProducesResponseType(typeof(DeskuntuEmaitzaDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DeskuntuEmaitzaDto>> Apply([FromBody] DeskuntuKodeaDto? dto)
        {
            try
            {
                return Ok(await _repo.ApplyAsync(dto?.Code));
            }
            catch (Exception ex)
            {
                return Ok(Invalid($"Errorea deskontua aplikatzean: {ex.Message}"));
            }
        }

        private static DeskuntuEmaitzaDto Invalid(string message)
        {
            return new DeskuntuEmaitzaDto
            {
                Valid = false,
                Message = message,
                Percentage = 0,
                CodeId = null
            };
        }
    }
}
