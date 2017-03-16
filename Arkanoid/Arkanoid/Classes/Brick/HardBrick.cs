using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Arkanoid.Classes
{
    class HardBrick : Brick
    {

        public HardBrick(int x, int y, int width, int height) : base(x, y, width, height)
        {
            hitBox = new Rect(x, y, width, height);
            setupBrick(width, height);
            thoughness = 2;
        }

        private void setupBrick(int width, int height)
        {
            brick = new Rectangle();
            brick.Stroke = new SolidColorBrush(Colors.Blue);
            brick.Fill = new SolidColorBrush(Colors.DarkMagenta);
            brick.Height = height;
            brick.Width = width;
            brick.SetValue(Rectangle.NameProperty, this.getX().ToString() + "_" + this.getY().ToString());
        }


    }
}
