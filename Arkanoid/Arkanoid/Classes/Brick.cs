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
    class Brick : Entity
    {

        private int thoughness;
        private bool isBroken = false;
        private Rectangle brick;
        

        public Brick(int x, int y, int width, int height)
        {
            hitBox = new Rect(x, y, width, height);
            setupBrick(width, height);
            thoughness = 1;

        }

        private void setupBrick(int width, int height)
        {
            brick = new Rectangle();
            brick.Stroke = new SolidColorBrush(Colors.Black);
            brick.Fill = new SolidColorBrush(Colors.Magenta);
            brick.Height = height;
            brick.Width = width;
            brick.SetValue(Rectangle.NameProperty, this.getX().ToString() + "_" + this.getY().ToString());
        }

        public void setBrick(Rectangle newBrick)
        {
            this.brick = newBrick;
        }

        public Rectangle getBrick()
        {
            return brick;
        }

        public bool isBrickBroken()
        {
            return this.isBroken;
        }

        public Boolean collides(Rect rect)
        {
            Rect check = hitBox;
            check.Intersect(rect);
            return (isBroken) ? false : !(check.IsEmpty);
        }

        public void Break()
        {
            thoughness--;
            switch (thoughness)
            {
                case 0:
                    isBroken = true;
                    break;
                case 1:
                    brick.Fill = new SolidColorBrush(Colors.Red);
                    break;
                case 2:
                    break;
            }

        }
    }
}
