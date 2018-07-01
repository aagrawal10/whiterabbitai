using System;
using System.Diagnostics;

namespace HostAgent {
  public class CustomTraceListener : TraceListener {
    public override void Write(string message) {
      Log.WriteLog(message);
    }

    public override void WriteLine(string message) {
      Log.WriteLog(message);
    }
  }
}
