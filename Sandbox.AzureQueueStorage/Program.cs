using Sandbox.AzureQueueStorage;

Console.WriteLine( "Start" );

var app = new AzureQueueApp();

await app.Start();

