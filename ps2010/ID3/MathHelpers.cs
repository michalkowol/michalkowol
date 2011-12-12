using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ID3
{
    /// <summary>
    /// Class which helps with the mathematical part of ID3 algorithm
    /// </summary>
    internal static class MathHelpers
    {
        /// <summary>
        /// Computes amount of information store in attribite.
        /// </summary>
        /// <param name="trainingSet">training set</param>
        /// <param name="attibite">attribute we want to check</param>
        /// <returns>amount of information store in attribite</returns>
        public static double Info(DataTable trainingSet, Attribute attibite)
        {
            var atributeElementsCount = TrainingSetHelpers.AtributeElementsCount(trainingSet, attibite);

            double trainingSetSize = trainingSet.Rows.Count;
            var pn = from elementCount in atributeElementsCount.Values
                     select elementCount / trainingSetSize;

            return Entropy(pn);
        }

        /// <summary>
        /// Computes amount of information store in attribite-result pair.
        /// </summary>
        /// <param name="trainingSet">training set</param>
        /// <param name="attribute">attribute we want to check</param>
        /// <param name="result">categorial attribute</param>
        /// <returns>amount of information store in attribite-result pair</returns>
        public static double InfoAttribute(DataTable trainingSet, Attribute attribute, Attribute result)
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

        /// <summary>
        /// Counts the gain value for attribute.
        /// </summary>
        /// <param name="trainingSet">training set</param>
        /// <param name="attribute">attribute we want to check</param>
        /// <param name="result">categorial attribute</param>
        /// <returns>counted gain</returns>
        public static double Gain(DataTable trainingSet, Attribute attribute, Attribute result)
        {
            return Info(trainingSet, result) - InfoAttribute(trainingSet, attribute, result);
        }

        /// <summary>
        /// Countes the gain ratio for attribute.
        /// </summary>
        /// <param name="trainingSet">training set</param>
        /// <param name="attribute">attribute we want to check</param>
        /// <param name="result">categorial attribute</param>
        /// <returns>counted gain ratio</returns>
        public static double GainRatio(DataTable trainingSet, Attribute attribute, Attribute result)
        {
            return Gain(trainingSet, attribute, result) / Info(trainingSet, attribute);
        }

        /// <summary>
        /// Counts the entropy.
        /// </summary>
        /// <param name="pn">list of parameters</param>
        /// <returns>entropy</returns>
        public static double Entropy(IEnumerable<double> pn)
        {
            double result = 0.0;
            foreach (var p in pn)
                result += p * Math.Log(p, 2);

            return -result;
        }
    }
}
