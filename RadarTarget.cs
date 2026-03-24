using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RadarSimulator
{
    // this class represents a target(drone, plane, etc.) that can be detected by the radar
    internal class RadarTarget
    {
        public double Angle { get; set; } // the angle of the target 
        public double Distance { get; set; } // the distance of the target from the center of the radar
        public Ellipse VisualElement { get; set; } // the shape that will be drawn on the radar
        public double Speed { get; set; } // the speed of the target
        public TextBlock DataTag { get; set; } // a text block to display the target's data when detected

        public RadarTarget(double startAngle, double startDistance, double speed) // constructor to initialize the target, we give it a starting angle and distance
        {
            Angle = startAngle;
            Distance = startDistance;
            Speed = speed;

            VisualElement = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red
            };

            DataTag = new TextBlock
            {
                Foreground = Brushes.LimeGreen,
                FontSize = 10,
                FontWeight = FontWeights.Bold
            };
        }
    }
}
