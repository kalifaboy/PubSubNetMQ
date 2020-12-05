using NetMQ;
using NetMQ.Sockets;
using System;
using System.Threading.Tasks;

namespace PublisherExample
{
    class Program
    {
        private const string TOPIC_A = "TopicA";
        private const string TOPIC_B = "TopicB";
        static async Task Main(string[] args)
        {
            using var publisherSocket = new PublisherSocket(">tcp://localhost:15000");
            Console.WriteLine("Publisher socket connecting...");
            publisherSocket.Options.SendHighWatermark = 1000;
            var rand = new Random(50);
            while (true)
            {
                var randomizedTopic = rand.NextDouble();
                var msg = new NetMQMessage();
                if (randomizedTopic > 0.5)
                {
                    msg.Append(TOPIC_A);
                    msg.Append("TopicA - hello there !");
                }
                else
                {
                    msg.Append(TOPIC_B);
                    msg.Append("TopicB - Bay there !");
                }

                publisherSocket.SendMultipartMessage(msg);
                Console.WriteLine(msg.Last.ConvertToString());
                await Task.Delay(100);
            }
        }
    }
}
