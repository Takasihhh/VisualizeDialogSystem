using System.Collections.Generic;
using UnityEngine;


public static class UnityEventCenter
{
    private static List<IEventReciver> _eventRecivers;

    public static void RegisterReciver(IEventReciver reciver)
    {
        _eventRecivers ??= new List<IEventReciver>();
        _eventRecivers.Add(reciver);
    }

    public static bool CallReciverByID(string eventID)
    {
        Debug.LogError("发送信息");
        foreach (var vReciver in _eventRecivers)
        {
            if (vReciver.EventID == eventID)
            {
                vReciver.TriggerEvent?.Invoke();
                return true;
            }
        }

        return false;
    }
}
