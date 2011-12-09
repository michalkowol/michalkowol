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
        public string Name { get; private set; }
        public List<string> Values { get; private set; }

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
            return "[" + Name + ", " + Values.ToString() + "]";
        }
    }


    public class TreeNode
    {
        public Attribute Attribute { get; private set; }
        public List<TreeNode> Children { get; private set; } // dziecko odpowiada tej wartosci atrybutu co kolejnośc na liście attribute.values

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
        private Attribute Result { get; set; }
        public TreeNode Root { get; private set; }

        private string checkIfAllExamplesEqual(DataTable trainingSet)
        {
            foreach (DataColumn column in trainingSet.Columns)
            {
                var columnElements = (from t in trainingSet.AsEnumerable()
                                      select t.Field<string>(column)).Distinct();

                if (columnElements.Count() > 1)
                    return null;
            }

            var r = (from t in trainingSet.AsEnumerable()
                     select t.Field<string>(Result.Name)).First();

            return r;
        }

        private string getMostCommonResult(DataTable trainingSet)
        {
            Dictionary<string, int> elementCount = TrainingSetHelpers.AtributeElementsCount(trainingSet, Result);

            KeyValuePair<string, int> maxPair = elementCount.First();
            foreach (var pair in elementCount)
            {
                if (pair.Value > maxPair.Value)
                    maxPair = pair;
            }

            return maxPair.Key;
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


        public TreeNode BuildID3Tree(DataTable trainingSet, string result, List<Attribute> attributes)
        {
            return BuildID3Tree(trainingSet, new Attribute(result), attributes);
        }

        public TreeNode BuildID3Tree(DataTable trainingSet, Attribute result, List<Attribute> attributes)
        {
            Result = result;
            Root = RecursiveBuildID3Tree(trainingSet, attributes);
            return Root;
        }

        private TreeNode RecursiveBuildID3Tree(DataTable trainingSet, List<Attribute> attributes)
        {
            foreach (var a in attributes)
            {
                Console.WriteLine(a.Name + ", info(x, t): \t" + MathHelpers.InfoAtribute(trainingSet, a, Result));
                Console.WriteLine(a.Name + ", gain(x, t): \t" + MathHelpers.Gain(trainingSet, a, Result));
                Console.WriteLine(a.Name + ", split(x, t): \t" + MathHelpers.Info(trainingSet, a));
                Console.WriteLine(a.Name + ", gainratio(x, t): \t" + MathHelpers.GainRatio(trainingSet, a, Result));
            }

            if (trainingSet.Rows.Count == 0)
                return new TreeNode(new Attribute("FAILURE"));

            string resultValue = checkIfAllExamplesEqual(trainingSet);
            if (resultValue != null)
                return new TreeNode(new Attribute(resultValue));

            if (attributes.Count == 0)
                return new TreeNode(new Attribute(getMostCommonResult(trainingSet)));

            Attribute attr = getAttrWithBiggestGainRatio(trainingSet, attributes);

            TreeNode root = new TreeNode(attr);

            for (int i = 0; i < attr.Values.Count; i++)
                root.Children[i] = RecursiveBuildID3Tree(removeUsedValues(trainingSet, attr, attr.Values[i]), removeAttribute(attributes));

            return root;
        }
    }
}
