

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using MyApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MyApp.Application.Interfaces.ILogRepository;
using MyApp.Application.Services;
using MyApp.Application.Services.log;



namespace MyApp.API.Controllers
{
    [ApiController]
    [Route("api/logs")]
    //[Authorize]
    [AllowAnonymous]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

     

        [HttpPost("log")]

        [AllowAnonymous]
        public async Task<IActionResult> Log([FromBody] LogEntryWrapper wrapper)
        {
            if (wrapper?.Entry == null)
                return BadRequest("Le log est vide ou mal formé");

            // On passe le vrai DTO au service
            var result = await _logService.CreateLogAsync(wrapper.Entry);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = "Log enregistré avec succès", id = result.Data });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllLogs()
        {
            var result = await _logService.GetAllLogsAsync();

            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(result.Data);
        }

    }
}