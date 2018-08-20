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

namespace Exapunks_Sprite_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<BorderAndFill> _editorPixels;
        private List<List<PixelState>> _frames;
        private int _currentFrame = 0;

        private Version _version = new Version(0, 1, 1);

        public MainWindow()
        {
            InitializeComponent();
            _frames = new List<List<PixelState>>();
            GenerateFirstFrame();

            Title += " " + _version.ToString();
        }

        private void GenerateFirstFrame()
        {
            _editorPixels = new List<BorderAndFill>();
            var firstFrame = new List<PixelState>();

            var rowCount = myGrid.RowDefinitions.Count;
            var columnCount = myGrid.ColumnDefinitions.Count;
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < columnCount; c++)
                {
                    var bnf = new BorderAndFill(r, c);
                    firstFrame.Add(bnf.PixelState);
                    Grid.SetRow(bnf, r);
                    Grid.SetColumn(bnf, c);
                    myGrid.Children.Add(bnf);
                    _editorPixels.Add(bnf);
                }
            }
            _frames.Add(firstFrame);
        }

        private void NextFrame(object sender, RoutedEventArgs e)
        {
            _currentFrame++;
            if (_currentFrame + 1 > _frames.Count)
                CreateNewFrame();
            else
                ReadFrame(_currentFrame);
        }

        private void PreviousFrame(object sender, RoutedEventArgs e)
        {
            if (_currentFrame == 0) return;

            _currentFrame--;
            ReadFrame(_currentFrame);
        }

        private void ImportFrames(object sender, RoutedEventArgs e)
        {
            _frames.Clear();

            //almost regex
            var text = DataString.Text.Replace("\r\n", " ").Replace("DATA ", "").Split(' ').ToList();
            text.RemoveAt(text.Count - 1);

            var numPages = text.Count / 100;
            for (int p = 0; p < numPages; p++)
            {
                var newFrame = new List<PixelState>();

                var ii = p * 100;
                var fi = p * 100 + 100;
                for (int i = ii; i < fi; i++)
                    newFrame.Add(new PixelState(text[i]));

                _frames.Add(newFrame);
            }

            _currentFrame = 0;
            ReadFrame(0);
        }

        private void ExportFrames(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            var space = " ";
            var lineEnd = Environment.NewLine;

            foreach (var frame in _frames)
            {
                for (int l = 0; l < 100; l += 5)
                {
                    //Sometimes stupid code is smarter code
                    sb.Append("DATA ");
                    sb.Append(frame[l + 0].ToString());
                    sb.Append(space);
                    sb.Append(frame[l + 1].ToString());
                    sb.Append(space);
                    sb.Append(frame[l + 2].ToString());
                    sb.Append(space);
                    sb.Append(frame[l + 3].ToString());
                    sb.Append(space);
                    sb.Append(frame[l + 4].ToString());
                    sb.Append("\r\n");
                }
            }

            DataString.Text = sb.ToString();
        }

        private void DeleteFrame(object sender, RoutedEventArgs e)
        {
            _frames.RemoveAt(_currentFrame);
            if (--_currentFrame < 0) _currentFrame = 0;

            if (_frames.Count == 0)
                CreateNewFrame();
            else
                ReadFrame(_currentFrame);
        }

        private void CreateNewFrame()
        {
            var newFrame = new List<PixelState>();
            foreach (var editorPixel in _editorPixels)
            {
                var nps = new PixelState(editorPixel.PixelState.Row, editorPixel.PixelState.Column);
                newFrame.Add(nps);
                editorPixel.SetPixelState(nps);
            }
            _frames.Add(newFrame);

            UpdateFrameText();
        }

        private void ReadFrame(int currentFrame)
        {
            var pixelsCount = _editorPixels.Count;
            var frame = _frames[currentFrame];
            for (int i = 0; i < pixelsCount; i++)
            {
                _editorPixels[i].SetPixelState(frame[i]);
            }

            UpdateFrameText();
        }

        private void UpdateFrameText()
        {
            FrameText.Content = $"Frame: {_currentFrame + 1}/{_frames.Count}";
        }
    }
}
