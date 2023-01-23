using Lumina.Excel.GeneratedSheets;

namespace AutoSummon
{
    internal class DutyManager
    {
        public Duty GetCurrentDuty()
        {
            var currentType = ServiceHandler.ClientState.TerritoryType;
            var collection =
                ServiceHandler.DataManager.GetExcelSheet<ContentFinderCondition>()!
                             .Where(a => a.TerritoryType.Row == currentType);
            var cfc = collection.DefaultIfEmpty(null).FirstOrDefault();
            if (cfc == null)
            {
                return new Duty()
                {
                    Classification = DutyClassification.NotInDuty,
                    Name = ""
                };
            }
            var duty = new Duty()
            {
                Name = cfc.Name,
                Classification = cfc.HighEndDuty ? DutyClassification.HighEnd : DutyClassification.Normal,
            };
            return duty;
        }

        private static DutyManager? _Instance = null!;
        public static DutyManager Instance => _Instance ??= new();
    }
}
