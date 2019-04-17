using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using CsvHelper;

namespace Cust360Simulator.Core
{
    /// <summary>
    /// This class is meant to run on a nightly basis
    /// To pick up a CSV of loyalty membership data
    /// And put it into the Customer 360 staging bucket (in Couchbase)
    /// </summary>
    public class LoyaltyCsvImportService
    {
        private readonly IBucket _bucket;

        public LoyaltyCsvImportService(IBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket("staging");
        }

        public async Task ImportCsv()
        {
            // TODO: find the newest created CSV file in the target folder
            // TODO: if it exists!
            var csvFilePath = GetNewestCsvPath();
            if (csvFilePath == null)
                return;

            // TODO: check to make sure it's newer than the file that was imported last time
            // TODO: if not, then bail out
            if (!ThisFileIsNewerThanTheLastImport(csvFilePath))
            {
                // TODO: log this?
                return;
            }

            // start importing from CSV into Couchbase
            using (var reader = new StreamReader(csvFilePath))
            {
                using (var csv = new CsvReader(reader))
                {
                    // TODO: okay to use dynamic here?
                    var records = csv.GetRecords<dynamic>();

                    // TODO: consider pool size / muxing
                    var documents = new List<IDocument<dynamic>>();
                    foreach (var record in records)
                    {
                        // TODO: insert a document into Couchbase staging bucket
                        var document = new Document<dynamic>
                        {
                            Id = $"loyalty-member-{record.Id}",
                            Content = record,
                            Expiry = 90000    // TODO: milliseconds? should these even be TTL'd?
                        };
                        documents.Add(document);
                    }

                    // TODO: batch insert
                    // TODO: break this up into multiple batches?
                    await _bucket.InsertAsync(documents);
                }
            }

            // TODO: keep track of the latest CSV file that has been imported and when it was imported
            TrackImportComplete(csvFilePath);
        }

        private void TrackImportComplete(string csvFilePath)
        {
            // TODO: store the filename that was used as well as the current datetime
            // TODO: so that we can check next time

            // TODO: another approach would be just to delete the CSV file?
        }

        private bool ThisFileIsNewerThanTheLastImport(string csvFilePath)
        {
            // TODO: check the data stored by TrackImportComplete

            return true;
        }

        private string GetNewestCsvPath()
        {
            var directory = new DirectoryInfo("CsvExports");
            var csvFiles = directory.GetFiles("*.csv")
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();

            // TODO: if a file was found, log it?

            return csvFiles?.FullName;
        }
    }
}