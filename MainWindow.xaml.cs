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
    public partial class MainWindow : Window
    {
        // We define the core variables that will drive our radar simulation
        private double currentAngle = 0; // keeps track of where the radar beam is pointing
        private Line scannerLine; // the visual representation of the sweeping beam
        private System.Windows.Threading.DispatcherTimer timer; // the heartbeat of our simulation
        private System.Collections.Generic.List<RadarTarget> targetsList; // a roster holding all the targets currently on screen
        private System.Random randomShared = new System.Random(); // a single shared dice for all random events to save memory

        public MainWindow()
        {
            InitializeComponent();

            // First, we prepare the canvas to draw the radar grid as soon as the window loads or resizes
            this.Loaded += (s, e) => DrawRadarBackground();
            this.SizeChanged += (s, e) => DrawRadarBackground();

            // Then, we set up the timer to act as our update loop (running every 20 milliseconds)
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = System.TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();

            // Finally, we populate our radar with a swarm of 36 initial targets
            targetsList = new System.Collections.Generic.List<RadarTarget>();
            for (int i = 0; i < 36; i++)
            {
                // We give each target a random starting angle, distance, and speed using our shared random generator
                double randomAngle = randomShared.Next(0, 360);
                double randomDistance = randomShared.Next(30, 240);
                double randomSpeed = randomShared.Next(2, 10) / 10.0;

                RadarTarget newTarget = new RadarTarget(randomAngle, randomDistance, randomSpeed);

                targetsList.Add(newTarget);

                // We add the target's visual dot and its data tag to the screen
                RadarCanvas.Children.Add(newTarget.VisualElement);
                RadarCanvas.Children.Add(newTarget.DataTag);
            }
        }

        // This method handles drawing the static background of the radar
        private void DrawRadarBackground()
        {
            // We start by wiping the canvas clean to ensure no overlapping drawings
            RadarCanvas.Children.Clear();

            double width = RadarCanvas.ActualWidth;
            double height = RadarCanvas.ActualHeight;

            // If the screen hasn't opened yet, we stop here to prevent errors
            if (height == 0 || width == 0) return;

            // We calculate the center point to anchor our radar circles
            double centerX = width / 2;
            double centerY = height / 2;
            double maxRadius = System.Math.Min(centerX, centerY) - 20;

            // We draw 4 concentric circles to represent the distance zones of the radar
            for (int i = 1; i <= 4; i++)
            {
                double radius = maxRadius * (i / 4.0);

                Ellipse circle = new Ellipse
                {
                    Width = radius * 2,
                    Height = radius * 2,
                    Stroke = Brushes.DarkGreen,
                    StrokeThickness = 2
                };

                // Position the circle exactly at the center of the canvas
                Canvas.SetLeft(circle, centerX - radius);
                Canvas.SetTop(circle, centerY - radius);
                RadarCanvas.Children.Add(circle);
            }

            // We add horizontal and vertical crosshairs to divide the radar into 4 quadrants
            Line horizontalLine = new Line
            {
                X1 = centerX - maxRadius,
                Y1 = centerY,
                X2 = centerX + maxRadius,
                Y2 = centerY,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 1
            };
            RadarCanvas.Children.Add(horizontalLine);

            Line verticalLine = new Line
            {
                X1 = centerX,
                Y1 = centerY - maxRadius,
                X2 = centerX,
                Y2 = centerY + maxRadius,
                Stroke = Brushes.DarkGreen,
                StrokeThickness = 1
            };
            RadarCanvas.Children.Add(verticalLine);
        }

        // This method is the engine of our simulation, called every 20 milliseconds by the timer
        private void Timer_Tick(object sender, System.EventArgs e)
        {
            // --- Step 1: Sweeping the Radar Beam ---

            // We rotate the beam by 2 degrees. If it completes a full circle, we reset it
            currentAngle += 2;
            if (currentAngle >= 360) currentAngle = 0;

            double width = RadarCanvas.ActualWidth;
            double height = RadarCanvas.ActualHeight;
            if (height == 0 || width == 0) return;

            double centerX = width / 2;
            double centerY = height / 2;
            double maxRadius = System.Math.Min(centerY, centerX) - 20;

            // Computers use radians for math, so we convert the angle before applying trigonometry to find the beam's end point
            double radians = currentAngle * (System.Math.PI / 180.0);
            double endX = centerX + (maxRadius * System.Math.Cos(radians));
            double endY = centerY + (maxRadius * System.Math.Sin(radians));

            // We draw the beam on the screen, or just update its coordinates if it already exists
            if (scannerLine == null)
            {
                scannerLine = new Line { X1 = centerX, Y1 = centerY, Stroke = Brushes.LimeGreen, StrokeThickness = 2 };
                RadarCanvas.Children.Add(scannerLine);
            }
            else
            {
                if (!RadarCanvas.Children.Contains(scannerLine))
                {
                    RadarCanvas.Children.Add(scannerLine);
                }
                scannerLine.X1 = centerX;
                scannerLine.Y1 = centerY;
                scannerLine.X2 = endX;
                scannerLine.Y2 = endY;
            }

            // --- Step 2: Updating the Targets (The Swarm) ---

            // Now, we iterate through every target in our roster to update its status
            foreach (RadarTarget target in targetsList)
            {
                // We move the target closer to the center based on its speed
                target.Distance -= target.Speed;

                // To make movement feel natural, we give the target a 5% chance to slightly alter its course
                if (randomShared.NextDouble() < 0.05)
                {
                    double angleVariation = randomShared.Next(-2, 3);
                    target.Angle += angleVariation;

                    // Keep the angle within a valid 0-360 range
                    if (target.Angle < 0) target.Angle += 360;
                    if (target.Angle >= 360) target.Angle -= 360;
                }

                // If the target reaches the center, we respawn it at the edge of the screen to keep the simulation running
                if (target.Distance <= 0)
                {
                    target.Distance = maxRadius;
                    target.Angle = randomShared.Next(0, 360);
                }

                // Threat Assessment: If the target crosses the halfway mark, we flag it as a danger (Red), otherwise it remains a warning (Yellow)
                if (target.Distance < (maxRadius / 2))
                {
                    target.VisualElement.Fill = Brushes.Red;
                }
                else
                {
                    target.VisualElement.Fill = Brushes.Yellow;
                }

                // We calculate the target's exact X and Y coordinates on the screen using its new distance and angle
                double targetRadians = target.Angle * (System.Math.PI / 180.0);
                double targetX = centerX + (target.Distance * System.Math.Cos(targetRadians));
                double targetY = centerY + (target.Distance * System.Math.Sin(targetRadians));

                // We place the target's visual dot on the screen, subtracting 5 to ensure its true center aligns with our coordinates
                if (!RadarCanvas.Children.Contains(target.VisualElement))
                {
                    RadarCanvas.Children.Add(target.VisualElement);
                }
                Canvas.SetLeft(target.VisualElement, targetX - 5);
                Canvas.SetTop(target.VisualElement, targetY - 5);

                // Lastly, we update the data tag with the live distance and position it slightly above and to the right of the target
                target.DataTag.Text = $"{(int)target.Distance}km";

                if (!RadarCanvas.Children.Contains(target.DataTag))
                {
                    RadarCanvas.Children.Add(target.DataTag);
                }
                Canvas.SetLeft(target.DataTag, targetX + 10);
                Canvas.SetTop(target.DataTag, targetY - 15);
            }
        }
    }
}