using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Timers;

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
    const string _endpointUrl = "";
    const int _periodInMsecs = 10 * 1000;
    WcfClient _client;
    Timer _timer;

    public DataManager() {
      _client = new WcfClient(_endpointUrl);
      _queue = new ConcurrentQueue<Update>();
    }

    public void OnStart() {
      // Get the entire database entries and send it via WCF.
      var db_utils = new DatabaseUtils();
      var schemas = db_utils.GetAllAppointments();
      var customers = schemas.Select(x => new Customer(x)).ToList();
      var channel = _client.CreateChannel();
      channel.RefreshDatabase(customers);
      ((ICommunicationObject)channel).Close();

      // Start a timer to Flush data periodically.
      _timer = new Timer(_periodInMsecs) { AutoReset = false };
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
    private void Flush(object sender, ElapsedEventArgs e) {
      // Call this periodically and flush the cache here.
      var list_of_updates = new List<Update>();
      Update current_update;
      while(_queue.TryDequeue(out current_update)) {
        list_of_updates.Add(current_update);
      }

      var channel = _client.CreateChannel();
      channel.ApplyUpdates(list_of_updates);
      ((ICommunicationObject)channel).Close();

      _timer.Start();
    }
  }
}
