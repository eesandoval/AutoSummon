namespace AutoSummon
{
    internal enum DutyClassification
    {
        Normal,
        HighEnd,
        NotInDuty
    }

    internal struct Duty
    {
        public string Name;
        public DutyClassification Classification;
    }
}
