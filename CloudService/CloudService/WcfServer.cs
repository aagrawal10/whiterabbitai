using System;
using System.ServiceModel;

namespace CloudService {
  public class WcfServer {
    const string EndpointUrl = "http://localhost:8080/refresh";
    ServiceHost _serviceHost;

    public void Start() {
      var baseUri = new Uri(EndpointUrl);
      _serviceHost = new ServiceHost(typeof(RefreshData), baseUri);
      _serviceHost.Open();
    }

    public void Stop() {
      _serviceHost.Close();
    }
  }
}
