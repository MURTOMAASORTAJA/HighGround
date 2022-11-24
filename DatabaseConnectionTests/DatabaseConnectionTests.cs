using Data.Models;
using Data.Models.Event;
using Data.Models.Work;
using HighGround;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DatabaseConnectionTests
{
    [TestFixture]
    public class DatabaseConnectionTests
    {
        private MongoClient _client;
        private IConfiguration _conf;

        [SetUp]
        public void Setup()
        {
            var confBuilder = new ConfigurationBuilder();
            confBuilder.AddJsonFile("appsettings.json", true);
            confBuilder.AddUserSecrets<DatabaseConnectionTests>();
            _conf = confBuilder.Build();
            _client = new MongoClient(GetMongoClientSettingsFromUserSecrets());

        }

        [TearDown]
        public void CleanDatabase()
        {
            var db = _client.GetDatabase(_conf.GetSection("MongoDbName").Value);
            var collections = db.ListCollectionNames().ToList().Select(c => 
            {
                db.DropCollection(c);
                return true;
            });
        }

        [Test]
        public void ShouldGetCorrectMongoClientSettings()
        {
            var host = _conf.GetSection("MongoHost").Value;
            var port = int.Parse(_conf.GetSection("MongoPort").Value);
            var db = _conf.GetSection("MongoDbName").Value;
            var user = _conf.GetSection("MongoDbUser").Value;
            var pass = _conf.GetSection("MongoDbPassword").Value;

            var settings = GetMongoClientSettingsFromUserSecrets();

            Assert.That(settings.Server.Host, Is.EqualTo(host));
            Assert.That(settings.Server.Port, Is.EqualTo(port));
            Assert.That(settings.Credential.Identity.Source, Is.EqualTo(db));
            Assert.That(settings.Credential.Identity.Username, Is.EqualTo(user));
            Assert.That(settings.Credential.Evidence, Is.EqualTo(new PasswordEvidence(pass)));
        }

        [Test]
        public void ShouldGetDatabaseAndCreateAndGetAndDropCollection()
        {
            // todo: this isn't the test I intended to write!

            var db = _client.GetDatabase(_conf.GetSection("MongoDbName").Value);
            Assert.That(db, Is.Not.Null);
            var collectionName = "testcollection";

            db.CreateCollection(collectionName);

            var collection = db.GetCollection<object>(collectionName);
            Assert.NotNull(collection);

            db.DropCollection(collectionName);
            var collectionNames = GetCollectionNames(db);

            Assert.That(collectionNames, Does.Not.Contain(collectionName));
        }

        [Test]
        public async Task ShouldInsertPerson()
        {
            var testPerson = new Person() { PersonName = "TestPerson", EmailAddress = "some@thing.com" };
            var dbc = GetDatabaseConnection();
            var insertResult = await dbc.InsertAsync(testPerson);

            var db = _client.GetDatabase(_conf.GetSection("MongoDbName").Value);
            var collection = db.GetCollection<Person>("people");
            var created = collection.AsQueryable().Where(p => p.Id == insertResult.Item1!.Value).FirstOrDefault();
            collection.DeleteOne(p => p.Id == insertResult.Item2.Id);

            Assert.That(insertResult.Item1, Is.Not.Null);
            Assert.That(insertResult.Item1, Is.Not.EqualTo(Guid.Empty));
            Assert.That(insertResult.Item2, Is.InstanceOf(typeof(Person)));
            Assert.That(created, Is.Not.Null);
            Assert.That(created.Id, Is.EqualTo(insertResult.Item2.Id));
            Assert.That(created.Id, Is.Not.EqualTo(testPerson.Id));
            Assert.That(created.EmailAddress, Is.EqualTo(testPerson.EmailAddress));
            Assert.That(created.PersonName, Is.EqualTo(testPerson.PersonName));
        }

        [Test]
        public async Task ShouldInsertEvent()
        {
            var testEvent = new Event()
            {
                Information = new Information()
                {
                    Title = "Best event",
                    Description = "This is the best event."
                },
                RSVP = new RSVPBook()
                {
                    Inclusivity = Enums.SignupInclusivity.InviteOnly,
                    Entries = new[]
                    {
                        new Entry() { PersonName = "Some Person" },
                        new Entry() { PersonName = "Another Person" }
                    }
                }
            };

            var dbc = GetDatabaseConnection();
            var insertResult = await dbc.InsertAsync(testEvent);

            var db = _client.GetDatabase(_conf.GetSection("MongoDbName").Value);
            var collection = db.GetCollection<Event>("events");
            var created = collection.AsQueryable().Where(p => p.Id == insertResult.Item1!.Value).FirstOrDefault();
            collection.DeleteOne(p => p.Id == insertResult.Item2.Id);

            Assert.That(insertResult.Item1, Is.Not.Null);
            Assert.That(insertResult.Item1, Is.Not.EqualTo(Guid.Empty));
            Assert.That(insertResult.Item2, Is.InstanceOf(typeof(Event)));
            Assert.That(created, Is.Not.Null);
            Assert.That(created.Id, Is.EqualTo(insertResult.Item2.Id));
            Assert.That(created.Id, Is.Not.EqualTo(testEvent.Id));
            Assert.That(created.Information.Title, Is.EqualTo(testEvent.Information.Title));
            Assert.That(created.Information.Description, Is.EqualTo(testEvent.Information.Description));
            Assert.That(created.RSVP.Entries.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task ShouldInsertAnonymousIdentifiable()
        {
            var gig = new Gig()
            {
                Definition = new GigDefinition()
                {
                    Information = new Information()
                    {
                        Title = "Some gig",
                        Description = "Something something something."
                    }
                },
                ResponsibleGigId = Guid.NewGuid(),
                Start = DateTime.Now.AddMonths(1).AddHours(4),
                Finish = DateTime.Now.AddMonths(1).AddHours(9)
            };
            object obj = gig;

            var dbc = GetDatabaseConnection();
            var insertResult = await dbc.InsertAsync(obj);

            var db = _client.GetDatabase(_conf.GetSection("MongoDbName").Value);
            var collection = db.GetCollection<Gig>("gigs");
            var created = collection.AsQueryable().Where(p => p.Id == insertResult.Item1!.Value).FirstOrDefault();
            collection.DeleteOne(p => p.Id == insertResult.Item1.Value);

            Assert.That(insertResult.Item1, Is.Not.Null);
            Assert.That(insertResult.Item1, Is.Not.EqualTo(Guid.Empty));
            Assert.That(insertResult.Item2, Is.InstanceOf(typeof(Gig)));
            Assert.That(created, Is.Not.Null);
            Assert.That(created.Id, Is.EqualTo(insertResult.Item1.Value));
            Assert.That(created.Id, Is.Not.EqualTo(gig.Id));
            Assert.That(created.Definition.Information.Title, Is.EqualTo(gig.Definition.Information.Title));
            Assert.That(created.Definition.Information.Description, Is.EqualTo(gig.Definition.Information.Description));
        }

        private DatabaseConnection GetDatabaseConnection()
        {
            if (!_conf.GetSection("MongoHost").Exists())
            {
                throw new InvalidOperationException("The configuration doesn't contain MongoHost property.");
            }
            var host = _conf.GetSection("MongoHost").Value;
            var port = int.Parse(_conf.GetSection("MongoPort").Value);
            var db = _conf.GetSection("MongoDbName").Value;
            var user = _conf.GetSection("MongoDbUser").Value;
            var pass = _conf.GetSection("MongoDbPassword").Value;
            return new DatabaseConnection(host, port, user, pass, db);
        }

        private MongoClientSettings GetMongoClientSettingsFromUserSecrets()
        {
            var host = _conf.GetSection("MongoHost").Value;
            var port = int.Parse(_conf.GetSection("MongoPort").Value);
            var db = _conf.GetSection("MongoDbName").Value;
            var user = _conf.GetSection("MongoDbUser").Value;
            var pass = _conf.GetSection("MongoDbPassword").Value;

            return DatabaseConnection.GetMongoClientSettings(
                    host, port, user, pass, db);
        }

        private List<string> GetCollectionNames(IMongoDatabase db)
        {
            var cursor = db.ListCollectionNames();
            var result = new List<string>();
            while (cursor.MoveNext())
            {
                result.AddRange(cursor.Current);
            }

            return result;
        }

    }
}