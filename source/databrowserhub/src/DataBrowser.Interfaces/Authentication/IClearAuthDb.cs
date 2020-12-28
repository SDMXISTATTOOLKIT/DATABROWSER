namespace DataBrowser.Interfaces.Authentication
{
    public interface IClearAuthDb
    {
        bool ClearAfterNodeRemove(int nodeId);
    }
}