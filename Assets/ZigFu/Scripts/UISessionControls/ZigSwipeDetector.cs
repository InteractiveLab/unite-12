using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ZigSwipeDetector : MonoBehaviour {
    public Vector2 size = new Vector2(300, 250);
    public ZigFader horizFader { get; private set; }
    public ZigFader vertFader { get; private set; }
    public List<GameObject> listeners = new List<GameObject>();

    void notifyListeners(string msgname, object arg)
    {
        SendMessage(msgname, arg, SendMessageOptions.DontRequireReceiver);
        for (int i = 0; i < listeners.Count; )
        {
            GameObject go = listeners[i];
            if (go)
            {
                go.SendMessage(msgname, arg, SendMessageOptions.DontRequireReceiver);
                i++;
            }
            else
            {
                listeners.RemoveAt(i);
            }
        }
    }
	// Use this for initialization
	void Awake () {
        horizFader = gameObject.AddComponent<ZigFader>();
        horizFader.direction = Vector3.right;
        horizFader.driftAmount = 15;
        horizFader.Edge += delegate {
            if (Mathf.Approximately(horizFader.value, 0)) {
                DoSwipe("Left");
            }
            else {
                DoSwipe("Right");
            }
        };

        vertFader = gameObject.AddComponent<ZigFader>();
        vertFader.direction = Vector3.up;
        vertFader.driftAmount = 10;
        vertFader.Edge += delegate {
            if (Mathf.Approximately(horizFader.value, 0)) {
                DoSwipe("Down");
            }
            else {
                DoSwipe("Up");
            }
        };
	}

    void DoSwipe(string direction) {
        notifyListeners("SwipeDetector_" + direction, this);
        notifyListeners("SwipeDetector_Swipe", direction);
    }
}
