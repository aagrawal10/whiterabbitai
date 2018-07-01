using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace CloudService {
  [DataContract]
  public enum Operation {
    [EnumMember]
    AddOrUpdate,

    [EnumMember]
    Delete
  }

  [DataContract]
  public class Customer {
    [DataMember]
    public string UniqueId;

    [DataMember]
    public string Slot;

    [DataMember]
    public string Name;

    [DataMember]
    public string Age;

    [DataMember]
    public string PhoneNumber;

    [DataMember]
    public string Email;
  }

  [DataContract]
  public class Update {
    [DataMember]
    public Operation operation;

    [DataMember]
    public Customer customer;
  }

  [ServiceContract]
  public interface IRefreshData {
    [OperationContract]
    void ApplyUpdates(List<Update> updates);

    [OperationContract]
    void RefreshDatabase(List<Customer> customers);
  }
}
