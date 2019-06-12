using UnityEngine;

public class Test : MonoBehaviour
{
    private DragonBonesProxy _com = null;

    private void Awake()
    {
        _com = transform.Find("fox").GetComponent<DragonBonesProxy>();
    }

    private void Start()
    {
        _com.FrameEvent += OnFrameEvent;
        _com.LoopCompleteEvent += OnLoopCompleteEvent;
        _com.CompleteEvent += OnCompleteEvent;

        print(string.Format("### RectColider {0}", _com.GetRectColider("身体")));
        print(string.Format("### HasAnimation {0}:{1}", "idle", _com.HasAnimation("idle")));

        _com.Play("newAnimation", 0, 1);
    }

    private void OnFrameEvent(string a)
    {
        print(string.Format("### OnFrameEvent {0}", a));
    }

    private void OnLoopCompleteEvent(string a)
    {
        print(string.Format("### OnLoopCompleteEvent {0}", a));
    }

    private void OnCompleteEvent(string a)
    {
        print(string.Format("### OnCompleteEvent {0}", a));
    }
}