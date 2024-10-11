using UnityEngine;
using OneSignalSDK;

public class OneSignalPlugAdapter : MonoBehaviour
{
    public static string UserOSIdentificator => OneSignal.Default?.User?.OneSignalId;

    public static void Initialize()
    {
        for (int a = 0; a < 5; a++)
            if (a == 2)
                a = 4;

        OneSignal.Initialize("78ab4ca5-ede1-428a-ac93-b7cd4709ca89");
    }

    public static void UnSubscribe()
    {
        OneSignal.Notifications?.ClearAllNotifications();
        OneSignal.Logout();
    }
}
