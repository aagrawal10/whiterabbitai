using System;
using System.Diagnostics;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using TableDependency.Enums;

namespace HostAgent {
  public class CommandInterceptor {
    const string connectionString = "Data Source=.\\SQLEXPRESS;database=TestData;trusted_connection=true;";
    const string TableName = "Table1";
    
    SqlTableDependency<Schema> _dependency;

    public void Start() {
      _dependency = new SqlTableDependency<Schema>(connectionString, tableName: TableName);

      _dependency.OnChanged += OnChange;
      _dependency.OnError += OnError;
      _dependency.OnStatusChanged += OnStatusChanged;
      _dependency.TraceLevel = TraceLevel.Verbose;
      _dependency.TraceListener = new CustomTraceListener();

      _dependency.Start();
    }

    public void Stop() {
      _dependency.Stop();
    }

    private void OnChange(object sender, RecordChangedEventArgs<Schema> args) {
      if (args == null) {
        Log.WriteLog("OnChange: args null");
        return;
      }

      if (args.ChangeType == ChangeType.None) {
        Log.WriteLog("OnChange: Change Type is null");
        return;
      } else if (args.ChangeType == ChangeType.Insert) {
        Log.WriteLog("OnChange: Insert {0}", args.Entity.ToString());
        DataManager.GetInstance().ProcessInsert(args.Entity);
      } else if (args.ChangeType == ChangeType.Update) {
        Log.WriteLog("OnChange: Update {0}", args.Entity.ToString());
        DataManager.GetInstance().ProcessUpdate(args.Entity);
      } else if (args.ChangeType == ChangeType.Delete) {
        Log.WriteLog("OnChange: Delete {0}", args.Entity.ToString());
        DataManager.GetInstance().ProcessDelete(args.Entity);
      }
    }

    private void OnStatusChanged(object sender, StatusChangedEventArgs e) {
      Log.WriteLog("OnStatusChanged called with {0}", e.ToString());
    }

    private void OnError(object sender, ErrorEventArgs e) {
      Log.WriteLog("OnError called with {0}", e.ToString());
    }
  }
}
