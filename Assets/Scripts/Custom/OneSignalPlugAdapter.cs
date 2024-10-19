using Newtonsoft.Json.Linq;
using OneSignalSDK;

public abstract class OneSignalPlugAdapter
{
    public static string UserOSIdentificator => OneSignal.Default?.User?.OneSignalId;

    public static void OsInitialize()
    {
        OneSignal.Initialize("78ab4ca5-ede1-428a-ac93-b7cd4709ca89");
    }

    public static void UnSub()
    {
        OneSignal.Notifications?.ClearAllNotifications();
        OneSignal.Logout();
    }
}
