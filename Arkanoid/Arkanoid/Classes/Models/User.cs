using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Classes.Models
{
    public class User
    {

        public User() {  }

        public User(String username, int id)
        {
            this.id = id;
            this.username = username;
        }

        [Column("id")]
        [PrimaryKey]
        [NotNull]
        [AutoIncrement]
        public int id { get; set; }
        [NotNull]
        public String username { get; set; }

    }
}
