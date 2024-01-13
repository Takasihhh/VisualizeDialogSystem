using UnityEngine.Events;

public interface IEventReciver
{
    UnityEvent TriggerEvent { get; }
    string EventID { get; }
}
