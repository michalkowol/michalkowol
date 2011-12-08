using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace ID3
{
    public class Attribute
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }

        public Attribute(string name, List<string> values)
        {
            Name = name;
            Values = values;
            Values.Sort();
        }

        public Attribute(string name)
            : this(name, new List<string>())
        {
        }

        public int IndexOf(string value)
        {
            return Values.BinarySearch(value);
        }

        public bool IsValidValue(string value)
        {
            return Values.Contains(value);
        }

        public override string ToString()
        {
            return Name + " " + Values.ToString();
        }
    }


    public class TreeNode
    {
        public Attribute Attribute { get; set; }
        public List<TreeNode> Children { get; set; } // dziecko odpowiada tej wartosci atrybutu co kolejnośc na liście attribute.values

        public TreeNode(Attribute attribute)
        {
            Attribute = attribute;
            Children = new List<TreeNode>();
            foreach (var attr in Attribute.Values)
                Children.Add(null);
        }

        public void AddTreeNode(TreeNode treeNode, string ValueName)
        {
            int index = Attribute.IndexOf(ValueName);
            if (index >= 0)
                Children[index] = treeNode;
        }

        public int NumberOfChildren
        {
            get { return Children.Count; }
        }

        public TreeNode GetChildByBranch(string branchName)
        {
            int index = Attribute.IndexOf(branchName);
            if (index < 0)
                return null;
            else
                return Children[index];
        }
    }

    public class ID3Tree
    {

        private string checkIfAllExamplesEqual(DataTable trainingSet, List<string> possibleValues)
        {
            //TODO
            return null;
        }

        private string getMostCommonResult(DataTable trainingSet)
        {
            //TODO
            return null;
        }

        private Attribute getAttrWithBiggestGainRatio(DataTable trainingSet, List<Attribute> attributes)
        {
            //TODO
            return null;
        }

        private DataTable removeUsedValues(DataTable trainingSet, Attribute attribute, string value)
        {
            //TODO
            return null;
        }

        private List<Attribute> removeAttribute(List<Attribute> attributes)
        {

            return null;
        }

        public TreeNode buildID3Tree(DataTable trainingSet, Attribute result, List<Attribute> attributes)
        {

            if (trainingSet.Rows.Count == 0)
                return new TreeNode(new Attribute("FAILURE"));

            string resultValue;
            resultValue = checkIfAllExamplesEqual(trainingSet, result.Values);
            if (resultValue != null)
                return new TreeNode(new Attribute(resultValue));

            if (attributes.Count == 0)
                return new TreeNode(new Attribute(getMostCommonResult(trainingSet)));

            Attribute attr = getAttrWithBiggestGainRatio(trainingSet, attributes);

            TreeNode root = new TreeNode(attr);

            for (int i = 0; i < attr.Values.Count; i++)
                root.Children[i] = buildID3Tree(removeUsedValues(trainingSet, attr, attr.Values[i]), result, removeAttribute(attributes));

            return root;
        }
    }
}
