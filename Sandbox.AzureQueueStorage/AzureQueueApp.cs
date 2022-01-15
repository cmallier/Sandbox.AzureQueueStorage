
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;


namespace Sandbox.AzureQueueStorage;

internal class AzureQueueApp
{
    public async Task Start()
    {
        string connectionString = Constants.AzureQueueStorageKey;
        string queueName = "queue-name";

        QueueClient queueClient = new QueueClient( connectionString, queueName );

        await queueClient.CreateIfNotExistsAsync();


        bool exitProgram = false;
        while( exitProgram == false )
        {
            Console.WriteLine( "What operation would you like to perform?" );
            Console.WriteLine( "  1 - Send message" );
            Console.WriteLine( "  2 - Peek at the next message" );
            Console.WriteLine( "  3 - Receive message" );
            Console.WriteLine( "  X - Exit program" );

            ConsoleKeyInfo option = Console.ReadKey();
            Console.WriteLine();  // ReadKey does not got the the next line, so this does
            Console.WriteLine();  // Provide some whitespace between the menu and the action

            switch( option.KeyChar )
            {
                case '1':
                    await SendMessageAsync( queueClient );
                    break;
                case '2':
                    await PeekMessageAsync( queueClient );
                    break;
                case '3':
                    await ReceiveMessageAsync( queueClient );
                    break;
                case 'X':
                    exitProgram = true;
                    break;
                default:
                    Console.WriteLine( "invalid choice" );
                    break;
            }
        }

    }

    static async Task SendMessageAsync( QueueClient queueClient )
    {
        // Message
        Console.WriteLine( "Enter message: " );
        string headline = Console.ReadLine();

        Console.WriteLine( "Enter location: " );
        string location = Console.ReadLine();

        MyQueueMessage message = new( headline, location );


        // Send
        string jsonMessage = JsonSerializer.Serialize( message );
        Response<SendReceipt> response = await queueClient.SendMessageAsync( jsonMessage );
        SendReceipt sendReceipt = response.Value;

        // Output
        Console.WriteLine( $"Message sent.  Message id={sendReceipt.MessageId}  Expiration time={sendReceipt.ExpirationTime}" );
        Console.WriteLine();
    }


    static async Task PeekMessageAsync( QueueClient queueClient )
    {
        // Peek
        Response<PeekedMessage> response = await queueClient.PeekMessageAsync();
        PeekedMessage message = response.Value;

        // Output
        Console.WriteLine( $"Message id  : {message.MessageId}" );
        Console.WriteLine( $"Inserted on : {message.InsertedOn}" );
        Console.WriteLine( "We are only peeking at the message, so another consumer could dequeue this message" );
    }


    static async Task ReceiveMessageAsync( QueueClient queueClient )
    {
        Response<QueueMessage> response = await queueClient.ReceiveMessageAsync();
        QueueMessage message = response.Value;

        if( message is null )
        {
            Console.WriteLine( "No more message" );
        }
        else
        {
            Console.WriteLine( $"Message id    : {message.MessageId}" );
            Console.WriteLine( $"Inserted on   : {message.InsertedOn}" );
            Console.WriteLine( $"Message (raw) : {message.Body}" );

            MyQueueMessage myMessage = message.Body.ToObjectFromJson<MyQueueMessage>();
            Console.WriteLine( "News Article" );
            Console.WriteLine( $"-  Headline : {myMessage.Title}" );
            Console.WriteLine( $"-  Location : {myMessage.Country}" );

            Console.WriteLine( "The processing for this message is just printing it out, so now it will be deleted" );
            await queueClient.DeleteMessageAsync( message.MessageId, message.PopReceipt );
            Console.WriteLine( $"Message deleted" );
        }

    }

}

internal record MyQueueMessage( string? Title, string? Country );
