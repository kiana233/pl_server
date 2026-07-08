namespace PlServer.Network;

public interface IConnectionRegistry
{
    void Register(ClientConnectionContext context);

    bool Remove(string connectionId);

    ClientConnectionContext? Get(string connectionId);

    IReadOnlyList<ClientConnectionContext> GetAll();
}
