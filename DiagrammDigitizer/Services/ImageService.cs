using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using DiagramDigitizer.Models;

namespace DiagramDigitizer.Services
{
    public class ImageService : IImageService
    {
        private string _pathToSaveDiagram = "C:\\Users\\lmusic\\Desktop\\diplom\\diagramResultVertical.msi";

        public ImageService()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        private double[] CalculateValuesInPoints(Bitmap image)
        {
            // remove shadows, divide all colors for black white and blue;
            // white - background
            // blue point on the graph
            // black - coordinate grid
            image = PrepareImage(image);

            //points to make graph
            var listOfBluePoints = GetBluePoints(image);

            // center - center of the diagram,
            // pointOnRound - point lying on the outer circle in left part of diagram, 
            // threeDBMLevelPoint - point lying on 3dbm circle in left part of diagram
            // y-coordinate should be equals for each point

            var center = FindCenter(image);
            var pointOnRound = FindPointOnRound(image, center);
            var threeDBMLevelPoint = Find3DBMLevelPoint(pointOnRound, center, image);

            image.SetPixel(center.X, center.Y, Color.Red);
            image.SetPixel(pointOnRound.X, pointOnRound.Y, Color.Red);
            image.SetPixel(threeDBMLevelPoint.X, threeDBMLevelPoint.Y, Color.Red);

            SaveImage(image);

            var lengthOfLineFromCenterToRound = center.X - pointOnRound.X;
            var stepOfPixel = Convert.ToDouble(3) / Convert.ToDouble(threeDBMLevelPoint.X - pointOnRound.X);
            var listOfValuesForAngle = new SortedList<int, List<double>>();

            listOfBluePoints.ForEach(point =>
            {
                var angle = GetAngleBetweenPoints(center, point);

                var valueInPoint = CalculateValueInPoint(lengthOfLineFromCenterToRound, stepOfPixel, center, point);
                var i = Convert.ToInt32(angle);
                if (listOfValuesForAngle.ContainsKey(i))
                {
                    listOfValuesForAngle[i].Add(valueInPoint);
                }
                else
                {
                    listOfValuesForAngle.Add(i, new List<double>{valueInPoint});
                }
            });

            var result = new double[360];

            foreach (var item in listOfValuesForAngle)
            {
                double average = 0;
                item.Value.ForEach(value => { average += value; });
                average /= item.Value.Count;
                result[item.Key == 360 ? 0 : item.Key] = average;
            }

            for (var i = 1; i < result.Length; i++)
            {
                if (result[i] <= 0)
                {
                    result[i] = InterpolateValue(i, result);
                }
            }
            return result;

        }

      public void HighLightImage()
        {
            var image = ReadImage();
            var result = CalculateValuesInPoints(image);
            var diagram = GenerateDiagram(result);
            GenerateFile(diagram);
        }

      //private DiagramImageProperty GetDiagramImageProperty(Bitmap image)
      //{
            
      //}

        private Bitmap PrepareImage(Bitmap image)
      {
          for (var i = 0; i < image.Height; i++)
          {
              for (var j = 0; j < image.Width; j++)
              {
                  var pixel = image.GetPixel(j, i);

                  if (pixel.B > 200 && pixel.G > 200 && pixel.R > 200)
                  {
                      image.SetPixel(j, i, Color.White);
                  }
                  else
                  {
                      if (pixel.B > 240 && pixel.G < 50 && pixel.R < 50)
                      {
                          image.SetPixel(j, i, Color.Blue);
                      }
                      else
                      {
                          image.SetPixel(j,i,Color.Black);
                      }
                  }
              }
          }

          return image;
      }
        //method to find center of the diagram
        private Point FindCenter(Bitmap image)
        {
            var maxCountOfBlackPixelsInLine = int.MinValue;

            var x = 0;
            var y = 0;

            for (var i = 0; i < image.Height; i++)
            {
                var counter = 0;
                for (var j = 0; j < image.Width; j++)
                {
                    
                    if (image.GetPixel(j, i).ToArgb() == Color.Black.ToArgb())
                    {
                        counter++;
                    }
                }

                if (counter <= maxCountOfBlackPixelsInLine) continue;

                maxCountOfBlackPixelsInLine = counter;
                y = i;
            }

            maxCountOfBlackPixelsInLine = int.MinValue;

            for (var i = 0; i < image.Width; i++)
            {
                var counter = 0;
                for (var j = 0; j < image.Height; j++)
                {

                    if (image.GetPixel(i, j).ToArgb() == Color.Black.ToArgb())
                    {
                        counter++;
                    }
                }

                if (counter <= maxCountOfBlackPixelsInLine) continue;

                maxCountOfBlackPixelsInLine = counter;
                x = i;
            }
            
            var center = new Point(x, y);

            return center;
        }

