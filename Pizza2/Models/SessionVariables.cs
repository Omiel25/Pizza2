namespace Pizza2.Models
{
    public class SessionVariables
    {
        public const string SessionKeyUserPrivilages = "SessionPrivilages";

        public const string SessionSessionId = "SessionId";
    }

    public enum SessionKeyEnum
    {
        SessionKeyUserPrivilages = 0,
        SessionSessionId = 1
    }
}
