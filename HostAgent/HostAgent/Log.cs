using System;

namespace HostAgent {
  public class Log {
    public static void WriteLog(string format, params object[] args) {
      Console.WriteLine(String.Format(format, args));
    }
  }
}
