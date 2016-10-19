using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private Canvas _canvas;
        private double _hexagonSideLength = 10.0;


        private int _oxigenCount;
        public int OxigenCount
        {
            get { return _oxigenCount; }
            set
            {
                _oxigenCount = value;
                Notify("OxigenCount");
            }
        }

        private int _hexagonSideCount = 10;
        public int HexagonSideCount
        {
            get { return _hexagonSideCount; }
            set
            {
                _hexagonSideCount = value;
                Notify("HexagonSideCount");
            }
        }

        private int _gravityGunsCount = 60;
        public int GravityGunsCount
        {
            get { return _gravityGunsCount; }
            set
            {
                _gravityGunsCount = value;
                Notify("GravityGunsCount");
            }
        }

        private int _oxigenReducerCount = 60;
        public int OxigenReducerCount
        {
            get { return _oxigenReducerCount; }
            set {
                _oxigenReducerCount = value;
                Notify("OxigenReducerCount");
            }
        }

        private int _oxigenGainerCount = 40;
        public int OxigenGainerCount
        {
            get { return _oxigenGainerCount; }
            set
            {
                _oxigenGainerCount = value;
                Notify("OxigenGainerCount");
            }
        }


        private double _oxigenReducerVariance = 0.8;
        public double OxigenReducerVariance
        {
            get { return _oxigenReducerVariance; }
            set
            {
                _oxigenReducerVariance = value;
                Notify("OxigenReducerVariance");
            }
        }

        private double _oxigenGainerVariance = 1.0;
        public double OxigenGainerVariance
        {
            get { return _oxigenGainerVariance; }
            set
            {
                _oxigenGainerVariance = value;
                Notify("OxigenGainerVariance");
            }
        }





        private int _hexagonMaxLengthCount { get { return 2 * _hexagonSideCount - 1; } }

        public MainWindow()
        {
            InitializeComponent();

            CreateCanvas();

            

            GenerateMap_Click(null, null);

        }

        public void CreateCanvas()
        {
            var canvasLength = 2 * _hexagonSideCount * _hexagonSideLength * Math.Sqrt(3.0);

            _canvas = new Canvas();
            _canvas.Width = canvasLength;
            _canvas.Height = canvasLength * (Math.Sqrt(3.0) / 2.0);
            //_canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            //_canvas.VerticalAlignment = VerticalAlignment.Stretch;
            //_canvas.Background = new SolidColorBrush(Colors.Yellow);
            _canvas.Margin = new Thickness(10.0);

            ViewBox.Child = _canvas;
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

        public void DrawHexagonAbsolute(double x, double y, HexagonData data)
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

            

            hexagon.DataContext = data;

            Canvas.SetLeft(hexagon, x);
            Canvas.SetTop(hexagon, y);

            _canvas.Children.Add(hexagon);
        }

        public double CalculateProbabilityFactor(double variance, double distanceToMainAxis)
        {
            return (1.0 / (variance * Math.Sqrt(2.0 * Math.PI))) * Math.Pow(Math.E, -0.5 * Math.Pow(distanceToMainAxis / variance, 2.0));
        }


        Dictionary<Polygon, double> OxigenReducers = new Dictionary<Polygon, double>();

        public void DistributeOxigenReducers()
        {
            OxigenReducers.Clear();

            var rnd = new Random();
            
            foreach (Polygon hexagon in _canvas.Children)
            {
                var hexagonData = hexagon.DataContext as HexagonData;
                if (! (hexagonData.RowIndex == Math.Floor(_hexagonMaxLengthCount / 2.0) && 
                    (hexagonData.ColIndex == 0 || (hexagonData.ColIndex == (_hexagonMaxLengthCount-1)))))
                {
                    var probability = CalculateProbabilityFactor(OxigenReducerVariance, hexagonData.DistanceToMainAxis);
                    var score = rnd.Next(1, 10) * (1.0 + probability);
                    OxigenReducers.Add(hexagon, score);
                }
            }

            OxigenReducers = OxigenReducers.OrderByDescending(pair => pair.Value).Take(_oxigenReducerCount).ToDictionary(x => x.Key, x => x.Value);

            foreach (var reducer in OxigenReducers)
            {
                var hexagon = reducer.Key;
                hexagon.Fill = Brushes.Red;
            }
        }

        Dictionary<Polygon, double> OxigenGainer = new Dictionary<Polygon, double>();

        public void DistributeOxigenGainers()
        {
            var rnd = new Random();
            var results = new Dictionary<Polygon, double>();
            foreach (Polygon hexagon in _canvas.Children)
            {
                var hexagonData = hexagon.DataContext as HexagonData;
                if (hexagonData.RowIndex != Math.Floor(_hexagonMaxLengthCount / 2.0) &&
                    !(hexagonData.ColIndex == 0 || hexagonData.ColIndex == _hexagonMaxLengthCount - 1) && !OxigenReducers.ContainsKey(hexagon))
                {
                    var probability = CalculateProbabilityFactor(OxigenGainerVariance, hexagonData.DistanceToMainAxis);
                    var score = rnd.Next(1, 10) * (1.0 - probability);
                    results.Add(hexagon, score);
                }
            }

            OxigenGainer = results.OrderByDescending(pair => pair.Value).Take(_oxigenGainerCount).ToDictionary(x => x.Key, x => x.Value);

            foreach (var result in OxigenGainer)
            {
                var hexagon = result.Key;
                hexagon.Fill = Brushes.Green;
                hexagon.MouseLeftButtonDown += OxigenGainer_MouseLeftButtonDown;
            }
        }


        Dictionary<Polygon, double> GravityGun = new Dictionary<Polygon, double>();

        public void DistributeGravityGuns()
        {
            var rnd = new Random();
            var results = new Dictionary<Polygon, double>();
            foreach (Polygon hexagon in _canvas.Children)
            {
                var hexagonData = hexagon.DataContext as HexagonData;
                if (hexagonData.RowIndex != Math.Floor(_hexagonMaxLengthCount / 2.0) &&
                    !(hexagonData.ColIndex == 0 || hexagonData.ColIndex == _hexagonMaxLengthCount - 1) && !OxigenReducers.ContainsKey(hexagon) && !OxigenGainer.ContainsKey(hexagon))
                {
                    //var probability = CalculateProbabilityFactor(10.0, hexagonData.DistanceToMainAxis);
                    var score = rnd.Next(1, 100);
                    results.Add(hexagon, score);
                }
            }

            GravityGun = results.OrderByDescending(pair => pair.Value).Take(GravityGunsCount).ToDictionary(x => x.Key, x => x.Value);

            foreach (var result in GravityGun)
            {
                var hexagon = result.Key;
                hexagon.Fill = Brushes.Yellow;
                hexagon.MouseLeftButtonDown += GravityGun_MouseLeftButtonDown;
            }
        }

        private void GravityGun_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var hexagon = sender as Polygon;
            if (hexagon.Fill == Brushes.Yellow)
                hexagon.Fill = Brushes.LightBlue;
            else
                hexagon.Fill = Brushes.Yellow;
        }

        private void OxigenGainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var hexagon = sender as Polygon;
            if (hexagon.Fill == Brushes.Green)
                hexagon.Fill = Brushes.LightBlue;
            else
                hexagon.Fill = Brushes.Green;
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

            var distanceToMainAxis = Math.Abs(Math.Floor(_hexagonMaxLengthCount / 2.0) - rowIndex);

            var hexagonData = new HexagonData(rowIndex, colIndex, distanceToMainAxis);

            DrawHexagonAbsolute(x - ((_hexagonSideLength * Math.Sqrt(3.0)) / 2.0), y - _hexagonSideLength, hexagonData);
        }
        

        public void DrawGrid()
        {
            //_hexagonSideLength = _canvas.Width / (_hexagonMaxLengthCount * HexagonSideCount * Math.Sqrt(3));
            _hexagonSideLength = (_hexagonMaxLengthCount / HexagonSideCount) * 10.0;

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
            _canvas.Children.Clear();
        }

        private void GenerateMap_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            DrawGrid();
            DistributeOxigenReducers();
            DistributeOxigenGainers();
            DistributeGravityGuns();
            OxigenCount = HexagonSideCount;
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Close Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string argument)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(argument));
            }
        }

        private void DeceaseOxigenButton_Click(object sender, RoutedEventArgs e)
        {
            OxigenCount--;
        }

        private void IncreaseOxigenButton_Click(object sender, RoutedEventArgs e)
        {
            OxigenCount++;
        }
    }
}
