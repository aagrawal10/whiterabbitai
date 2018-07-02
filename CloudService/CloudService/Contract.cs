using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;
using CloudService;

namespace Contract {
  [DataContract(Name = "Operation")]
  public enum Operation {
    [EnumMember]
    AddOrUpdate,

    [EnumMember]
    Delete
  }

  [DataContract(Name = "Customer")]
  public class Customer {
    [DataMember(Name = "UniqueId")]
    public string UniqueId;

    [DataMember(Name = "Slot")]
    public string Slot;

    [DataMember(Name = "Name")]
    public string Name;

    [DataMember(Name = "Age")]
    public string Age;

    [DataMember(Name = "PhoneNumber")]
    public string PhoneNumber;

    [DataMember(Name = "Email")]
    public string Email;

    public Customer() { } // Default constructor

    public Customer(Schema schema) {
      this.UniqueId = schema.UniqueId;
      this.Slot = schema.Slot;
      this.Name = schema.Name;
      this.Age = schema.Age;
      this.PhoneNumber = schema.PhoneNumber;
      this.Email = schema.Email;
    }
  }

  [DataContract(Name = "Update")]
  public class Update {
    [DataMember(Name = "Operation")]
    public Operation operation;

    [DataMember(Name = "Customer")]
    public Customer customer;
  }

  [ServiceContract(Name = "IRefreshData")]
  public interface IRefreshData {
    [OperationContract(Name = "ApplyUpdates")]
    void ApplyUpdates(List<Update> updates);

    [OperationContract(Name = "RefreshDatabase")]
    void RefreshDatabase(List<Customer> customers);
  }
}
