using System;

namespace HostAgent {
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

    public override string ToString() {
      var value = String.Format("{0}, {1}, {2}, {3}, {4}, {5}", UniqueId, Slot, Name, Age, PhoneNumber, Email);
      return value;
    }
  }
}
