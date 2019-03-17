using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCalculatorEx
{
    class Program
    {
        static void Main(string[] args)
        {
            TransportIPv4 transport = new TransportIPv4();
            transport.Bind("192.168.1.218", 9999);

            Server server = new Server(transport);
            server.Run();
        }
    }
}
