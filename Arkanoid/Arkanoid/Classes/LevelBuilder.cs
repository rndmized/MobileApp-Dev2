using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Arkanoid.Classes
{
    class LevelBuilder
    {


        public List<Brick> getNewLevelLayout(int rows, int columns, Canvas GameCanvas)
        {
            List<Brick> levelLayout = new List<Brick>();

            int height = (int)GameCanvas.Height / 20;
            int width = (int)GameCanvas.Width / columns;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = (int)width * j;
                    int y = (int)height * i;
                    Brick brick = new Brick(x, y, width, height);
                    Canvas.SetLeft(brick.getBrick(), x);
                    Canvas.SetTop(brick.getBrick(), y);
                    levelLayout.Add(brick);
                }
            }
            return levelLayout;
        }


        public List<Brick> getNewRandomLevelLayout(int rows, int columns, Canvas GameCanvas)
        {
            List<Brick> levelLayout = new List<Brick>();

            int height = (int)GameCanvas.Height / 20;
            int width = (int)GameCanvas.Width / columns;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = (int)width * j;
                    int y = (int)height * i;
                    Brick brick = new Brick(x, y, width, height);
                    Canvas.SetLeft(brick.getBrick(), x);
                    Canvas.SetTop(brick.getBrick(), y);
                    levelLayout.Add(brick);
                }
            }
            Random rnd = new Random();
            for (int i = 0; i < rnd.Next(10,25); i++)
            {
                levelLayout.RemoveAt(rnd.Next(0,levelLayout.Count));
            }
            return levelLayout;
        }
    }
}