        private List<Point> GetBluePoints(Bitmap image)
        {
            var listOfBluePoints = new List<Point>();

            for (var i = 0; i < image.Height; i++)
            {
                for (var j = 0; j < image.Width; j++)
                {
                    var pixel = image.GetPixel(j, i);

                    if (pixel.ToArgb() == Color.Blue.ToArgb())
                    {
                        listOfBluePoints.Add(new Point(j, i));

                    }
                }
            }

            return listOfBluePoints;
        }

        private Point FindPointOnRound(Bitmap image, Point center)
        {
            var y = center.Y;
            var blackPixelSectionsDictionary = new Dictionary<int,int>();
            var index = 0;
            for (int i = 0; i < center.X; i++)
            {
                var pixel = image.GetPixel(i, y);
                if (pixel.ToArgb() == Color.Black.ToArgb())
                {
                    if (blackPixelSectionsDictionary.ContainsKey(index))
                    {
                        blackPixelSectionsDictionary[index]++;
                    }
                    else
                    {
                        blackPixelSectionsDictionary.Add(index, 1);
                    }
                }
                else
                {
                    index = i + 1;
                }
            }

            var indexOfMaxValue = blackPixelSectionsDictionary
                .AsQueryable()
                .FirstOrDefault(x => x.Value == blackPixelSectionsDictionary.Values.Max()).Key;

            return new Point(indexOfMaxValue, y);
        }

        private Point Find3DBMLevelPoint(Point pointOnRound, Point center, Bitmap image)
        {
            for (var i = pointOnRound.X + 2; i < center.X; i++)
            {
                var point = new Point(i, center.Y);
                if (PixelsAboveIsBlack(image, point) &&
                    PixelsBelowIsBlack(image, point))
                {
                    return point;
                }
            }

            throw new Exception("cannot find 3db level");
        }

        private bool PixelsAboveIsBlack(Bitmap image, Point point)
        {
            for (var i = point.Y; i < point.Y + 5; i++)
            {
                if (image.GetPixel(point.X, i).ToArgb() != Color.Black.ToArgb())
                {
                    return false;
                }
            }

            return true;
        }

        private bool PixelsBelowIsBlack(Bitmap image, Point point)
        {
            for (var i = point.Y; i < point.Y - 5; i++)
            {
                if (image.GetPixel(point.X, i).ToArgb() != Color.Black.ToArgb())
                {
                    return false;
                }
            }

            return true;
        }

        //returns value in point based on value in nearest neighbors
        private double InterpolateValue(int index, double[] array)
        {
            double prevValue = 0;
            double nextValue = 0;
            double nextValueIndex = 0;
            double prevValueIndex = 0;

            for (var i = index + 1; i < array.Length; i++)
            {
                if (array[i] <= 0) continue;

                nextValue = array[i];
                nextValueIndex = i;

                break;
            }

            for (var i = index - 1; i > 0; i--)
            {
                if (array[i] <= 0) continue;
                prevValue = array[i];
                prevValueIndex = i;
                break;
            }

            var result = (((nextValue - prevValue) / (nextValueIndex + prevValueIndex)) * (index - prevValueIndex));

            return Math.Abs(prevValue < nextValue ? (prevValue + result) : (prevValue - result));

        }

        private double CalculateValueInPoint(double distanceFromCenterToRound, double stepOfValueForEachPixel, Point center, Point point)
        {
            var distanceFromCenterToPoint = GetDistanceBetweenPoints(center, point);
            var valueInPoint = (distanceFromCenterToRound - distanceFromCenterToPoint) * stepOfValueForEachPixel;

            return valueInPoint;
        }

        //X-axis angle
        private double GetDistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(((p2.X - p1.X) * (p2.X - p1.X)) + ((p2.Y - p1.Y) * (p2.Y - p1.Y)));
        }

        private double GetAngleBetweenPoints(Point p1, Point p2)
        {
            var angle = 180 - (Math.Atan2(p1.Y - p2.Y, p1.X - p2.X) / Math.PI * 180);
            return (angle < 0) ? angle + 360 : angle;
        }

        // create file .msi and write diagram data to this file
        private void GenerateFile(Diagram diagram)
        {
            var textToWrite = "NAME" + " " + diagram.Name + "\r\n";
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

        //generate diagram for test temp test method
        private Diagram GenerateDiagram(double[] horizontal)
        {
            if (horizontal.Length < 0)
            {
                horizontal = new double[360];
            }

            var vertical = new double[360];

            for (var i = 0; i < horizontal.Length; i++)
            {
                vertical[i] = Math.Sqrt(i);
            }

            var diagram = new Diagram("TestName", 1800, horizontal, vertical);

            return diagram;
        }

        private void SaveImage(Bitmap image)
        {
            image.Save(@"C:\\Users\\lmusic\\Desktop\\diplom\\diagram-VerticalCopy.png", ImageFormat.Png);
        }
        private Bitmap ReadImage()
        {
            var image = new Bitmap("C:\\Users\\lmusic\\Desktop\\diplom\\diagram-vertical.png");
            return image;
        }

    }
}
