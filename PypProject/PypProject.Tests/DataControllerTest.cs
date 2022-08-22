using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PypProject.Api.Controllers;
using PypProject.Business.Abstract;
using PypProject.Core.Extensions;
using PypProject.Entities.Concrete;
using PypProject.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PypProject.Tests
{
    public class DataControllerTest
    {
        private readonly DataController _controller;
        private readonly IDataService _service;

        public DataControllerTest()
        {
            _service = new DataServiceFake();
            _controller = new DataController(_service);
        }

        [Fact]
        public void UploadFileTest()
        {
            var stream = File.OpenRead("../../../example_data.xlsx");
            FormFile file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = FileExtension.xlxs
            };
            // Act
            var okResult = _controller.Upload(file);
            // Assert
            Assert.IsType<OkObjectResult>(okResult as OkObjectResult);
        }

        [Fact]
        public void GetReportTest()
        {
            var reportDto = new ReportDto
            {
                Type = 1,
                StartDate = new DateTime(2014, 01, 01),
                EndDate = new DateTime(2014, 05, 05),
                AcceptorEmails = new string[] { "asgar.b@code.edu.az" }
            };

            // Act
            var result = _controller.SendReport(reportDto);
            // Assert
            Assert.IsType<OkObjectResult>(result as OkObjectResult);
        }
    }
}
