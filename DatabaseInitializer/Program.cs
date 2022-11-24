// See https://aka.ms/new-console-template for more information

using MongoDB.Driver;
using Mono.Options;

var server = "";
var port = 0;
var username = "";
var password = "";
var db = "";
List<string> extra = new List<string>();

var options = new OptionSet
{
    { "s|server=", "MongoDB server hostname or IP address.", s => server = s },
    { "P|port=", "MongoDB server port", p => port = int.Parse(p) },
    { "u|username=", "MongoDB server username", u => username = u },
    { "p|password=", "MongoDB server password", p => password = p },
    { "db|database=", "MongoDB database name", d => db = d },
};

try
{
    extra = options.Parse(args);
}
catch (OptionException oEx)
{
    Console.WriteLine(oEx.Message);
    return;
}

ValidateArgs();




void ValidateArgs()
{
    var invalid = false;

    if (string.IsNullOrEmpty(server))
    {
        Console.WriteLine("Server can't be empty.");
        invalid = true;
    }

    if (port <= 0)
    {
        Console.WriteLine("Port is invalid.");
        invalid = true;
    }

    if (string.IsNullOrEmpty(username))
    {
        if (string.IsNullOrEmpty(password)) {
            Console.WriteLine("Both username and password can't be empty.");
        } else
        {
            Console.WriteLine("Username can't be empty.");
        }
        invalid = true;
    }

    if (invalid)
    {
        Environment.Exit(1);
    }
}

DoMongoStuff();

void DoMongoStuff()
{
    var connString = $"mongodb://{server}:{port}";
    var client = new MongoClient(connString);
    var db = client.GetDatabase("ug");
    db.ListCollectionNames();
}