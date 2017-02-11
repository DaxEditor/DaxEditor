using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaxEditor.MeasuresExtensions
{
    public static class MeasuresExtensions
    {
        public static IList<DaxMeasure> GetSupportingMeasures(this IList<DaxMeasure> measures)
        {
            return MeasureUtilities.GetSupportingMeasures(measures);
        }

        public static IList<DaxMeasure> GetNotSupportingMeasures(this IList<DaxMeasure> measures)
        {
            return MeasureUtilities.GetNotSupportingMeasures(measures);
        }
    }
}
