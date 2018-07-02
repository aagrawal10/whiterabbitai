using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;
using Contract;

namespace HostAgent {
  /// <summary>
  /// Sycs up intercepted updates to cloud DB.
  /// </summary>
  public class DataManager {
    #region instance
    private static DataManager _instance = null;

    public static DataManager GetInstance() {
      if (_instance != null) return _instance;
      _instance = new DataManager();
      return _instance;
    }
    #endregion

    private ConcurrentQueue<Update> _queue;
    const string _endpointUrl = "http://localhost:8080/refresh";
    const int _periodInMsecs = 10 * 1000;
    WcfClient _client;
    System.Timers.Timer _timer;

    public DataManager() {
      _client = new WcfClient(_endpointUrl);
      _queue = new ConcurrentQueue<Update>();
    }

    public void OnStart() {
      // TODO: This can be improved if the service was made stateful.
      // Get the entire database entries and send it via WCF.
      var db_utils = new DatabaseUtils();
      var schemas = db_utils.GetAllAppointments();
      var customers = schemas.Select(x => new Customer(x)).ToList();
      while (true) {
        IRefreshData channel = null;
        try {
          Log.WriteLog("----- OnStart: COUNT {0} -----", customers.Count());
          channel = _client.CreateChannel();
          channel.RefreshDatabase(customers);
          break;
        } catch (Exception ex){
          Log.WriteLog("Error connecting to cloud vm ", ex);
          // TODO: This can be improved using a two way WCF contract.
          Thread.Sleep(1000);
        } finally {
          if (channel != null) {
            ((ICommunicationObject)channel).Close(); }
        }
      }

      // Start a timer to Flush data periodically.
      _timer = new System.Timers.Timer(_periodInMsecs) { AutoReset = false };
      _timer.Elapsed += Flush;
      _timer.Start();
    }

    public void OnStop() {
      _client.Dispose();
    }

    /// <summary>
    /// Process an insert operation.
    /// </summary>
    /// <param name="schema"></param>
    public void ProcessInsert(Schema schema) {
      var update = new Update {
        customer = new Customer(schema),
        operation = Operation.AddOrUpdate
      };
      _queue.Enqueue(update);
    }

    /// <summary>
    /// Process an update operation.
    /// </summary>
    /// <param name="schema"></param>
    public void ProcessUpdate(Schema schema) {
      var update = new Update {
        customer = new Customer(schema),
        operation = Operation.AddOrUpdate
      };
      _queue.Enqueue(update);
    }

    /// <summary>
    /// Process delete operation.
    /// </summary>
    /// <param name="schema"></param>
    public void ProcessDelete(Schema schema) {
      var update = new Update {
        customer = new Customer(schema),
        operation = Operation.Delete
      };
      _queue.Enqueue(update);
    }

    /// <summary>
    /// Called periodically.
    /// Flushes the queue and makes WCF call to process updates to cloud.
    /// </summary>
    private void Flush(object sender, System.Timers.ElapsedEventArgs e) {
      Log.WriteLog("----- FLUSH CALLED -----");
      // Call this periodically and flush the cache here.
      var list_of_updates = new List<Update>();
      Update current_update;
      while(_queue.TryDequeue(out current_update)) {
        list_of_updates.Add(current_update);
      }

      // TODO: Handle the case when cloud vm is down here as the updates are already dequeued.
      Log.WriteLog("----- FLUSH Count {0} -----", list_of_updates.Count);
      var channel = _client.CreateChannel();
      channel.ApplyUpdates(list_of_updates);
      ((ICommunicationObject)channel).Close();

      _timer.Start();
      Log.WriteLog("----- FLUSH COMPLETED -----");
    }
  }
}
