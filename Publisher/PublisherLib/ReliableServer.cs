using NetMQ;
using NetMQ.Sockets;
using System;

namespace PublisherLib
{
    internal class ReliableServer : IDisposable
    {
        private const string PublishMessageCommand = "P";
        private const string HeartbeatMessage = "HB";

        private readonly string _address;

        private readonly NetMQActor _actor;
        private PublisherSocket _publisherSocket;
        private NetMQTimer _heartbeatTimer;
        private NetMQPoller _poller;

        public ReliableServer(string endpoint)
        {
            _address = endpoint;
            _actor = NetMQActor.Create(Run);
        }

        private void Run(PairSocket shim)
        {
            using (_publisherSocket = new PublisherSocket(_address))
            {
                //_publisherSocket.Bind(_address);
                //_publisherSocket.ReceiveReady += DropPublisherSubscriptions;
                shim.ReceiveReady += OnShimMessage;

                _heartbeatTimer = new NetMQTimer(TimeSpan.FromSeconds(10));
                _heartbeatTimer.Elapsed += OnHeartbeatTimerElapsed;

                // signal the actor that the shim is ready to work
                shim.SignalOK();

                _poller = new NetMQPoller { shim, _publisherSocket, _heartbeatTimer };
                _poller.Run();
            }
        }

        /*private void DropPublisherSubscriptions(object sender, NetMQSocketEventArgs e)
        {
            // just drop the subscription messages, we have to do that to Welcome message to work
            _publisherSocket.SkipMultipartMessage();
        }*/

        private void OnHeartbeatTimerElapsed(object sender, NetMQTimerEventArgs e)
        {
            _actor.SendFrame(HeartbeatMessage);
        }

        private void OnShimMessage(object sender, NetMQSocketEventArgs e)
        {
            string command = e.Socket.ReceiveFrameString();
            if (command == PublishMessageCommand)
            {
                var msg = e.Socket.ReceiveMultipartMessage();
                Console.WriteLine($"Sending: {msg.Last.ConvertToString()}");
                _publisherSocket.SendMultipartMessage(msg);
            }
            else if (command == NetMQActor.EndShimMessage)
            {
                _poller.Stop();
            }
        }

        public void Publish(NetMQMessage message)
        {
            _actor.SendMoreFrame(PublishMessageCommand).SendMultipartMessage(message);
        }

        public void Dispose()
        {
            _actor?.Dispose();
        }
    }
}
