using System;
using System.Collections.Generic;

namespace BlackBoxService {
  class Program {
    static void Test() {
      var db_utils = new DatabaseUtils();
      var rand = new Random();
      var uniqueIdList = new List<string>();

      // Create the table first.
      // db_utils.CreateTable();

      // Create entries in table.
      for (int i = 0; i < 10; i++) {
        var schema = new Schema {
          UniqueId = Guid.NewGuid().ToString(),
          Slot = DateTime.Now.AddHours(rand.Next(1, 100)).ToShortDateString(),
          Name = Guid.NewGuid().ToString(),
          Age = rand.Next(30, 40).ToString(),
          Email = Guid.NewGuid().ToString(),
          PhoneNumber = Guid.NewGuid().ToString()
        };

        uniqueIdList.Add(schema.UniqueId);
        db_utils.AddAppointment(schema);
      }

      // Print all entries in table.
      foreach (var entry in db_utils.GetAllAppointments()) {
        Console.WriteLine(entry.ToString());
      }

      // Get all unique Ids manually
      foreach (var uniqueId in uniqueIdList) {
        // Test Get.
        var schemaObject = db_utils.GetAppointment(uniqueId);

        // Test Update.
        schemaObject.Age = Guid.NewGuid().ToString();
        db_utils.UpdateAppointment(uniqueId, schemaObject);
      }

      // Print all entries in table.
      foreach (var entry in db_utils.GetAllAppointments()) {
        Console.WriteLine(entry.ToString());
      }

      // Delete all entries.
      foreach (var uniqueId in uniqueIdList) {
        db_utils.CancelAppointment(uniqueId);

        // Test Get.
        db_utils.GetAppointment(uniqueId);
      }

      // Print all entries in table.
      foreach (var entry in db_utils.GetAllAppointments()) {
        Console.WriteLine(entry.ToString());
      }
    }

    static void Main(string[] args) {
      Test();
    } // End main
  }
}
