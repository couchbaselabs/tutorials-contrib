using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace Cust360Simulator.Core.Loyalty
{
    /// <summary>
    /// This class is meant to run on a nightly basis
    /// To pick up a CSV of loyalty membership data
    /// And put it into the Customer 360 staging bucket (in Couchbase)
    /// </summary>
    public class LoyaltyCsvImportService
    {
        private readonly ILogger<LoyaltyCsvImportService> _logger;
        private readonly IBucket _bucket;

        public LoyaltyCsvImportService(IBucketProvider bucketProvider, ILogger<LoyaltyCsvImportService> logger)
        {
            _logger = logger;
            _bucket = bucketProvider.GetBucket("staging");
        }

        public async Task ImportCsv()
        {
            var csvFilePath = GetNewestCsvPath();
            if (csvFilePath == null)
                return;

            // check to make sure it's newer than the file that was imported last time
            // if not, then bail out
            if (!ThisFileIsNewerThanTheLastImport(csvFilePath))
            {
                _logger.LogInformation($"Not importing {csvFilePath}");
                return;
            }

            // start importing from CSV into Couchbase
            using (var reader = new StreamReader(csvFilePath))
            {
                using (var csv = new CsvReader(reader))
                {
                    var records = csv.GetRecords<dynamic>();

                    var documents = new List<IDocument<dynamic>>();
                    foreach (var record in records)
                    {
                        var document = new Document<dynamic>
                        {
                            Id = $"loyalty-member-{record.Id}",
                            Content = record,
                            Expiry = 90000
                        };
                        documents.Add(document);
                    }

                    // consider breaking this up into multiple batches
                    await _bucket.InsertAsync(documents);
                }
            }

            TrackImportComplete(csvFilePath);
        }

        // store the filename that was used as well as the current datetime
        // so that we can check next time
        private void TrackImportComplete(string csvFilePath)
        {
            var lastCsvImportInfo = new CsvImportTrackingInfo();
            lastCsvImportInfo.Filename = csvFilePath;
            lastCsvImportInfo.ImportedDateTime = DateTime.Now;

            _bucket.Upsert("csvImportTrackingInfo", lastCsvImportInfo);
        }

        // check both the date and the filename
        // to ensure we don't import a file that's already been imported
        private bool ThisFileIsNewerThanTheLastImport(string csvFilePath)
        {
            if (!_bucket.Exists("csvImportTrackingInfo"))
                return true;

            var lastCsvImportInfo = _bucket.Get<CsvImportTrackingInfo>("csvImportTrackingInfo").Value;
            var fileInfo = new FileInfo(csvFilePath);
            if (fileInfo.LastWriteTime <= lastCsvImportInfo.ImportedDateTime)
            {
                _logger.LogInformation($"{csvFilePath} is older than the last import.");
                return false;
            }

            if (fileInfo.Name == Path.GetFileName(lastCsvImportInfo.Filename))
            {
                _logger.LogInformation($"{csvFilePath} has already been imported.");
                return false;
            }
            return true;
        }

        private string GetNewestCsvPath()
        {
            var directory = new DirectoryInfo("CsvExports");
            var csvFile = directory.GetFiles("*.csv")
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();

            if(csvFile != null)
                _logger.LogInformation($"Trying to import from {csvFile.FullName}");

            return csvFile?.FullName;
        }
    }
}