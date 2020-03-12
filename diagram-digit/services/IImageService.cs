using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diagram_digit.Models;

namespace diagram_digit.services
{
    public interface IImageService
    {
        Task<Diagram> GetDiagramModel(string horizontalPath, string verticalPath, string name = "new_diagram", int freq = 900, double gain = 17);
        Task<Dictionary<string, string>> CreateImageFiles(DiagramImageModel imageModel);

        string DownloadDiagramFile(int id);
    }
}
