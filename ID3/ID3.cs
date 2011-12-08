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
        private string name;
        private ArrayList values;

        public Attribute(string _name, IList _values)
        {
            name = _name;
            values = new ArrayList(values);
            values.Sort();
        }

        public Attribute(string _name)
        {
            name = _name;
            values = null;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public ArrayList Values
        {
            get
            {
                if (values == null)
                    return null;
                else
                    return new ArrayList(values);
            }
        }

        public int indexOf(string value)
        {
            if (values != null)
                return values.BinarySearch(value);
            else
                return -1;
        }

        public bool isValidValue(string value)
        {
            return indexOf(value) >= 0;
        }

        public override string ToString()
        {
            if (values != null)
                return name + "  " + values.ToString();
            else
                return name;
        }
    }


    public class TreeNode
    {
        private Attribute attribute;
        private ArrayList children = null; // dziecko odpowiada tej wartosci atrybutu co kolejnośc na liście attribute.values

        public TreeNode(Attribute attr)
        {
            attribute = attr;
            if (attr.Values != null)
            {
                children = new ArrayList(attribute.Values.Count);
                for (int i = 0; i < attribute.Values.Count; i++)
                    children.Add(null);
            }
        }

        public void AddTreeNode(TreeNode treeNode, string ValueName)
        {
            int index = attribute.indexOf(ValueName);
            if (children != null && index >= 0)
                children[index] = treeNode;
        }

        public int nummberOfChildren
        {
            get
            {
                if (children == null)
                    return 0;
                else
                    return children.Count;
            }
        }

        public TreeNode getChild(int index)
        {
            if (children == null)
                return null;
            else
                return (TreeNode)children[index];
        }

        public void setChild(int index, TreeNode child)
        {
            if (children != null)
                children[index] = child;
        }

        public Attribute Attribute
        {
            get
            {
                return attribute;
            }
        }

        public TreeNode getChildByBranch(string branchName)
        {
            int index = attribute.indexOf(branchName);
            if (children == null || index < 0)
                return null;
            else
                return (TreeNode)children[index];
        }
    }

    public class ID3Tree
    {

        private string checkIfAllExamplesEqual(DataTable trainingSet, IList possibleValues)
        {
            //TODO
            return null;
        }

        private string getMostCommonResult(DataTable trainingSet)
        {
            //TODO
            return null;
        }

        private Attribute getAttrWithBiggestGainRatio(DataTable trainingSet, Attribute[] attributes)
        {
            //TODO
            return null;
        }

        private DataTable removeUsedValues(DataTable trainingSet, Attribute attribute, string value)
        {
            //TODO
            return null;
        }

        private Attribute[] removeAttribute(Attribute[] attributes)
        {
            //TODO
            return null;
        }

        public TreeNode buildID3Tree(DataTable trainingSet, Attribute result, Attribute[] attributes)
        {

            if (trainingSet.Rows.Count == 0)
                return new TreeNode(new Attribute("FAILURE"));

            string resultValue;
            resultValue = checkIfAllExamplesEqual(trainingSet, result.Values);
            if (resultValue != null)
                return new TreeNode(new Attribute(resultValue));

            if (attributes.Length == 0)
                return new TreeNode(new Attribute(getMostCommonResult(trainingSet)));

            Attribute attr = getAttrWithBiggestGainRatio(trainingSet, attributes);

            TreeNode root = new TreeNode(attr);

            for (int i = 0; i < attr.Values.Count; i++)
            {
                root.setChild(i, buildID3Tree(removeUsedValues(trainingSet, attr, (string)attr.Values[i]), result, removeAttribute(attributes)));
            }


            return root;
        }
    }




    class ExampleID3
    {
        static void Main(string[] args)
        {
        }
    }
}
