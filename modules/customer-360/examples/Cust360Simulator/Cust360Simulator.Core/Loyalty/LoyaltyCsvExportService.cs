using System;
using System.IO;
using CsvHelper;

namespace Cust360Simulator.Core.Loyalty
{
    /// <summary>
    /// This class is used to produce a CSV export
    /// Which is meant to run on a nightly basis
    /// To export all the membership data from the loyalty database
    /// </summary>
    public class LoyaltyCsvExportService
    {
        private readonly LoyaltyRepository _loyaltyRepository;

        public LoyaltyCsvExportService(LoyaltyRepository loyaltyRepository)
        {
            _loyaltyRepository = loyaltyRepository;
        }
        public void ExportCsv()
        {
            var members = _loyaltyRepository.GetAllMembersForExport();

            // example: loyalty04162019_1125AM.csv
            var timestampForFilename = DateTime.Now.ToString("MMddyyyy_hhmmss");
            var csvFileName = "loyalty" + timestampForFilename + ".csv";
            var csvFullPath = Path.Combine("CsvExports", csvFileName);

            using (var writer = new StreamWriter(csvFullPath))
            {
                var csv = new CsvWriter(writer);
                {
                    csv.WriteRecords(members);
                }
            }
        }
    }
}