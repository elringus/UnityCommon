﻿
namespace UnityCommon
{
    /// <summary>
    /// Represents an integer range starting with <see cref="StartIndex"/> and ending with <see cref="EndIndex"/>.
    /// Both endpoints are considered to be included.
    /// </summary>
    [System.Serializable]
    public readonly struct IntRange
    {
        public readonly int StartIndex;
        public readonly int EndIndex;

        public IntRange (int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public bool Contains (int index)
        {
            return index >= StartIndex && index <= EndIndex;
        }
    }
}
