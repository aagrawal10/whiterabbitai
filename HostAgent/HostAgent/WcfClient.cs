using System.ServiceModel;

namespace HostAgent {
  public class WcfClient {
    private string _endpointUrl;
    private ChannelFactory<IRefreshData> _channelFactory;

    public WcfClient(string endpointUrl) {
      _endpointUrl = endpointUrl;
      _channelFactory = new ChannelFactory<IRefreshData>(new NetTcpBinding(), _endpointUrl);
    }

    public IRefreshData CreateChannel() {
      // TODO: Add a channel cache.
      return _channelFactory.CreateChannel();
    }

    public void Dispose() {
      _channelFactory.Close();
    }
  }
}
