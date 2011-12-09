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

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (!(obj is Attribute))
                return false;

            var that = obj as Attribute;

            if (this.Name.Equals(that.Name))
                return false;

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
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

        private string CheckIfAllExamplesEqual(DataTable trainingSet)
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

        private string GetMostCommonResult(DataTable trainingSet)
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

        private Attribute GetAttributeWithBiggestGainRatio(DataTable trainingSet, List<Attribute> attributes)
        {
            Attribute AttributeWithBiggestGainRatio = attributes.First();
            double maxGainRatio = 0;

            foreach (var attribute in attributes)
            {
                double attributeGainRatio = MathHelpers.GainRatio(trainingSet, attribute, Result);
                if (attributeGainRatio > maxGainRatio)
                {
                    AttributeWithBiggestGainRatio = attribute;
                    maxGainRatio = attributeGainRatio;
                }
            }

            return AttributeWithBiggestGainRatio;
        }

        private DataTable RemoveUsedValues(DataTable trainingSet, Attribute attribute, string value)
        {
            // TODO check it

            var rowsToRemove = (from row in trainingSet.AsEnumerable()
                                where row.Field<string>(attribute.Name) == value
                                select row).ToList();

            foreach (var rowToRemove in rowsToRemove)
                rowToRemove.Delete();

            trainingSet.AcceptChanges();

            return trainingSet;
        }

        private List<Attribute> RemoveAttribute(List<Attribute> attributes, Attribute attributeToRemove)
        {
            attributes.Remove(attributeToRemove);
            return attributes;
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
            if (trainingSet.Rows.Count == 0)
                return new TreeNode(new Attribute("FAILURE"));

            string resultValue = CheckIfAllExamplesEqual(trainingSet);
            if (resultValue != null)
                return new TreeNode(new Attribute(resultValue));

            if (attributes.Count == 0)
                return new TreeNode(new Attribute(GetMostCommonResult(trainingSet)));

            Attribute attributeWithBiggestGainRatio = GetAttributeWithBiggestGainRatio(trainingSet, attributes);

            TreeNode root = new TreeNode(attributeWithBiggestGainRatio);

            for (int i = 0; i < attributeWithBiggestGainRatio.Values.Count; i++)
                root.Children[i] = RecursiveBuildID3Tree(RemoveUsedValues(trainingSet, attributeWithBiggestGainRatio, attributeWithBiggestGainRatio.Values[i]), RemoveAttribute(attributes, attributeWithBiggestGainRatio));

            return root;
        }
    }
}
