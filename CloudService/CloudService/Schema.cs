using System;
using Contract;

namespace CloudService {
  /// <summary>
  /// Schema stored in the DB.
  /// </summary>
  public class Schema {
    public string UniqueId { get; set; }

    /// Time of the slot reserved.
    public string Slot { get; set; }
    
    // Name, Age, PhoneNumber and Email corresponding to the appointment.
    public string Name { get; set; }

    public string Age { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public Schema() { } // Default constructor

    public Schema(Customer customer) {
      this.UniqueId = customer.UniqueId;
      this.Slot = customer.Slot;
      this.Name = customer.Name;
      this.Age = customer.Age;
      this.PhoneNumber = customer.PhoneNumber;
      this.Email = customer.Email;
    }

    public override string ToString() {
      var value = String.Format("{0}, {1}, {2}, {3}, {4}, {5}", UniqueId, Slot, Name, Age, PhoneNumber, Email);
      return value;
    }
  }
}
