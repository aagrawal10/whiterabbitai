using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace HostAgent {
  public class DatabaseUtils {
    const string TableName = "Table1";
    const string UniqueId = "UniqueId";
    const string Slot = "Slot";
    const string Name = "Name";
    const string Age = "Age";
    const string PhoneNumber = "PhoneNumber";
    const string Email = "Email";
    const string connectionString = "Data Source=.\\SQLEXPRESS;database=TestData;trusted_connection=true;";

    /// <summary>
    /// Insert element to DB.
    /// </summary>
    /// <param name="schema">Schema object to insert.</param>
    /// <returns>True if success. False, otherwise.</returns>
    public bool AddAppointment(Schema schema) {
      try {
        if (this.GetAppointment(schema.UniqueId) != null) {
          Log.WriteLog("AddAppointment: Entry {0} already exists", schema.UniqueId);
          return false;
        }

        var query = String.Format("INSERT INTO {0} ({1},{2},{3},{4},{5},{6}) VALUES('{7}','{8}','{9}','{10}','{11}','{12}')",
          TableName, UniqueId, Slot, Name, Age, PhoneNumber, Email,
          schema.UniqueId, schema.Slot, schema.Name, schema.Age, schema.PhoneNumber, schema.Email);
        return RunNonQuery(query);
      }
      catch (Exception ex) {
        Log.WriteLog("AddAppointment: Exception while adding schema {0}, {1}", schema.ToString(), ex.ToString());
        return false;
      }
    }

    /// <summary>
    /// Update element to DB.
    /// </summary>
    public bool UpdateAppointment(string uniqueId, Schema schema) {
      if (this.GetAppointment(uniqueId) == null) {
        Log.WriteLog("UpdateAppointment: Entry {0} doesn't exist", uniqueId);
        return false;
      }

      var query = String.Format("UPDATE {0} SET {1}='{2}',{3}='{4}',{5}='{6}',{7}='{8}',{9}='{10}' WHERE {11} = '{12}'",
        TableName, Slot, schema.Slot, Name, schema.Name, Age, schema.Age, PhoneNumber, schema.PhoneNumber,
        Email, schema.Email, UniqueId, uniqueId);
      return RunNonQuery(query);
    }

    /// <summary>
    /// Remove an element from DB.
    /// </summary>
    /// <param name="uniqueId">Primary key of the element to be deleted</param>
    public bool CancelAppointment(string uniqueId) {
      if (this.GetAppointment(uniqueId) == null) {
        Log.WriteLog("CancelAppointment: Entry {0} doesn't exist", uniqueId);
        return false;
      }

      var query = String.Format("DELETE FROM {0} WHERE {1} = '{2}'", TableName, UniqueId, uniqueId);
      return RunNonQuery(query);
    }

    public Schema GetAppointment(string uniqueId) {
      var query = String.Format("SELECT {0},{1},{2},{3},{4},{5} FROM {6} WHERE {7} = '{8}'", UniqueId, Slot, Name, Age, PhoneNumber, 
        Email, TableName, UniqueId, uniqueId);

      using (var connection = new SqlConnection()) {
        connection.ConnectionString = connectionString;
        using (var command = new SqlCommand()) {
          command.Connection = connection;
          command.CommandText = query;
          command.Connection.Open();
          var reader = command.ExecuteReader();

          if (!reader.HasRows) {
            Log.WriteLog("GetAppointment: {0} not found", uniqueId);
            return null;
          }
          
          var returnValue = new List<Schema>();

          while (reader.Read()) {
            var schema = GetSchemaFromSchemaTable(reader);
            returnValue.Add(schema);
          }

          if (returnValue.Count != 1) {
            Log.WriteLog("GetAppointment: Multiple entries found for {0}", uniqueId);
            return null;
          }

          return returnValue[0];
        }
      }
    }

    public List<Schema> GetAllAppointments() {
      var query = String.Format("SELECT {0},{1},{2},{3},{4},{5} FROM {6}", UniqueId, Slot, Name, Age, PhoneNumber, Email, TableName);

      using (var connection = new SqlConnection()) {
        connection.ConnectionString = connectionString;
        using (var command = new SqlCommand()) {
          command.Connection = connection;
          command.CommandText = query;
          command.Connection.Open();
          var reader = command.ExecuteReader();
          var returnValue = new List<Schema>();

          while(reader.Read()) {
            var schema = GetSchemaFromSchemaTable(reader);
            returnValue.Add(schema);
          }

          return returnValue;
        }
      }
    }

    public void DeleteTable() {
      var query = @"DROP TABLE dbo.{0}";
      var cmd = String.Format(query, TableName);
      RunNonQuery(cmd);
    }

    public void CreateTable() {
      this.DeleteTable();
      var query = @"CREATE TABLE dbo.{0}
                      ({1} varchar(50) PRIMARY KEY NOT NULL,
                       {2} varchar(50) NOT NULL,
                       {3} varchar(50) NOT NULL,
                       {4} varchar(50) NOT NULL,
                       {5} varchar(50) NOT NULL,
                       {6} varchar(50) NOT NULL)";
      var cmd = String.Format(query, TableName, UniqueId, Slot, Name, Age, Email, PhoneNumber);
      RunNonQuery(cmd);
    }

    #region Private
    
    private Schema GetSchemaFromSchemaTable(SqlDataReader reader) {
      var schema = new Schema();
      schema.UniqueId = (string)reader[UniqueId];
      schema.Slot = (string)reader[Slot];
      schema.Name = (string)reader[Name];
      schema.Age = (string)reader[Age];
      schema.Email = (string)reader[Email];
      schema.PhoneNumber = (string)reader[PhoneNumber];
      return schema;
    }

    private bool RunNonQuery(string query) {
      try {
        using (var connection = new SqlConnection()) {
          connection.ConnectionString = connectionString;
          using (var command = new SqlCommand()) {
            command.CommandText = query;
            command.Connection = connection;
            command.Connection.Open();
            var result = command.ExecuteNonQuery();

            Log.WriteLog("{0} rows updated with query {1}", result, command.CommandText);
            return true;
          }
        }
      } catch (Exception ex) {
        Log.WriteLog("RunNonQuery: Error {0} {1}", query, ex);
        return false;
      }
    }

    #endregion
  }
}
