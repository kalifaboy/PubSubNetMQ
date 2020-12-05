using NetMQ;
using NetMQ.Sockets;
using System;
using System.Threading.Tasks;

namespace Broker
{
    internal class Broker
    {
        private readonly short _xPubPort;
        private readonly short _xSubPort;

        private NetMQSocket _xPubSocket;
        private NetMQSocket _xSubSocket;
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
    }
}
