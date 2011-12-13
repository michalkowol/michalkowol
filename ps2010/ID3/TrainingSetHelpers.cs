using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ID3
{
    internal class AtributeResultPair
    {
        public string Attribute { get; set; }
        public string Result { get; set; }

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

    internal static class TrainingSetHelpers
    {
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
