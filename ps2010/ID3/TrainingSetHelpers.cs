using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ID3
{
    /// <summary>
    /// Represent Atribute Result Pair.
    /// </summary>
    internal class AtributeResultPair
    {
        /// <summary>
        /// Attribute.
        /// </summary>
        public string Attribute { get; set; }
        
        /// <summary>
        /// Result.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="attribute">attribute</param>
        /// <param name="result">result</param>
        public AtributeResultPair(string attribute, string result)
        {
            Attribute = attribute;
            Result = result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (!(obj is AtributeResultPair))
                return false;

            var that = obj as AtributeResultPair;

            if (this.Attribute.Equals(that.Attribute) && this.Result.Equals(that.Result))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return Attribute.GetHashCode() + Result.GetHashCode();
        }
    }

    /// <summary>
    /// Class with provides operation on training set
    /// </summary>
    internal static class TrainingSetHelpers
    {
        /// <summary>
        /// Computes atribite value's count in training set.
        /// </summary>
        /// <param name="trainingSet">training set</param>
        /// <param name="attribute">attribite</param>
        /// <returns>{atribite value : atribite value's count in training set} dictonary</returns>
        public static Dictionary<string, int> AtributeElementsCount(DataTable trainingSet, Attribute attribute)
        {
            var dict = new Dictionary<string, int>();

            var elements = from t in trainingSet.AsEnumerable()
                           select t.Field<string>(attribute.Name);

            foreach (string element in elements)
            {
                int value = dict.ContainsKey(element) ? dict[element] : 0;
                value++;
                dict[element] = value;
            }

            return dict;
        }


        /// <summary>
        /// Computes atribute-result values pair's count in training set. 
        /// </summary>
        /// <param name="trainingSet">training set</param>
        /// <param name="attribute">attribute</param>
        /// <param name="result">seeked value</param>
        /// <returns>{atribute-result values pair : atribute-result values pair's count in training set} dictionary</returns>
        public static Dictionary<AtributeResultPair, int> AtributeResultElementsCount(DataTable trainingSet, Attribute attribute, Attribute result)
        {
            var dict = new Dictionary<AtributeResultPair, int>();

            var pairs = from t in trainingSet.AsEnumerable()
                        select new AtributeResultPair(t.Field<string>(attribute.Name), t.Field<string>(result.Name));

            foreach (AtributeResultPair pair in pairs)
            {
                int value = dict.ContainsKey(pair) ? dict[pair] : 0;
                value++;
                dict[pair] = value;
            }

            return dict;
        }
    }
}
