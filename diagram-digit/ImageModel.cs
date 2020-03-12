using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace diagram_digit
{
    public class DiagramImageModel
    {
        public IFormFile Horizontal { get; set; }
        public IFormFile Vertical { get; set; }
    }
}
