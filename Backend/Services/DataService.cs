using Data.Models;
using HighGround;

namespace Backend.Services
{
    public class DataService
    {
        private DatabaseConnection dbConn;

        private DatabaseConnection GetDatabaseConnection(IConfiguration configuration)
        {
            var host = configuration.GetSection("MongoHost").Value;
            var port = int.Parse(configuration.GetSection("MongoPort").Value);
            var db = configuration.GetSection("MongoDbName").Value;
            var user = configuration.GetSection("MongoDbUser").Value;
            var pass = configuration.GetSection("MongoDbPassword").Value;

            return new DatabaseConnection(host, port, user, pass, db);
        }

        public DataService(IConfiguration configuration)
        {
            dbConn = GetDatabaseConnection(configuration);
        }

        #region Person methods
        public Person? GetPerson(Guid id)
        {
            return dbConn.Get<Person>(id);
        }

        /// <summary>
        /// Returns an array of tuples (person id, person name) corresponding to Person objects that 
        /// have a PersonName property containing the substring provided with <paramref name="nameContains"/> parameter.
        /// </summary>
        public Tuple<Guid, string>[] FindPeople(string nameContains)
        {
            return dbConn.Query<Person>().Where(p =>
            !string.IsNullOrEmpty(p.PersonName) &&
            p.PersonName.ToLowerInvariant().Contains(nameContains.ToLowerInvariant())).Select(p => new Tuple<Guid, string>(p.Id, p.PersonName)).ToArray();
        }

        public async Task<Guid> InsertPerson(
            string personName, 
            string telegramUserId, 
            string phoneNumber, 
            string emailAddress, 
            bool emailWorkNotifications)
        {
            return (await dbConn.InsertAsync(new Person()
            {
                PersonName = personName,
                TelegramUserId = telegramUserId,
                PhoneNumber = phoneNumber,
                EmailAddress = emailAddress,
                EmailWorkNotifications = emailWorkNotifications
            })).Item1.GetValueOrDefault();
        }

        public async Task<bool> DeletePerson(Guid id)
        {
            return (await dbConn.DeleteAsync<Person>(id)).IsAcknowledged;
        }

        public Person[] ListPeople(int? skip, int? max)
        {
            return dbConn.List<Person>(skip, max);
        }

        public string[] ListPeopleNames(int? skip, int? max)
        {
            return dbConn
                .Query<Person>()
                .Skip(skip.GetValueOrDefault())
                .Take(max.GetValueOrDefault())
                .Select(p => p.PersonName).ToArray();
        }
        #endregion
    }
}
