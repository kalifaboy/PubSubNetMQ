using NetMQ;
using NetMQ.Sockets;
using System;

namespace Broker
{
    internal class Broker
    {
        private readonly short _xPubPort;
        private readonly short _xSubPort;

        private XPublisherSocket _xPubSocket;
        private XSubscriberSocket _xSubSocket;
        public Broker(short xPubPort, short xSubPort)
        {
            _xPubPort = xPubPort;
            _xSubPort = xSubPort;
            if (_xPubPort == _xSubPort) throw new Exception("Pub & Sub ports should not be the same");
        }

        public void Launch()
        {
            try
            {
                using (_xPubSocket = new XPublisherSocket($"@tcp://localhost:{_xPubPort}"))
                using (_xSubSocket = new XSubscriberSocket($"@tcp://localhost:{_xSubPort}"))
                {
                    _xPubSocket.ReceiveReady += _xPubSocket_ReceiveReady;
                    _xSubSocket.ReceiveReady += _xSubSocket_ReceiveReady;
                    var proxy = new Proxy(_xSubSocket, _xPubSocket);
                    Console.WriteLine("Broker started ..");
                    proxy.Start();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
                throw;
            }

        }

        private void _xSubSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var msg = e.Socket.ReceiveMultipartMessage();
            Console.WriteLine($"Dispatching message for topic {msg.First.ConvertToString()}");
        }

        private void _xPubSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {

        }
    }
}
