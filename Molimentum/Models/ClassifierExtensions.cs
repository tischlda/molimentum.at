using System;
using System.Collections.Generic;

namespace Molimentum.Models
{
    public static class ClassifierExtensions
    {
        public static IEnumerable<ClassifiedItem<T>> AsCLassified<T>(this IEnumerable<T> items, Classifier classifier, Func<T, double> getWeight)
        {
            return classifier.Classify(items, getWeight);
        }
    }
}