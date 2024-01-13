using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class BallPhyicsTest :MonoBehaviour, IEventReciver
{
    
    [SerializeField]private UnityEvent _event;
    [SerializeField] private string id;

    [ContextMenu("创建一个ID")]
    private void CreateID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        _event.AddListener(EventTrigger);
        UnityEventCenter.RegisterReciver(this);
    }

    private void EventTrigger()
    {
        BallFly().Forget();
    }
    private async UniTaskVoid BallFly()
    {
        await UniTask.WaitForSeconds(4f);
        GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }
    
    public UnityEvent TriggerEvent { get=>_event; }
    public string EventID { get=>id; }
}
