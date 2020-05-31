using System;
using System.Collections.Generic;

namespace ModelBase
{
    [Serializable]
    public class LineContainer : Dictionary<int, Line>
    {
        public Map Map { get; }
        int counter = 0;
        public void Add(Line line)
        {
            counter++;
            Add(counter, line);
            line.ID = counter;
        }

        public LineContainer(Map map) => Map = map;
      

        public void Reset()
        {
            Clear();
            counter = 0;
        }
    }
}
