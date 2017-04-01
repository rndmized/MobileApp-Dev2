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
   * Ball is an entity subclass that represents a ball.
   * It has an Ellipse as a visual representation of the ball that can be returned to be rendered, a speed that determines how many pixels does it moves per tick and xyvectors to determine direction.
   * Speed can be modified by increasing it by one or decreasing it by one.
   * It also has a collision detection function that determines whether it has been collided or not.
   */
    class Ball : Entity
    {
        private Ellipse ball;
        private float speed = 5;
        private float yVector = -1;
        private float xVector = -1;

        public Ball(int x, int y, int width, int height)
        {
            hitBox = new Rect(x,y,height,width);
            setupBall(height,width);

        }

        private void setupBall(int height, int width)
        {
            this.ball = new Ellipse();
            this.ball.Name = "ball";
            this.ball.Height = height;
            this.ball.Width = width;
            this.ball.Stroke = new SolidColorBrush(Colors.Black);
            this.ball.Fill = new SolidColorBrush(Colors.LimeGreen);
        }

        public Rect getHitBox()
        {
            return hitBox;
        }

        public Ellipse getBall()
        {
            return this.ball;
        }

        public void setBall(Ellipse ellipse)
        {
            this.ball = ellipse;
        }

        public float getSpeed() { return this.speed; }
        public void speedUp() { this.speed++; }
        public void speedDown() { if (speed > 0) { this.speed--; } }

        public float getXVector() { return xVector; }
        public void setXVector(float newVec) { xVector = newVec; }

        public float getYVector() { return yVector; }
        public void setYVector(float newVec) { yVector = newVec; }


    }
}
