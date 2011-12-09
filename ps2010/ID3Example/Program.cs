using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ID3;
using System.Data;

namespace ID3Example
{
    class Program
    {

        private static DataTable getDataTable()
        {
            DataTable result = new DataTable("samples");
            DataColumn column = result.Columns.Add("outlook");
            column.DataType = typeof(string);

            column = result.Columns.Add("temperature");
            column.DataType = typeof(string);

            column = result.Columns.Add("humidity");
            column.DataType = typeof(string);

            column = result.Columns.Add("windy");
            column.DataType = typeof(string);

            column = result.Columns.Add("result");
            column.DataType = typeof(string);

            result.Rows.Add(new string[] { "sun", "hight", "hight", "false", "don't play" });
            result.Rows.Add(new string[] { "sun", "hight", "hight", "true", "don't play" });
            result.Rows.Add(new string[] { "cloudy", "hight", "hight", "false", "play" });
            result.Rows.Add(new string[] { "rain", "hight", "hight", "false", "play" });
            result.Rows.Add(new string[] { "rain", "low", "normal", "false", "play" });
            result.Rows.Add(new string[] { "rain", "low", "normal", "true", "don't play" });
            result.Rows.Add(new string[] { "cloudy", "low", "normal", "true", "play" });
            result.Rows.Add(new string[] { "sun", "gentle", "hight", "false", "don't play" });
            result.Rows.Add(new string[] { "sun", "low", "normal", "false", "play" });
            result.Rows.Add(new string[] { "rain", "gentle", "normal", "false", "play" });
            result.Rows.Add(new string[] { "sun", "gentle", "normal", "false", "play" });
            result.Rows.Add(new string[] { "cloudy", "gentle", "hight", "true", "play" });
            result.Rows.Add(new string[] { "cloudy", "hight", "normal", "false", "play" });
            result.Rows.Add(new string[] { "rain", "gentle", "hight", "true", "don't play" });

            return result;
        }

        static void Main(string[] args)
        {
            ID3.Attribute outlook = new ID3.Attribute("outlook", new List<string>(new string[] { "sun", "cloudy", "rain" }));
            ID3.Attribute temperature = new ID3.Attribute("temperature", new List<string>(new string[] { "hight", "low", "gentle" }));
            ID3.Attribute humidity = new ID3.Attribute("humidity", new List<string>(new string[] { "hight", "normal" }));
            ID3.Attribute windy = new ID3.Attribute("windy", new List<string>(new string[] { "true", "false" }));

            List<ID3.Attribute> attributes = new List<ID3.Attribute>(new ID3.Attribute[] { outlook, temperature, humidity, windy });

            DataTable samples = getDataTable();
            ID3Tree id3 = new ID3Tree();
            TreeNode root = id3.BuildID3Tree(samples, "result", attributes);

            Console.ReadKey();
        }
    }
}
