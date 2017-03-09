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

        protected int thoughness;
        protected bool isBroken = false;
        protected Rectangle brick;
        

        public Brick(int x, int y, int width, int height)
        {
            hitBox = new Rect(x, y, width, height);
            setupBrick(width, height);
            thoughness = 1;

        }

        private void setupBrick(int width, int height)
        {
            brick = new Rectangle();
            brick.Stroke = new SolidColorBrush(Colors.Green);
            brick.Fill = new SolidColorBrush(Colors.GreenYellow);
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
                    break;
                case 2:
                    break;
            }

        }

        public Ball impactEffect(Ball ball) {
            return ball;
            //DO nothing for regular bricks.
        }
            
    }
}
