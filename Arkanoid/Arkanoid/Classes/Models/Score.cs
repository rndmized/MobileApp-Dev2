using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkanoid.Classes.Models
{
    public class Score
    {
        public Score() { }
        public Score(int user_id, int score)
        {
            this.score = score;
            this.id = user_id;

        }
        public int id { get; set; }
        public int score { get; set; }

    }
}
