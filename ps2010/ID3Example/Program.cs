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
           /* DataColumn column = result.Columns.Add("outlook");
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
            */

            DataColumn column = result.Columns.Add("age");
            column.DataType = typeof(string);

            column = result.Columns.Add("competition");
            column.DataType = typeof(string);

            column = result.Columns.Add("type");
            column.DataType = typeof(string);

            column = result.Columns.Add("result");
            column.DataType = typeof(string);


 /*   old	| yes	      | swr	| down
	--------+-------------+---------+--------
	old	| no	      | swr 	| down
	--------+-------------+---------+--------
	old	| no	      | hwr	| down
	--------+-------------+---------+--------
	mid	| yes	      | swr	| down
	--------+-------------+---------+--------
	mid	| yes	      | hwr	| down
	--------+-------------+---------+--------
	mid	| no	      | hwr	| up
	--------+-------------+---------+--------
	mid	| no	      | swr	| up
	--------+-------------+---------+--------
	new	| yes	      | swr	| up
	--------+-------------+---------+--------
	new	| no	      | hwr	| up
	--------+-------------+---------+--------
	new	| no	      | swr	| up
            */

            result.Rows.Add(new string[] { "old", "yes", "swr", "down" });
            result.Rows.Add(new string[] { "old", "no", "swr", "down" });
            result.Rows.Add(new string[] { "old", "no", "hwr", "down" });
            result.Rows.Add(new string[] { "mid", "yes", "swr", "down" });
            result.Rows.Add(new string[] { "mid", "yes", "hwr", "down" });
            result.Rows.Add(new string[] { "mid", "no", "hwr", "up" });
            result.Rows.Add(new string[] { "mid", "no", "swr", "up" });
            result.Rows.Add(new string[] { "new", "yes", "swr", "up" });
            result.Rows.Add(new string[] { "new", "no", "hwr", "up" });
            result.Rows.Add(new string[] { "new", "no", "swr", "up" });

            return result;
        }

        static void Main(string[] args)
        {
            ID3.Attribute age = new ID3.Attribute("age", new List<string>(new string[] { "new", "mid", "old" }));
            ID3.Attribute comp = new ID3.Attribute("competition", new List<string>(new string[] { "yes", "no" }));
            ID3.Attribute type = new ID3.Attribute("type", new List<string>(new string[] { "swr", "hwr" }));

            List<ID3.Attribute> attributes = new List<ID3.Attribute>(new ID3.Attribute[] { age, comp, type });

            DataTable samples = getDataTable();
            ID3Tree id3 = new ID3Tree();
            TreeNode root = id3.BuildID3Tree(samples, "result", attributes);

            Console.Write(root.ToString());

            Console.ReadKey();
        }
    }
}
