using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ID3;

namespace ID3Example
{
    class Program
    {
        static void Main(string[] args)
        {
            TreeNode node = new TreeNode(new ID3.Attribute("michal"));
            Console.WriteLine("ok!");
        }
    }
}
