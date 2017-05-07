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

namespace Gaze.Control
{
    public partial class GazeButton : Button
    {
        public static string BGHighlightColor = "#FFE59400";
        public static string BGOriginalColor = "#FF373737";

        public BrushConverter bc = new BrushConverter();

        public GazeButton()
        {
            var style = TryFindResource("NormalStyle") as Style;

            //this.Style = style;
            //this.Background = bc.ConvertFrom(BGOriginalColor) as Brush;
           
        }



        private void SetToHighlightStyle()
        {
            Style st = new Style();
        }
    }
}
