using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaxEditor
{
    /// <summary>
    /// Utilities for measures.
    /// </summary>
    public static class MeasureUtilities
    {
        /// <summary>
        /// Checks by the name of the measure, whether it is a supporting or not.
        /// Used for KPI implementation of XML bim files.
        /// </summary>
        /// <param name="measureName">The name of the measure</param>
        /// <exception cref="ArgumentNullException">If measureName is null</exception>
        /// <returns>true if measure is a supporting</returns>
        public static bool IsSupportingMeasure(string measureName)
        {
            if (measureName == null)
            {
                throw new ArgumentNullException(nameof(measureName));
            }

            return measureName.StartsWith("_") && (
                   measureName.EndsWith(" Goal", StringComparison.OrdinalIgnoreCase) ||
                   measureName.EndsWith(" Status", StringComparison.OrdinalIgnoreCase) ||
                   measureName.EndsWith(" Trend", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks by the measure, whether it is a supporting or not.
        /// Used for KPI implementation of XML bim files.
        /// </summary>
        /// <param name="measure">The measure</param>
        /// <exception cref="ArgumentNullException">If measure is null</exception>
        /// <returns>true if measure is a supporting</returns>
        public static bool IsSupportingMeasure(DaxMeasure measure)
        {
            if (measure == null)
            {
                throw new ArgumentNullException(nameof(measure));
            }

            return IsSupportingMeasure(measure.Name);
        }

        /// <summary>
        /// Returns not supporting measures.
        /// </summary>
        /// <param name="measures">Input measures</param>
        /// <exception cref="ArgumentNullException">If measures is null</exception>
        /// <returns>Not supporting measures</returns>
        public static IList<DaxMeasure> GetNotSupportingMeasures(IList<DaxMeasure> measures)
        {
            if (measures == null)
            {
                throw new ArgumentNullException(nameof(measures));
            }

            return measures.Where(i => !IsSupportingMeasure(i)).ToList();
        }

        /// <summary>
        /// Returns supporting measures.
        /// </summary>
        /// <param name="measures">Input measures</param>
        /// <exception cref="ArgumentNullException">If measures is null</exception>
        /// <returns>Supporting measures</returns>
        public static IList<DaxMeasure> GetSupportingMeasures(IList<DaxMeasure> measures)
        {
            if (measures == null)
            {
                throw new ArgumentNullException(nameof(measures));
            }

            return measures.Where(i => IsSupportingMeasure(i)).ToList();
        }
    }
}
