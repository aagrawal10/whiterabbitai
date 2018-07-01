using System;
using System.Diagnostics;
using TableDependency;
using TableDependency.SqlClient;
using TableDependency.EventArgs;

namespace HostAgent {
  class Program {
    static CommandInterceptor _interceptor;

    static void OnStart() {
      // Start intercepting DB requests.
      _interceptor = new CommandInterceptor();
      _interceptor.Start();

      // Initialize DataManager.
      DataManager.GetInstance().OnStart();
    }

    static void OnStop() {
      _interceptor.Stop();
      DataManager.GetInstance().OnStop();
    }

    static void Main(string[] args) {
      OnStart();
      Console.ReadKey();
      OnStop();
    }
  }
}
