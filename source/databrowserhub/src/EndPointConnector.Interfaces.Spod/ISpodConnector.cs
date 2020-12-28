namespace EndPointConnector.Interfaces.Spod
{
    public interface ISpodConnector
    {
        /// <summary>
        ///     Get endpoint type
        /// </summary>
        SpodEndPointCostant.ConnectorType EndPointType { get; }
    }
}