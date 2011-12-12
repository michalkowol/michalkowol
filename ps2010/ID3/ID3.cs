using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace ID3
{
    /// <summary>
    /// Class that represents attribiute.
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// Name of the attribute
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// List of all possible values for this attribute.
        /// </summary>
        public List<string> Values { get; private set; }

        /// <summary>
        /// Default constructor which sets name and values of the attribute.
        /// </summary>
        /// <param name="name">name of the attribute</param>
        /// <param name="values">possible values of the attributes</param>
        public Attribute(string name, List<string> values)
        {
            Name = name;
            Values = values;
            Values.Sort();
        }

        /// <summary>
        /// Constructor which sets the name of attribute and no values.
        /// </summary>
        /// <param name="name">name of the attribute</param>
        public Attribute(string name)
            : this(name, new List<string>())
        {
        }

        /// <summary>
        /// Returns index of given value.
        /// </summary>
        /// <param name="value">value to locate</param>
        /// <returns>index of given value if the value is found, otherwise negativ number</returns>
        public int IndexOf(string value)
        {
            return Values.BinarySearch(value);
        }

        /// <summary>
        /// Determines if a value is correct for the attribute.
        /// </summary>
        /// <param name="value">value check</param>
        /// <returns>true if attribute is correct, false otherwise</returns>
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

    /// <summary>
    /// Class that represents tree node in ID3 algorithm. 
    /// Tree node element has a node which is represented by Attribute and branches (Children) 
    /// which corresponds to list of Attribute's values. To each child can be conncted another TreeNode or it can be null.
    /// </summary>
    public class TreeNode
    {
        /// <summary>
        /// Tree node attribute.
        /// </summary>
        public Attribute Attribute { get; private set; }

        /// <summary>
        /// List of children for this tree node. 
        /// Each child in the list corresponds to a value in the list of Attribute values at the same index.
        /// </summary>
        public List<TreeNode> Children { get; private set; } // dziecko odpowiada tej wartosci atrybutu co kolejnośc na liście attribute.values

        /// <summary>
        /// Default constructor. Creates TreeNode with given attribute.
        /// </summary>
        /// <param name="attribute">the node attribute</param>
        public TreeNode(Attribute attribute)
        {
            Attribute = attribute;
            Children = new List<TreeNode>();
            foreach (var attr in Attribute.Values)
                Children.Add(null);
        }

        /// <summary>
        /// Adds new Child to the TreeNode to the selected value.
        /// </summary>
        /// <param name="treeNode">new TreeNode child to add</param>
        /// <param name="ValueName">Name of the value to which connect the new child</param>
        public void AddTreeNode(TreeNode treeNode, string ValueName)
        {
            int index = Attribute.IndexOf(ValueName);
            if (index >= 0)
                Children[index] = treeNode;
        }

        /// <summary>
        /// Returns total number of children for this node.
        /// </summary>
        public int NumberOfChildren
        {
            get { return Children.Count; }
        }

        /// <summary>
        /// Returns specified subtree.
        /// </summary>
        /// <param name="branchName">Name of the value which subtree we want to get  </param>
        /// <returns>TreeNode element which is the root of specified branch</returns>
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

    /// <summary>
    /// Class which count ID3 decision tree and make decisions based on the previously built tree.
    /// </summary>
    public class ID3Tree
    {
        /// <summary>
        /// Categorial attribute.
        /// </summary>
        private Attribute Result { get; set; }

        /// <summary>
        /// List of non-categorial attributes.
        /// </summary>
        private List<Attribute> Attrs;

        /// <summary>
        /// Root of the ID3Tree.
        /// </summary>
        public TreeNode Root { get; private set; }

        /// <summary>
        /// Checks if all examples in training set has the same value of categorial attribute
        /// </summary>
        /// <param name="trainingSet">training set</param>
        /// <returns>name of the value if ale examples have the same, otherwise null</returns>
        private string CheckIfAllExamplesEqual(DataTable trainingSet)
        {

            var columnElements = (from t in trainingSet.AsEnumerable()
                                  select t.Field<string>(Result.Name)).Distinct();

            if (columnElements.Count() != 1)
                return null;
            else
                return columnElements.First();

        }

        /// <summary>
        /// Returns the most common value of the categorial attribute for the given training set.
        /// </summary>
        /// <param name="trainingSet">training set</param>
        /// <returns>most common categorial attribute's value</returns>
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

        /// <summary>
        /// Returns non-categorial attriubute with the biggest gain ratio.
        /// </summary>
        /// <param name="trainingSet">training set</param>
        /// <param name="attributes">list of non-categorial attributes</param>
        /// <returns>attribute with the biggest gain ratio</returns>
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

        /// <summary>
        /// Removes rows from training set which are wrong for particular ID3 subtree. 
        /// Leave only this rows which have paticular value of given attribute.
        /// </summary>
        /// <param name="trainingSet">original training set</param>
        /// <param name="attribute">attribute we want to determine</param>
        /// <param name="value">value of the given attribute we want to leave</param>
        /// <returns>new training set only with selected, correct rows</returns>
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

        /// <summary>
        /// Removed non-categorial attribute which is already used in ID3 decision tree.
        /// </summary>
        /// <param name="attributes">list of non-categorial attributes</param>
        /// <param name="attributeToRemove">attribute to remove</param>
        /// <returns>new list without given attribute</returns>
        private List<Attribute> RemoveAttribute(List<Attribute> attributes, Attribute attributeToRemove)
        {
            List<Attribute> copyAttr = new List<Attribute>(attributes);
            copyAttr.Remove(attributeToRemove);
            return copyAttr;
        }

        /// <summary>
        /// Builds the ID3 decision tree based on the given training set, set of non-categorical attributes and categorial attribute.
        /// </summary>
        /// <param name="trainingSet">training set to build the tree</param>
        /// <param name="result">categorial attribute</param>
        /// <param name="attributes">list of nont-categorial attributes</param>
        /// <returns>Root node from the ID3 decision tree</returns>
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

        /// <summary>
        /// Build recursive ID3 decision tree.
        /// </summary>
        /// <param name="trainingSet">training set of data</param>
        /// <param name="attributes">list of non-categorial attributes</param>
        /// <returns>root node for the given data</returns>
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


        /// <summary>
        /// Check if values for an attribute given in training set really exist.
        /// </summary>
        /// <param name="trainingSet">training set of values</param>
        /// <param name="attr">attribute to check</param>
        /// <returns>true if exist, otherwise for an exception</returns>
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

        /// <summary>
        /// Check correctness of given training set, set of non-categorical attributes and categorial attribute.
        /// </summary>
        /// <param name="trainingSet">training set to check</param>
        /// <param name="attributes">non-categorial attributes to check</param>
        /// <param name="result">categorial attribute to chceck</param>
        /// <returns>true if given parameters are correct, otherwise throws exception</returns>
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

        /// <summary>
        /// Chceck correctness of the test set, comparing it with previously given categorial attribute and non-categorial attributes.
        /// </summary>
        /// <param name="testSet">test set to check</param>
        /// <returns>true if given parameters are correct, otherwise throws exception</returns>
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

        /// <summary>
        /// Returns the next root for the given set of test data. 
        /// </summary>
        /// <param name="row">set of data</param>
        /// <param name="root">actual root</param>
        /// <returns>next root</returns>
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


        /// <summary>
        /// Count a decision based on the previously calculated ID3 decision tree.
        /// </summary>
        /// <param name="testSet">Problems which we want to resolve with already built decision tree</param>
        /// <returns>list of answer for given problems</returns>
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
