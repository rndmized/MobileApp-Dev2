using Arkanoid.Classes.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Classes
{
    public class SQLiteController
    {
        private String path;
        private SQLite.Net.SQLiteConnection conn;

        public SQLiteController()
        {
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            Debug.WriteLine(Windows.Storage.ApplicationData.Current.LocalFolder.Path.ToString());
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<User>();
            conn.CreateTable<Score>();
        }

        public List<User> getUsers()
        {
            var user_query = conn.Table<User>();
            List<User> users = new List<User>();
            foreach (var user in user_query)
            {
                users.Add(user);
            }
            return users;
        }

        public void addUser(String username)
        {
            var query = conn.Insert(new User(username, 0));
            conn.Commit();
        }

        public int getUserMaxScore(int user_id)
        {
            var score_query = conn.Query<Score>("Select * from score where id = ? ORDER BY score DESC LIMIT 1;", user_id);
            if (score_query.Count == 0)
            {
                return 0;
            }
            return score_query.First<Score>().score;
        }

        public void saveScore(User user, int score)
        {

            List<Score> count = conn.Query<Score>("Select count(id) from score where id =  ?",  user.id).ToList<Score>();
            if (count.Capacity < 10)
            {
                Score new_score = new Score(user.id, score);
                var score_query = conn.Insert(new_score);
                conn.Commit();
            }
            else
            {
                //var scores = conn.FindWithQuery<Score>("Select * from score where id = " + user.id + ", ORDER BY score DESC, LIMIT 10;");
                var scores = conn.Query<Score>("Select * from score where id = " + user.id + " ORDER BY score DESC LIMIT 10;");
                foreach (var score_item in scores)
                {
                    if (score > score_item.score)
                    {
                        conn.Execute("Update score set score = " + score + "where id = " + user.id + "and score = " + score_item.score);
                        conn.Commit();
                        break;
                    }
                }
            }

        }

    }
}
