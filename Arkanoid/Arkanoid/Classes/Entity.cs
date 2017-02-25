using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Arkanoid.Classes
{
    class Entity
    {
        protected Rect hitBox;

        public void setY(int y)
        {
            hitBox.Y = y;
        }
        public void setX(int x)
        {
            hitBox.X = x;
        }

        public int getY()
        {
            return (int)hitBox.Y;
        }
        public int getX()
        {
            return (int)hitBox.X;
        }

        public int getWith()
        {
            return (int)this.hitBox.Width;
        }
        public int getHeight()
        {
            return (int)this.hitBox.Height;
        }
    }
}
