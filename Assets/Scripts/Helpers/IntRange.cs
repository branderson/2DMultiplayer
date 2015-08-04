using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Helpers
{
    /// <summary>
    /// A simple example cass, which contains an integer range
    /// and a text property.
    /// </summary>
    public class IntRange : IRangeProvider<int>
    {
        public IntRange(int a, int b, int value)
        {
            Range = new Range<int>(a, b);
            Value = value;
        }

        public int Value 
        {
            get;
            set;
        }

        #region IRangeProvider<int> Members

        public Range<int> Range
        {
            get;
            set;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} ({1} - {2})", Value, Range.From, Range.To);
        }
    }

    /// <summary>
    /// Compares two range items by comparing their ranges.
    /// </summary>
    public class RangeItemComparer : IComparer<IntRange>
    {
        #region IComparer<IntRange> Members

        public int Compare(IntRange x, IntRange y)
        {
            return x.Range.CompareTo(y.Range);
        }

        #endregion
    }
}