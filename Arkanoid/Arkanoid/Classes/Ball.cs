﻿using System;
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
    class Ball : Entity
    {
        Ellipse ball; 
        public Ball(int x, int y, int width, int height)
        {
            hitBox = new Rect(x,y,1,1);
            setupBall(height,width);

        }

        private void setupBall(int height, int width)
        {
            this.ball = new Ellipse();
            this.ball.Height = height;
            this.ball.Width = width;
            this.ball.Stroke = new SolidColorBrush(Colors.Black);
            this.ball.Fill = new SolidColorBrush(Colors.Yellow);
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
    }
}