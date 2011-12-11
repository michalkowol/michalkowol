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

        public override string ToString()
        {
            StringBuilder s = new StringBuilder(Attribute.Name + "\n");

            if (Children != null)
            {
                s.AppendLine("[");
                for (int i = 0; i < Children.Count; i++ )
                {
                    s.AppendLine(Attribute.Values[i] + " \n ( " + Children[i].ToString() + ")");
                }
                s.AppendLine("]");
            }

            return s.ToString();
        }
    }

    public class ID3Tree
    {
        private Attribute Result { get; set; }
        private List<Attribute> Attrs;
        public TreeNode Root { get; private set; }

        private string CheckIfAllExamplesEqual(DataTable trainingSet)
        {

            var columnElements = (from t in trainingSet.AsEnumerable()
                                  select t.Field<string>(Result.Name)).Distinct();

            if (columnElements.Count() != 1)
                return null;
            else
                return columnElements.First();

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
            DataTable newTable = trainingSet.Copy();


            var rowsToRemove = (from row in newTable.AsEnumerable()
                                where row.Field<string>(attribute.Name) != value
                                select row).ToList();


            foreach (var rowToRemove in rowsToRemove)
                rowToRemove.Delete();

            newTable.AcceptChanges();

            return newTable;
        }

        private List<Attribute> RemoveAttribute(List<Attribute> attributes, Attribute attributeToRemove)
        {
            List<Attribute> copyAttr = new List<Attribute>(attributes);
            copyAttr.Remove(attributeToRemove);
            return copyAttr;
        }

        public TreeNode BuildID3Tree(DataTable trainingSet, Attribute result, List<Attribute> attributes)
        {
            bool isValid = ValidateTrainingSet(trainingSet,attributes,result);
            if (!isValid)
                return null;

            Attrs = attributes;
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


        //validating dataset
        private bool checkIfAttributeExist(DataTable trainingSet, Attribute attr)
        {

                var columnElements = (from t in trainingSet.AsEnumerable()
                                        select t.Field<string>(attr.Name)).Distinct();


            
                foreach (string val in columnElements)
                {
                    if (!attr.Values.Contains(val))
                        throw new Exception("Wrong value in given data: " + val + "\n");
                }

            return true;
        }

        public bool ValidateTrainingSet(DataTable trainingSet, List<Attribute> attributes, Attribute result)
        {
            if (attributes == null || result == null || trainingSet == null)
                throw new Exception("One of parameter is null\n");

            if (trainingSet.Columns.Count != attributes.Count + 1)
                throw new Exception("Wrong number of columns in training data");

            foreach (Attribute attr in attributes)
            {

                if (!checkIfAttributeExist(trainingSet, attr))
                    return false;           
               
            }


            if (!checkIfAttributeExist(trainingSet, result))
                return false; 

            return true;
        }

        public bool ValidateTestSet(DataTable testSet)
        {
            if (Attrs == null || testSet == null)
                throw new Exception("One of parameter is null\n");

            if (testSet.Columns.Count != Attrs.Count)
                throw new Exception("Wrong number of columns in training data");

            foreach (Attribute attr in Attrs)
            {

                if (!checkIfAttributeExist(testSet, attr))
                    return false;

            }
            return true;
        }

        private TreeNode NextRoot(DataRow row, TreeNode root)
        {
            if (root.Children == null || root.Children.Count == 0)
                return root;
            else
            {
                string val = row.Field<string>(root.Attribute.Name);
                int childNum = root.Attribute.Values.IndexOf(val);
                return NextRoot(row, root.Children[childNum]);
            }
        }

        public List<string> CountResult(DataTable testSet)
        {
            ValidateTestSet(testSet);
            TreeNode localRoot;
            List<string> result = new List<string>();
            foreach (DataRow rowElement in testSet.Rows)
            {
                localRoot = Root;
                localRoot = NextRoot(rowElement, localRoot);
                result.Add(localRoot.Attribute.Name);
            }
            return result;
        }
    }
}
