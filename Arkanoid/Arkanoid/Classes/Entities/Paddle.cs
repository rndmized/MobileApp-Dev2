using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Arkanoid.Classes
{
    /*
    *Paddle is an entity subclass that represents a paddle.
    * It has a Rectangle as a visual representation of the paddle that can be returned to be rendered.
    * It also has a collision detection function that determines whether it has been collided or not.
    */
    class Paddle : Entity
    {
        private Rectangle paddle;

        public Paddle(int x, int y, int width, int height)
        {
            hitBox = new Rect(x, y, width, height);
            setupPaddle(width, height);
        }

        private void setupPaddle(int width, int height)
        {
            paddle = new Rectangle();
            paddle.Height = height;
            paddle.Width = width;
            paddle.Stroke = new SolidColorBrush(Colors.Black);
            paddle.Fill = new SolidColorBrush(Colors.WhiteSmoke);
        }

        public Rectangle getPaddle()
        {
            return this.paddle;
        }

        public void setPaddle(Rectangle newPaddle)
        {
            this.paddle = newPaddle;
        }

        public Boolean collides(Rect rect)
        {
            Rect check = hitBox;
            check.Intersect(rect);
            return false ? false : !(check.IsEmpty);
        }

    }
}
