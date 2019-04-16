using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dapper;
using Npgsql;

namespace Cust360Simulator.Core
{
    public class LoyaltyRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public LoyaltyRepository(NpgsqlConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public void CreateNewMember()
        {
            var member = new LoyaltyMember();
            member.Password = Path.GetRandomFileName();
            member.FirstName = Faker.Name.First();
            member.LastName = Faker.Name.Last();
            member.Points = Faker.RandomNumber.Next(1, 100);

            _dbConnection.Execute(@"
                INSERT INTO members (password, fname, lname, points) VALUES (@Password,@FirstName,@LastName,@Points);", member);
        }

        public void UpdateLoyaltyMember()
        {
            var sql = "SELECT * FROM members ORDER BY RANDOM() LIMIT 1;";
            var member = _dbConnection.Query<LoyaltyMember>(sql).First();

            member.Points += Faker.RandomNumber.Next(1, 100);

            _dbConnection.Execute("UPDATE members SET points = @Points WHERE id = @Id",
                new {member.Points, member.Id});
        }

        public List<LoyaltyMember> GetAllMembersForExport()
        {
            // NOTE: there might be a better way to export data from postgres
            // ultimately into a CSV file
            var sql = "SELECT *, fname AS FirstName, lname AS LastName FROM members";
            var members = _dbConnection.Query<LoyaltyMember>(sql);
            return members.ToList();
        }
    }
}