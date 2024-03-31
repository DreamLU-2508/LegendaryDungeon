using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class Rangef
    {
        public float start;
        public float end;

        public Rangef(float start, float length)
        {
            this.start = start;
            this.end = start + length;
        }
    }

    public class ChancefTable<T>
    {
        public class RollRangef
        {
            public T value;
            public Rangef range;

            public RollRangef(T value, Rangef range)
            {
                this.value = value;
                this.range = range;
            }
        }
        public List<RollRangef> ranges;
        private float maxRoll = 0f;

        public ChancefTable()
        {
            ranges = new List<RollRangef>();
        }
        public bool CanRoll { get => ranges.Count > 0 && maxRoll > 0f; }
        public bool AddRange(float rangeLength, T value)
        {
            if (rangeLength > 0f)
            {
                this.ranges.Add(new RollRangef(value, new Rangef(maxRoll, rangeLength)));
                maxRoll += rangeLength;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// If the total chance is lesser than 1.0 => The roll can fail.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="missingValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryRoll(URandom rand, T missingValue, out T result)
        {
            result = missingValue;
            float value = rand.NextFloat(0f, maxRoll < 1f || Mathf.Approximately(maxRoll, 1f) ? 1f : maxRoll);
            for (int i = 0; i < ranges.Count; i++)
            {
                RollRangef rollRangef = ranges[i];
                if (rollRangef.range.start <= value && value < rollRangef.range.end)
                {
                    result =  rollRangef.value;
                    return true;
                }
            }
            return false;
        }

        public T RollWithinMaxRange(URandom rand)
        {
            float rollValue = rand.NextFloat(0, maxRoll);
            for (int i = 0; i < ranges.Count; i++)
            {
                var aRange = ranges[i].range;
                
                if (aRange.start <= rollValue && aRange.end > rollValue)
                {
                    return ranges[i].value;
                }
            }
            // Otherwise, crash
            throw new System.Exception("ChanceTable: internal error: value " + rollValue + " maxRoll:" + maxRoll + "ranges " + ranges.Count);
        }
    }
}