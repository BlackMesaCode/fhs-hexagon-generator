using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HexagonGenerator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Canvas _canvas;
        private double _hexagonSideLength = 10.0;
        private int _hexagonSideCount = 10;
        private int _oxigenReducerCount = 40;

        private int _hexagonMaxLengthCount { get { return 2 * _hexagonSideCount - 1; } }

        public MainWindow()
        {
            InitializeComponent();

            CreateCanvas();

            DrawGrid();

            DistributeOxigenReducers();

        }

        public void CreateCanvas()
        {
            var canvasLength = 2 * _hexagonSideCount * _hexagonSideLength * Math.Sqrt(3.0);

            _canvas = new Canvas();
            _canvas.Width = canvasLength;
            _canvas.Height = canvasLength * (Math.Sqrt(3.0) / 2.0);
            //_canvas.Background = new SolidColorBrush(Colors.LightGray);
            _canvas.Margin = new Thickness(10.0);

            Panel.Children.Add(_canvas);
        }

        public void DrawPoint(double x, double y)
        {
            var ellipse = new Ellipse();
            ellipse.Width = 2;
            ellipse.Height = 2;
            ellipse.Fill = Brushes.Black;

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);

            _canvas.Children.Add(ellipse);
        }

        public void DrawHexagonAbsolute(double x, double y, double distanceToMainAxis)
        {
            Polygon hexagon = new Polygon();
            hexagon.Stroke = Brushes.Black;
            hexagon.Fill = Brushes.LightBlue;
            hexagon.StrokeThickness = 1;
            hexagon.Points = new PointCollection() { new Point(0, _hexagonSideLength / 2),
                                               new Point((Math.Sqrt(3) * _hexagonSideLength)/2, 0),
                                               new Point(Math.Sqrt(3) * _hexagonSideLength, _hexagonSideLength / 2),
                                               new Point(Math.Sqrt(3) * _hexagonSideLength, 1.5 * _hexagonSideLength),
                                               new Point((Math.Sqrt(3) * _hexagonSideLength)/2, 2* _hexagonSideLength),
                                               new Point(0, 1.5 * _hexagonSideLength),

            };

            hexagon.DataContext = CalculateProbabilityFactor(distanceToMainAxis);

            Canvas.SetLeft(hexagon, x);
            Canvas.SetTop(hexagon, y);

            _canvas.Children.Add(hexagon);
        }

        public double CalculateProbabilityFactor(double distanceToMainAxis)
        {
            var variance = 1.0;
            return (1 / (variance * Math.Sqrt(2 * Math.PI))) * Math.Pow(Math.E, -0.5 * Math.Pow(distanceToMainAxis / variance, 2.0));
        }

        public void DistributeOxigenReducers()
        {
            var rnd = new Random();
            var results = new Dictionary<Polygon, double>();
            foreach (Polygon hexagon in _canvas.Children)
            {
                var score = rnd.Next(1, 10) * (1 + (double)hexagon.DataContext);
                results.Add(hexagon, score);
            }

            foreach (var result in results.OrderByDescending(pair => pair.Value).Take(_oxigenReducerCount))
            {
                var hexagon = result.Key;
                hexagon.Fill = Brushes.Red;
            }
        }

        public void DrawHexagonOnGrid(int rowIndex, int colIndex)
        {
            double x;
            if (rowIndex % 2 == 0) // rowIndex is even
            {
                x = ((_hexagonSideLength * Math.Sqrt(3.0)) / 2.0) + (colIndex * _hexagonSideLength * Math.Sqrt(3.0));
            }
            else // rowIndex is odd
            {
                x = (_hexagonSideLength * Math.Sqrt(3.0)) * (colIndex + 1);
            }

            double y = _hexagonSideLength + rowIndex * 1.5 * _hexagonSideLength;

            var distanceToMainAxis = Math.Abs(Math.Round(_hexagonMaxLengthCount / 2.0, MidpointRounding.ToEven) - rowIndex);

            DrawHexagonAbsolute(x - ((_hexagonSideLength * Math.Sqrt(3.0)) / 2.0), y - _hexagonSideLength, distanceToMainAxis);
        }
        

        public void DrawGrid()
        {
            var currentHexagonRowCount = _hexagonSideCount;
            for (int rowIndex = 0; rowIndex < _hexagonMaxLengthCount; rowIndex++)
            {
                int indexOffset;
                if (rowIndex % 2 == 0)
                {
                    indexOffset = (int)Math.Round((_hexagonMaxLengthCount - currentHexagonRowCount) / 2.0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    indexOffset = (int)Math.Round((_hexagonMaxLengthCount - currentHexagonRowCount) / 2.0, MidpointRounding.ToEven); 
                }
                for (int colIndex = 0; colIndex < currentHexagonRowCount; colIndex++)
                {
                    DrawHexagonOnGrid(rowIndex, colIndex + indexOffset);
                }
                if (rowIndex >= _hexagonMaxLengthCount / 2)
                    currentHexagonRowCount--;
                else
                    currentHexagonRowCount++;
            }
        }

        public void Reset()
        {
            foreach (Polygon hexagon in _canvas.Children)
            {
                hexagon.Fill = Brushes.LightBlue;
            }
        }

        private void GenerateOxigenReducers_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            DistributeOxigenReducers();
        }
    }
}
