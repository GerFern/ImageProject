namespace ModelBase
{
    public class RelationInfo
    {
        public RelationInfo(RelationType relationType, (Line, Line[])[] intersectedLines)
        {
            RelationType = relationType;
            IntersectedLines = intersectedLines;
        }

        public RelationType RelationType { get; }
        public (Line, Line[])[] IntersectedLines { get; }

        public override string ToString() => RelationType.ToString();
    }
}
