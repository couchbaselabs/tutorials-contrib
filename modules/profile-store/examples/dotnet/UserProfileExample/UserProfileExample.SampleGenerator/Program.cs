using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserProfileExample.Models;

namespace UserProfileExample.SampleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // basic settings
            var numberOfUserProfilesToGenerate = 25000;
            var fileToCreate = "user-profile-sample.json";
            var camelCaseSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            // start a new file if necessary
            if (File.Exists(fileToCreate))
                File.Delete(fileToCreate);

            // this will be "list" format, so start an array
            File.AppendAllText(fileToCreate, "[");

            for (var i = 0; i < numberOfUserProfilesToGenerate; i++)
            {
                // this will be "list" format, so put a comma before each object (except the first one)
                if (i != 0)
                    File.AppendAllText(fileToCreate, ",");

                // create randomish user, serialize to json, write to the file
                var user = FakeUser.Create();
                var userJson = JsonConvert.SerializeObject(user, camelCaseSettings);
                File.AppendAllText(fileToCreate, userJson);
            }

            // this will be "list" format, so end the array
            File.AppendAllText(fileToCreate, "]");
        }
    }
}
