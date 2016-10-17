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
        private double _hexagonSideLength = 50.0;
        private int _hexagonSideCount = 4;
        public int _hexagonMaxLengthCount { get { return 2 * _hexagonSideCount - 1;  } }

        public MainWindow()
        {
            InitializeComponent();

            CreateCanvas();

            DrawGrid();

        }

        public void CreateCanvas()
        {
            var canvasLength = (2 * _hexagonSideCount - 1) * _hexagonSideLength * Math.Sqrt(3.0);

            _canvas = new Canvas();
            _canvas.Width = canvasLength;
            _canvas.Height = canvasLength;
            _canvas.Background = new SolidColorBrush(Colors.LightGray);
            _canvas.Margin = new Thickness(10.0);

            MainGrid.Children.Add(_canvas);
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

        public void DrawHexagonAbsolute(double x, double y)
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

            Canvas.SetLeft(hexagon, x);
            Canvas.SetTop(hexagon, y);

            _canvas.Children.Add(hexagon);
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

            DrawHexagonAbsolute(x, y);
        }

        public void DrawGrid()
        {
            var currentHexagonRowCount = _hexagonSideCount;
            for (int rowIndex = 0; rowIndex < _hexagonMaxLengthCount; rowIndex++)
            {
                for (int colIndex = 0; colIndex < _hexagonMaxLengthCount; colIndex++)
                {
                    var margin = Math.Round((GetHexagonMaxCountByRow(rowIndex) - currentHexagonRowCount) / 2.0, MidpointRounding.AwayFromZero);

                    if (colIndex >= margin && colIndex < _hexagonMaxLengthCount - margin)
                        DrawHexagonOnGrid(rowIndex, colIndex);   
                }
                if (rowIndex >= _hexagonMaxLengthCount / 2)
                    currentHexagonRowCount--;
                else
                    currentHexagonRowCount++;
            }
        }


        public int GetHexagonMaxCountByRow(int rowIndex)
        {
            if (rowIndex % 2 == 0)
            {
                return _hexagonMaxLengthCount;
            }
            else
            {
                return _hexagonMaxLengthCount - 1;
            }
        }



    }
}
