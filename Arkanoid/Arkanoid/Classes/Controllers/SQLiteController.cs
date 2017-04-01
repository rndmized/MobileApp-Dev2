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
    /*
     Interfaces between Application and Database
         */
    public class SQLiteController
    {
        private String path;
        private SQLite.Net.SQLiteConnection conn;

        public SQLiteController()
        {
            //Create db and tables if they do not exist
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<User>();
            conn.CreateTable<Score>();
        }

        //Return a list of users from db.
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

        //Returns list of Scores from a given user id
        public List<Score> getScoresByUser(int user_id)
        {
            var scores_query = conn.Query<Score>("Select * from score where id = ? ORDER BY score DESC LIMIT 5", user_id);
            List<Score> scores = new List<Score>();
            foreach (var score in scores_query)
            {
                scores.Add(score);
            }
            return scores;
        }

        //Adds user to db
        public void addUser(String username)
        {
            var query = conn.Insert(new User(username, 0));
            conn.Commit();
        }


        //Get maximum score of a given user id
        public int getUserMaxScore(int user_id)
        {
            var score_query = conn.Query<Score>("Select * from score where id = ? ORDER BY score DESC LIMIT 1;", user_id);
            if (score_query.Count == 0)
            {
                return 0;
            }
            return score_query.First<Score>().score;
        }

        //Saves score to database, if there are more than 10 score fields overwrite the lowest value.
        public void saveScore(User user, int score)
        {

            List<Score> count = conn.Query<Score>("Select count(id) from score where id =  ?",  user.id).ToList<Score>();
            if (count.Count < 10)
            {
                Score new_score = new Score(user.id, score);
                var score_query = conn.Insert(new_score);
                conn.Commit();
            }
            else
            {

                var scores = conn.Query<Score>("Select * from score where id = ? ORDER BY score ASC LIMIT 10", user.id);
                foreach (var score_item in scores)
                {
                    if (score > score_item.score)
                    {
                        conn.Query<Score>("Update score set score = ? where id = ? and score = ?",score,user.id,score_item.score);
                        conn.Commit();
                        break;
                    }
                }
            }

        }

    }
}
