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
    /*
     *Brick is an entity subclass that represents a brick. It has toughness representing how many huts can it withstand before braking and a isBroken status.
     * It also has a Rectangle as a visual representation of the brick that can be returned to be rendered.
     * It also has a collision detection function that determines whether it has been collided or not.
     */
    class Brick : Entity
    {

        protected int toughness;
        protected bool isBroken = false;
        protected Rectangle brick;
        

        public Brick(int x, int y, int width, int height)
        {
            hitBox = new Rect(x, y, width, height);
            setupBrick(width, height);
            toughness = 1;

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
            toughness--;
            switch (toughness)
            {
                case 0:
                    isBroken = true;
                    break;
                case 1:
                    brick.Fill = new SolidColorBrush(Colors.GreenYellow);
                    break;
                case 2:
                    break;
            }

        }
            
    }
}
