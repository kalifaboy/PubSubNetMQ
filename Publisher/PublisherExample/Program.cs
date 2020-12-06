using PublisherLib;
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
            using var publisher = new Publisher(">tcp://localhost", 15000);

            var rand = new Random(50);
            while (true)
            {
                var randomizedTopic = rand.NextDouble();
                if (randomizedTopic > 0.5)
                {
                    publisher.Publish(TOPIC_A, "TopicA - hello there !");
                }
                else
                {
                    publisher.Publish(TOPIC_B, "TopicB - Bay there !");
                }

                await Task.Delay(100);
            }
        }
    }
}
