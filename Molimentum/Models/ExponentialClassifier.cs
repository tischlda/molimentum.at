using System;
using System.Collections.Generic;
using System.Linq;

namespace Molimentum.Models
{
    public class ExponentialClassBuilder : IClassBuilder
    {
        private readonly int _classCount;
        private readonly double _exponent;
        
        public ExponentialClassBuilder(int classCount, double exponent)
        {
            _classCount = classCount;
            _exponent = exponent;
        }

        public List<double> BuildClasses()
        {
 	        return Enumerable
                .Range(0, _classCount)
                .Select(i => (Math.Pow(_exponent, i) - 1.0) / (Math.Pow(_exponent, _classCount - 1.0) - 1.0)).Reverse().ToList();
        }
    }
}