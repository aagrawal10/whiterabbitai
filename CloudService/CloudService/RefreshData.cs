using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudService {
  public class RefreshData : IRefreshData {
    public void ApplyUpdates(List<Update> updates) {
      var db_utils = DatabaseUtils.GetInstance();
      foreach(var update in updates) {
        if (update.operation == Operation.AddOrUpdate) {
          if (!db_utils.AddOrUpdateAppointment(new Schema(update.customer))) {
            Log.WriteLog("ApplyUpdates: Error while adding/updating schema");
          }
        }
        else if (update.operation == Operation.Delete) {
          if (!db_utils.CancelAppointment(update.customer.UniqueId)) {
            Log.WriteLog("ApplyUpdates: Error while deleting schema");
          }
        }
      }
    }

    public void RefreshDatabase(List<Customer> customers) {
      var db_utils = DatabaseUtils.GetInstance();

      // Clean up the database.
      db_utils.CreateTable();

      // Add all entries from scratch.
      foreach (var customer in customers) {
        if (!db_utils.AddOrUpdateAppointment(new Schema(customer))) {
          Log.WriteLog("RefreshDatabase: Error while refreshing database");
        }
      }
    }
  }
}
