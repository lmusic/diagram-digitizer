using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using DiagramDigitizer.Models;

namespace DiagramDigitizer.Services
{
    public class ImageService : IImageService
    {
        private string _pathToSaveDiagram = "C:\\Users\\lmusic\\Desktop\\diplom\\diagramResult.msi";

        public ImageService()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }
        private Bitmap ReadImage()
        {
            var image = new Bitmap("C:\\Users\\lmusic\\Desktop\\diplom\\diagram.png");
            return image;
        }

      private double[] CalculateValuesInPoints(Bitmap image)
        {
            int maxBlueX = 0;
            int maxGreenX = 0;

            var listOfBluePoints = new List<Point>();
            var listOfRedPoints = new List<Point>();
            var result = new double[360];

            for (var i = 0; i < image.Width; i++)
            {
                for (var j = 0; j < image.Height; j++)
                {
                    var pixel = image.GetPixel(i, j);
                    if (pixel.B > 200 && pixel.G < 200 && pixel.R < 200)
                    {
                        listOfBluePoints.Add(new Point(i,j));

                        if (maxBlueX < j)
                        {
                            maxBlueX = j;
                        }
                    }
                    if (pixel.B < 200 && pixel.G > 200 && pixel.R < 200)
                    {
                        if (maxGreenX < j)
                        {
                            maxGreenX = j;
                        }
                    }
                    if (pixel.B < 200 && pixel.G < 200 && pixel.R > 200)
                    {
                        listOfRedPoints.Add(new Point(i, j));
                    }
                }
            }

            var stepOfPixel = Convert.ToDouble(3) / Convert.ToDouble(maxBlueX - maxGreenX);
            var center = FindCenter(listOfRedPoints);
            var lengthOfLineFromCenterToRound = maxBlueX - center.X;
            var valueInCenter = lengthOfLineFromCenterToRound * stepOfPixel;

            listOfBluePoints.ForEach(point =>
            {
                var angle = GetAngleBetweenPoints(center, point);
                var valueInPoint = CalculateValueInPoint(stepOfPixel, valueInCenter, center, point);
                result[Convert.ToInt32(angle)] = valueInPoint;
                
            });
            return result;

        }

        private double CalculateValueInPoint(double stepOfValueForEachPixel, double valueInCenter, Point center, Point point)
        {
            var distanceFromCenterToPoint = GetDistanceBetweenPoints(center, point);
            var valueInPoint = valueInCenter - (distanceFromCenterToPoint * stepOfValueForEachPixel);

            return valueInPoint;
        }


        private double GetAngleBetweenPoints(Point p1, Point p2)
        {
            var angle = Math.Atan2(p1.Y - p2.Y, p1.X - p2.X) / Math.PI * 180;
            return (angle < 0) ? angle + 360 : angle;
        }

        private double GetDistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt((p2.X - p1.X) ^ 2 + (p2.Y - p1.Y) ^ 2);
        }

        private Bitmap ConvertGreenColor(Bitmap image)
        {
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    var pixel = image.GetPixel(i, j);

                }
            }

            return image;
        }

        public Point FindCenter(List<Point> listOfRedPoint)
        {
            var minX = int.MaxValue;
            var maxX = int.MinValue;

            var minY = int.MaxValue;
            var maxY = int.MinValue;

            listOfRedPoint.ForEach(point =>
            {
                if (point.X < minX)
                {
                    minX = point.X;
                }

                if (point.X > maxX)
                {
                    maxX = point.X;
                }

                if (point.Y < minY)
                {
                    minY = point.Y;
                }

                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
            });

            var center = new Point((maxX+minX)/2, (maxY+minY)/2);

            return center;
        }


        private void SaveImage(Bitmap image)
        {
            image.Save(@"C:\\Users\\lmusic\\Desktop\\diplom\\diagramCopy.png", ImageFormat.Png);
        }

        private void GenerateFile(Diagram diagram)
        {
            var textToWrite = "NAME" +" " + diagram.Name + "\r\n";
            textToWrite = textToWrite + "FREQUENCY" + " " + diagram.Frequency + "\r\n";
            textToWrite = textToWrite + "TILT" + " " + diagram.Tilt + "\r\n";
            textToWrite = textToWrite + "COMMENT" + " " + diagram.Comment + "\r\n";
            textToWrite = textToWrite + "HORIZONTAL" + " " + "\r\n";

            for (var i = 0; i < 360; i++)
            {
                textToWrite = textToWrite + i + " " + diagram.Horizontal[i] + "\r\n";
            }

            textToWrite = textToWrite + "VERTICAL" + "\r\n";
            for (var i = 0; i < 360; i++)
            {
                textToWrite = textToWrite + i + " " + diagram.Vertical[i] + "\r\n";
            }

            using var sw = new StreamWriter(_pathToSaveDiagram, false, System.Text.Encoding.Default);

            sw.WriteLine(textToWrite);
        }

        private Diagram GenerateDiagram(double[] horizontal)
        {
            if (horizontal.Length < 0)
            {
                horizontal = new double[360];
            }

            var vertical = new double[360];

            for (var i = 0; i < horizontal.Length; i++)
            {
                horizontal[i] = Math.Sqrt(i);
                vertical[i] = Math.Sqrt(i);
            }
            var diagram = new Diagram("TestName", 1800,horizontal, vertical);

            return diagram;
        }

        public void HighLightImage()
        {
            var image = ReadImage();
            var result = CalculateValuesInPoints(image);
            GenerateFile(GenerateDiagram(result));
        }
    }
}
