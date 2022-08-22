using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PypProject.Business.Abstract;
using PypProject.Core.Utilities.Results;
using PypProject.DataAccess.Abstract;
using PypProject.DataAccess.Concrete.EntityFramework;
using PypProject.Entities.DTOs;
using System;
using System.Threading.Tasks;

namespace PypProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            var result = _dataService.Upload(file);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public IActionResult SendReport([FromQuery] ReportDto reportDto)
        {
            var result = _dataService.SendReport(reportDto);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
