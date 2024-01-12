using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.IO.Pipes;


namespace tgbot
{
    public class SQL
    {
        public static string CONNECTION_STRING = @"Data Source =" + Environment.CurrentDirectory + @"\proged.sqlite; Version=3;";


        public static void ChangeLanguage(string id_user, int id_language)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"UPDATE Users SET id_language = {id_language} WHERE id_user like {id_user}";
                command.ExecuteNonQuery();
                connection.Close();


            }
        }

        public static int languageUser(long id_user)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"SELECT id_language FROM Users WHERE id_user = {id_user}";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // Проверяем наличие строк для чтения
                    {
                        int id = Convert.ToInt32(reader["id_language"]);
                        return id;
                    }
                }
                return 1;

            }
        }
        public static int TSUser(long id_user)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"SELECT use_techsup FROM Users WHERE id_user = {id_user}";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // Проверяем наличие строк для чтения
                    {
                        int id = Convert.ToInt32(reader["use_techsup"]);
                        return id;
                    }
                }
                return 1;

            }
        }
        public static void ChangeTS(long id_user, int use_techsup)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"UPDATE Users SET use_techsup = {use_techsup} WHERE id_user like {id_user}";
                command.ExecuteNonQuery();
                connection.Close();


            }
        }

        public static string TechSup(string str, long user_id, int language)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"insert into TechnicalSupport(id_user,appel) values ({user_id}, @str)";
                command.Parameters.AddWithValue("@str", str);
                command.ExecuteNonQuery();
                connection.Close();

                if (language == 1)
                {
                    return "Сообщение успешно отправлено";
                }
                if (language == 2)
                {
                    return "The message has been sent successfully";
                }
                return "1";

            }
        }

        public static bool IsUserExists(long id_user)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"Select * from Users where id_user = {id_user}";

                return command.ExecuteScalar() == null;

            }
        }
        public static int GetCountExercise()
        {
            List<int> count = new List<int>();
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"SELECT id_exercise from Exercise";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id_exercise"]);
                        count.Add(id);
                    }
                }
                connection.Close();
            }

            return count.Count;
        }

        public static void AddUser(long id_user, string nickname)
        {
            if (IsUserExists(id_user))
            {
                using (var connection = new SQLiteConnection(CONNECTION_STRING))
                {
                    connection.Open();
                    var command = new SQLiteCommand();
                    command.Connection = connection;
                    command.CommandText = $"insert into Users(id_user, nickname, id_language, use_techsup) values ({id_user},'{nickname}',1,0)";
                    command.ExecuteNonQuery();
                }
                using (var connection = new SQLiteConnection(CONNECTION_STRING))
                {
                    connection.Open();
                    int count = GetCountExercise();

                    for (int i = 0; i < count; i++)
                    {
                        using (var command = new SQLiteCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = $"insert into Progress_status (id_user, id_exercise, status) values ({id_user},{i + 1},'NotCompleted')";
                            command.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }
            }
        }


        public static string ShowTheory(int id_theory)
        {
            string result = ""; // Инициализируем переменную заранее


            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"SELECT theory FROM Theory WHERE id_Theory = {id_theory}";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader["theory"].ToString();
                    }
                }
                connection.Close();
            }

            return result; // Возвращаем значение result после завершения работы с базой данных
        }

        public static List<(string, string, int)> ShowTask(int id_exercise)
        {
            List<(string, string, int)> result = new List<(string, string, int)>(); // Инициализируем переменную заранее


            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"SELECT exercise, answer, id_exercise FROM Exercise WHERE id_exercise = {id_exercise}";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ex = reader["exercise"].ToString();
                        string ans = reader["answer"].ToString();
                        result.Add((ex, ans, id_exercise));
                    }
                }
                connection.Close();
            }

            return result; // Возвращаем значение result после завершения работы с базой данных
        }

        public static List<(string, string, int)> GetCourses(int id_language, int id_proglanguage)
        {
            List<(string, string, int)> courses = new List<(string, string, int)>();


            using (SQLiteConnection connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();

                string query = $"SELECT name, difficulty, id_course FROM Courses where id_language = {id_language} and id_proglanguage = {id_proglanguage}"; // Выбираем оба столбца

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["name"].ToString();
                            string difficulty = reader["difficulty"].ToString();
                            int id = Convert.ToInt32(reader["id_course"]);
                            courses.Add((name, difficulty, id));
                        }
                    }
                }
                connection.Close();
            }

            return courses;
        }
        public static List<(string, string, int)> GetExercises(int id_course)
        {
            List<(string, string, int)> exercises = new List<(string, string, int)>();

            using (SQLiteConnection connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();

                string query = $"SELECT exercise, answer, id_exercise FROM Exercise where id_course = {id_course}"; // Выбираем оба столбца

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string exercise = reader["exercise"].ToString();
                            string answer = reader["answer"].ToString();
                            int id = Convert.ToInt32(reader["id_exercise"]);
                            exercises.Add((exercise, answer, id));
                        }
                    }
                }
                connection.Close();
            }

            return exercises;

        }

        public static void ChangeStatus(long id_user, int id_exercise)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                List<(string, string, int)> task = ShowTask(id_exercise);
                List<int> lst = SearchExercise(task[0].Item2);
                int count = lst.Count;
                for (int i = 0; i < count; i++)
                {
                    using (var command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = $"Update Progress_status set status = 'Completed' where id_user = {id_user} and id_exercise = {lst[i]}";
                        command.ExecuteNonQuery();
                    }
                }

            }
        }

        public static List<int> SearchExercise(string Answer)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                List<int> ids = new List<int>();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"select id_exercise from Exercise where answer = @Answer";
                command.Parameters.AddWithValue("@Answer", Answer);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id_exercise"]);
                        ids.Add(id);
                    }
                }
                connection.Close();
                return ids;
            }
        }
        public static string CheckStatus(long id_user, int id_exercise)
        {
            string status = "";
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"select status from Progress_status where id_user = {id_user} and id_exercise = {id_exercise}";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        status = reader["status"].ToString();
                    }
                }
                connection.Close();
            }
            if (status == "NotCompleted")
            {
                return "✗";
            }
            if (status == "Completed")
            {
                return "✓";
            }
            return "1";
        }


        public static List<(string, int, int)> StatusProcent(List<(int, int, List<int>, List<int>)> lst)
        {
            List<(string, int ,int)> finalyLst = new List<(string, int ,int)>();
            int count = lst.Count;
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                for (int i = 0; i < count; i++)
                {
                    List<int> ids_exercise = new List<int>();
                    using (var command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = $"select name from Courses where id_course = {lst[i].Item1}";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = reader["name"].ToString();
                                float test = ((float)lst[i].Item4.Count / (float)lst[i].Item3.Count) * 100;
                                int id = Convert.ToInt32(test);
                                finalyLst.Add((name, lst[i].Item2,id));
                            }
                        }
                    }
                   
                }
                connection.Close();
                return finalyLst;
            }
        }
        public static List<(int,int, List<int>, List<int>)> GetProgressStatus(long id_user, int id_language)
        {
            List<(int, int, List<int>, List<int>)> lst = new List<(int,int, List<int>, List<int>)>();
            List<(int,int)> ids_courses = new List<(int,int)>();
            List<(int, List<int>)> lst_exercise = new List<(int, List<int>)>();

            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var command = new SQLiteCommand();
                command.Connection = connection;
                command.CommandText = $"select id_course, id_proglanguage from Courses where id_language = {id_language}";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["id_course"]);
                        int id_pr = Convert.ToInt32(reader["id_proglanguage"]);
                        ids_courses.Add((id, id_pr));
                    }
                }
                connection.Close();
            }
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                int count = ids_courses.Count();
                for (int i = 0; i < count; i++)
                {
                    List<int> ids_exercise = new List<int>();
                    using (var command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = $"select id_exercise from Exercise where id_course = {ids_courses[i].Item1}";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["id_exercise"]);
                                ids_exercise.Add(id);
                            }
                        }
                    }
                    List<int> Completed = GetCompleted(id_user, ids_exercise);
                    lst.Add((ids_courses[i].Item1, ids_courses[i].Item2,ids_exercise,Completed));

                }
                connection.Close();
            }
            return lst;
        }

        public static List<int> GetCompleted(long id_user, List<int> ids_exercise)
        {
            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                connection.Open();
                List<int> lst = new List<int>();
                int count = ids_exercise.Count();
                for (int i = 0; i < count; i++)
                {
                    List<int> ids_complexercise = new List<int>();
                    using (var command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = $"select status from Progress_status where id_exercise = {ids_exercise[i]} and id_user = {id_user}";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string status = reader["status"].ToString();
                                if (status == "Completed")
                                {
                                    lst.Add(ids_exercise[i]);
                                }

                            }
                        }

                    }
                }

                connection.Close();
                return lst;
            }

        }
    }
}