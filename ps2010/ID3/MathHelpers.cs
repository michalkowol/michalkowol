using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ID3
{
    internal static class MathHelpers
    {
        public static double Info(DataTable trainingSet, Attribute result)
        {
            var atributeElementsCount = TrainingSetHelpers.AtributeElementsCount(trainingSet, result);

            double trainingSetSize = trainingSet.Rows.Count;
            var pn = from elementCount in atributeElementsCount.Values
                     select elementCount / trainingSetSize;

            return Entropy(pn);
        }

        public static double InfoAtribute(DataTable trainingSet, Attribute attribute, Attribute result)
        {
            double infoAttribute = 0.0;
            double trainingSetSize = trainingSet.Rows.Count;

            var atributeElementsCount = TrainingSetHelpers.AtributeElementsCount(trainingSet, attribute);
            var atributeResultElementsCount = TrainingSetHelpers.AtributeResultElementsCount(trainingSet, attribute, result);
            
            foreach (var element in atributeElementsCount)
            {
                double count = element.Value;
                string atributeName = element.Key;

                var details = from elementResultCount in atributeResultElementsCount
                              where elementResultCount.Key.Attribute == atributeName
                              select elementResultCount.Value / count;

                infoAttribute += count / trainingSetSize * Entropy(details);
            }

            return infoAttribute;
        }

        public static double Gain(DataTable trainingSet, Attribute attribute, Attribute result)
        {
            return Info(trainingSet, result) - InfoAtribute(trainingSet, attribute, result);
        }

        public static double GainRatio(DataTable trainingSet, Attribute attribute, Attribute result)
        {
            return Gain(trainingSet, attribute, result) / Info(trainingSet, attribute);
        }

        public static double Entropy(IEnumerable<double> pn)
        {
            double result = 0.0;
            foreach (var p in pn)
                result += p * Math.Log(p, 2);

            return -result;
        }
    }
}
