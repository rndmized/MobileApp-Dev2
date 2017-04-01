using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Classes
{


    /*
     Score Controller Class Is to add and reset score and to return score value.
         */
    public class ScoreController
    {
        private volatile int score = 0;

        public int getScore()
        {
            return score;
        }

        public void addScore(int points)
        {
            this.score += points;
        }

        public void resetScore()
        {
            this.score = 0;
        }
    }
}
