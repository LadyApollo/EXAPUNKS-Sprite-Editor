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
    /// Interaction logic for BorderAndFill.xaml
    /// </summary>
    public partial class BorderAndFill : UserControl
    {
        public PixelState PixelState { get; private set; }
        private Brush _originalBrush;

        private bool _lockInput;

        public BorderAndFill(int row, int column)
        {
            PixelState = new PixelState(row, column);
            InitializeComponent();
            _originalBrush = myButton.Background;
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            if (_lockInput) return;
            SetActivated(!PixelState.Activated);
        }

        public void SetLock(bool value)
        {
            _lockInput = value;
            //myButton.IsEnabled = value;
        }

        public void SetActivated(bool activated)
        {
            PixelState.Activated = activated;
            myButton.Background = PixelState.Activated ? Brushes.DimGray : _originalBrush;
        }        

        public void SetPixelState(PixelState newPixelState)
        {
            if (newPixelState.Row != PixelState.Row)
                throw new InvalidOperationException("Rows don't match");
            if (newPixelState.Column != PixelState.Column)
                throw new InvalidOperationException("Rows don't match");

            PixelState = newPixelState;
            SetActivated(PixelState.Activated);
        }
    }

    public class PixelState
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public bool Activated;
        
        public PixelState(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public PixelState(string data)
        {
            Activated = data[0] == '1';
            Column = Convert.ToInt32(data[1].ToString());
            Row = Convert.ToInt32(data[2].ToString());
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Convert.ToInt32(Activated));
            sb.Append(Column);
            sb.Append(Row);
            return sb.ToString();
        }
    }
}
