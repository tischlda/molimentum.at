using System;
using System.Collections.Generic;
using System.Linq;

namespace Molimentum.Models
{
    public class Classifier
    {
        private readonly List<double> _classes;

        public Classifier(IClassBuilder classBuilder)
        {
            _classes = classBuilder.BuildClasses();
        }

        public IEnumerable<ClassifiedItem<T>> Classify<T>(IEnumerable<T> items, Func<T, double> getWeight)
        {
            var max = items.Max(item => getWeight(item));

            foreach (var item in items)
            {
                var relativeWeight = getWeight(item) / max;

                yield return new ClassifiedItem<T>
                {
                    Item = item,
                    Class = _classes.FindLastIndex(classWeight => classWeight >= relativeWeight) + 1,
                };
            }
        }
    }
}