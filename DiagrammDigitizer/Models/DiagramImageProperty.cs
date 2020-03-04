using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace DiagramDigitizer.Models
{
    public class DiagramImageProperty
    {
        public Point Center { get; set; }
        public int Radius { get; set; }
        public double StepOfChangingValueForPixel { get; set; }
    }
}
