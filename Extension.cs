using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace DataBase
{
    public static class Extension
    {
	public static void Hello()
	{ //add
	    Console.WriteLine("Hello");
	}

        public static T ToObject<T>(this SqlDataReader dr) where T : class, new()
        {
            dr.Read();
            T Result = new T();

            PropertyInfo[] Props = typeof(T).GetProperties();

            for (int i = 0; i < dr.FieldCount; ++i)
            {
                string Column = dr.GetName(i);
                PropertyInfo prop = Props.Where(tp => tp.Name.Equals(Column)).FirstOrDefault();
                prop.SetValue(Result, dr[Column]);
            }

            dr.Close();
            return Result;
        }

        public static T Select<T>(this SqlConnection con, string Restraints, params object[] args) where T : class, new()
        {
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            using (SqlCommand cmd = new SqlCommand($"SELECT TOP 1 * FROM {typeof(T).Name} WHERE {string.Format(Restraints, args)}", con))
            {
                var dr = cmd.ExecuteReader();
                return dr.ToObject<T>();
            }
        }

        public static List<T> Selects<T>(this SqlConnection con, string Restraints, params object[] args) where T : class, new()
        {
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            List<T> list = new List<T>();

            using (SqlCommand cmd = new SqlCommand($"SELECT * FROM {typeof(T).Name} WHERE {string.Format(Restraints, args)}", con))
            {
                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    T temp = new T();

                    foreach (PropertyInfo prop in typeof(T).GetProperties())
                    {
                        if (!object.Equals(dr[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(temp, dr[prop.Name]);
                        }
                    }
                    list.Add(temp);
                }

                dr.Close();

                return list;
            }
        }
           
        public static void Insert<T>(this SqlConnection con, T obj) where T : class, new()
        {
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            List<string> names = new List<string>();
            List<string> values = new List<string>();

            foreach (var i in typeof(T).GetProperties())
            {
                names.Add(i.Name);
            }

            foreach (var i in typeof(T).GetProperties())
            {
                if (i.GetValue(obj) == null)
                {
                    if (i.PropertyType.Name.Equals("String"))
                    {
                        values.Add(@"''");
                    }
                    else
                    {
                        values.Add("DEFAULT");
                    }
                    continue;
                }

                if (i.PropertyType.FullName.Contains("System.DateTime"))
                {
                    values.Add($"\'{(DateTime)i.GetValue(obj):yyyy-MM-dd HH:mm:ss}\'");
                }
                else
                {
                    values.Add($"\'{i.GetValue(obj).ToString()}\'");
                }
            }
            string sqlCmd = $"INSERT INTO {typeof(T).Name}({string.Join(", ", names)}) VALUES ({string.Join(", ", values)})";

            SqlCommand cmd = new SqlCommand(sqlCmd, con);
            cmd.ExecuteNonQuery();

            cmd.Dispose();

            names.Clear();
            values.Clear();
        }

        public static void Update<T>(this SqlConnection con, T obj, string Restraints, params object[] args) where T : class, new()
        {
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();

            List<string> names = new List<string>();

            foreach (var i in typeof(T).GetProperties())
            {
                if (i.GetValue(obj) == null)
                {
                    if (i.PropertyType.Name.Equals("String"))
                    {
                        names.Add($"{i.Name} = \'\'");
                    }
                    else
                    {
                        names.Add($"{i.Name} = DEFAULT");
                    }
                    continue;
                }

                if (i.PropertyType.FullName.Contains("System.DateTime"))
                {
                    names.Add($"{i.Name} = \'{(DateTime)i.GetValue(obj):yyyy-MM-dd HH:mm:ss}\'");
                }
                else
                {
                    names.Add($"{i.Name} = \'{i.GetValue(obj).ToString()}\'");
                }
            }

            string sqlCmd = $"UPDATE {typeof(T).Name} SET {string.Join(", ", names)} WHERE {string.Format(Restraints, args)}";

            SqlCommand cmd = new SqlCommand(sqlCmd, con);
            cmd.ExecuteNonQuery();

            cmd.Dispose();

            names.Clear();
        }
    }
}
