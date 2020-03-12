using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using diagram_digit.Models;
using diagram_digit.services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace diagram_digit.Controllers
{
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [Route("api/v1/image/upload")]
        [HttpPost]
        public async Task<Diagram> UploadImage(DiagramImageModel imageModel)
        {
            var path = await _imageService.CreateImageFiles(imageModel);
            return await _imageService.GetDiagramModel(path["Horizontal"], path["Vertical"]);
        }
    
        [Route("api/v1/image/download")]
        [HttpGet]
        public async Task<IActionResult> DownloadImage()
        {
            string file_path = _imageService.DownloadDiagramFile(0);

            string file_type = "application/pdf";

            string file_name = "diagram.pdf";

            return PhysicalFile(file_path, file_type, file_name);
        }
    }
}