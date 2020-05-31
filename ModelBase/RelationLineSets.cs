using System.Collections.Generic;

namespace ModelBase
{
    public class RelationLineSets
    {
        LineSet owner;
        public Dictionary<LineSet, IntersectLines> others;

        public RelationLineSets(LineSet owner)
        {
            this.owner = owner;
        }

        public void Add(Line line, Line lineOther)
        {
            LineSet other = lineOther.Owner;
            IntersectLines intersected;
            if (!others.TryGetValue(other, out intersected))
            {
                intersected = new IntersectLines();
                others.Add(other, intersected);
            }
            List<Line> lines;
            if(!intersected.TryGetValue(line, out lines))
            {
                lines = new List<Line>();
                intersected.Add(line, lines);
            }
            lines.Add(lineOther);
        }
    }
}
