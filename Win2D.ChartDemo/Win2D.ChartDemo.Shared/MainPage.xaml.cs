using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Foundation;

namespace Win2D.ChartDemo
{
    public sealed partial class MainPage : Page
    {
        private List<double> _value = new List<double>();
        private List<string> _text = new List<string>();
        private List<Color> _color = new List<Color>();
        private Random rnd = new Random();

        public MainPage()
        {
            InitializeComponent();
        }

        private void CanvasControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            var session = args.DrawingSession;
            var center = 280;
            var R = 200;

            var startXText = 550;
            var startYText = 200;
            
            var vec = new Microsoft.Graphics.Canvas.Numerics.Vector2 { X = center, Y = center };

            var startAngle = new List<float>();
            var sweepAngle = new List<float>();

            foreach (var value in _value)
            {
                startAngle.Add(sweepAngle.Sum());
                sweepAngle.Add((float)value * (float)((2 * Math.PI) / 100));
            }

            for (var i = 0; i < startAngle.Count(); i++)
            {
                using (var pathAngle = new CanvasPathBuilder(session.Device))
                {
                    pathAngle.BeginFigure(center, center);
                    pathAngle.AddArc(vec, R, R, startAngle[i], sweepAngle[i]);
                    pathAngle.EndFigure(CanvasFigureLoop.Closed);

                    session.FillGeometry(CanvasGeometry.CreatePath(pathAngle), _color[i]);
                }

                // история
                var step = i * 24;
                session.DrawText(_text[i], new Vector2(startXText, startYText + step), Color.FromArgb(255, 0, 0, 0));
                session.FillRectangle(new Rect(new Point(startXText - 25, startYText + 10 + step), new Point(startXText - 5, startYText + 20 + step)), _color[i]);

                // подписи
                var dx = R * 1.1 * Math.Cos(startAngle[i] + sweepAngle[i] / 2);
                var dy = R * 1.1 * Math.Sin(startAngle[i] + sweepAngle[i] / 2);

                session.DrawText(String.Format("{0}%", _value[i]), new Vector2((float)(dx + center - 18), (float)(dy + center - 18)), Color.FromArgb(255, 0, 0, 0));
            }
        }

        private Color RundomColor()
        {
            return Color.FromArgb(255, (byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
        }

        private void bAdd_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbValue.Text))
            {
                return;
            }

            var setValue = Convert.ToDouble(tbValue.Text);
            if (_value.Sum() + setValue > 100 || setValue > 100)
            {
                return;
            }

            _value.Add(setValue);
            _text.Add(tbText.Text);
            _color.Add(RundomColor());

            var value = new List<string>();
            for (var i = 0; i < _value.Count(); i++)
            {
                value.Add(String.Format("{0}% ({1})", _value[i], _text[i]));
            }

            lbValue.ItemsSource = null;
            lbValue.ItemsSource = value;

            ccChart.Invalidate();

            tbText.Text = tbValue.Text = "";
        }
    }
}