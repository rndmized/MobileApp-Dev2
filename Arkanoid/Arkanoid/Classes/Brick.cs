using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Arkanoid.Classes
{
    class Brick : Entity
    {

        private int thoughness;
        private bool isBroken = false;
        private Rectangle rectangle;
        

        public Brick(int x, int y, int width, int height)
        {
            hitBox = new Rect(x, y, width, height);

        }
        public void setRectangle(Rectangle rectangle)
        {
            this.rectangle = rectangle;
        }

        public Rectangle getBrick()
        {
            return rectangle;
        }

        public bool isBrickBroken()
        {
            return this.isBroken;
        }

        public Boolean collides(Rect rect)
        {
            Rect check = hitBox;
            check.Intersect(rect);
            return (isBroken) ? false : check.IsEmpty;
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
    }
}
