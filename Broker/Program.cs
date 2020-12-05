using System;

namespace Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var broker = new Broker(10000, 15000);
                broker.Launch();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
