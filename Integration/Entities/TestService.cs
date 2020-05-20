namespace Integration
{
    public class TestService : ITestService
    {

        int ITestService.somee
        {
            get;
            set;
        }


        string ITestService.DoubleMessage(string message)
        {
            return message + message;
        }
    }
}