using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ServerCalculatorEx
{
    public interface IServerComponents
    {
        void Step();
        void Run();
        byte[] Receive(ref EndPoint sender);
        void Send(byte[] data, EndPoint sender);
    }
}
