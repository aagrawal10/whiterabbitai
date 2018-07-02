using System;
using System.Collections.Generic;
using System.Threading;

namespace BlackBoxService {
  class Program {
    static DatabaseUtils db_utils = new DatabaseUtils();
    static List<string> uniqueIdList = new List<string>();

    static void AddAppointment() {
      var rand = new Random();

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
    }

    static void UpdateAppointment() {
      // Get all unique Ids manually
      foreach (var uniqueId in uniqueIdList) {
        var rand = new Random();
        if (rand.Next(0, 10) > 7) continue;

        // Test Get.
        var schemaObject = db_utils.GetAppointment(uniqueId);

        // Test Update.
        schemaObject.Age = Guid.NewGuid().ToString();
        db_utils.UpdateAppointment(uniqueId, schemaObject);
      }
    }

    static void DeleteAppointment() {
      foreach (var uniqueId in uniqueIdList) {
        var rand = new Random();
        if (rand.Next(0, 10) > 7) continue;
        db_utils.CancelAppointment(uniqueId);
        uniqueIdList.Remove(uniqueId);
      }
    }

    static void Test() {
      var rand = new Random();

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
      while(true) {
        AddAppointment();
        Thread.Sleep(3000);
        UpdateAppointment();
        Thread.Sleep(3000);
        DeleteAppointment();
        Thread.Sleep(3000);
      }
    } // End main
  }
}
