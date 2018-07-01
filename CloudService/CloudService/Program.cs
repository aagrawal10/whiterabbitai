using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudService {
  class Program {
    static WcfServer _server;

    static void OnStart() {
      _server = new WcfServer();
      _server.Start();
    }

    static void OnStop() {
      _server.Stop();
    }

    static void Main(string[] args) {
      OnStart();
      Console.ReadKey();
      OnStop();
    }
  }
}
