namespace DiagramDigitizer.Models
{
    public class Diagram
    {
        public string Name { get; private set; } = "";

        public int Frequency { get; private set; }
        public double Gain { get; private set; }
        public string Tilt { get; private set; } = "Electrical";

        public string Comment { get; private set; } = "";

        public double[] Horizontal { get; private set; } = new double[360];
        
        public double[] Vertical { get; private set; } = new double[360];

        protected Diagram()
        {

        }

        public Diagram(string name, int freq, double[] horizontal, double[] vertical, double gain, string tilt = "",
            string comment = "")
        {
            Name = name;
            Frequency = freq;
            Gain = gain;
            Horizontal = horizontal;
            Vertical = vertical;
            if (tilt != "")
            {
                Tilt = tilt;
            }
            else
            {
                Tilt = "Electrical";
            }

            if (comment != "")
            {
                Comment = comment;
            }
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetFrequency(int freq)
        {
            Frequency = freq;
        }

        public void SetTilt(string tilt)
        {
            Tilt = tilt;
        }

        public void SetComment(string comment)
        {
            Comment = comment;
        }

        public void SetHorizontal(double[] horizontal)
        {
            Horizontal = horizontal;
        }

        public void SetVertical(double[] vertical)
        {
            Vertical = vertical;
        }
    }
}
