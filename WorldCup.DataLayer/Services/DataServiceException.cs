namespace WorldCup.DataLayer.Services;

public class DataServiceException : Exception
{
    public DataServiceException(string message) : base(message) { }
    public DataServiceException(string message, Exception innerException) : base(message, innerException) { }
}
