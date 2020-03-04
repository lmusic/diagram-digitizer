using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiagramDigitizer.Services
{
    public interface IImageService
    {
        void GetDiagramFile(string name = "new_diagram", int freq = 900, double gain = 17);
    }
}
