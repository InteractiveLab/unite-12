using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZigPushDetector : MonoBehaviour {
	public float size = 150.0f;
	public float initialValue = 0.2f;
	public float driftSpeed = 0.05f;
	
	public float clickTimeFrame = 1.0f;
    public float clickMaxDistance = 100;

    public float clickPushTime { get; private set; }
	public bool IsClicked { get; private set; }
	public Vector3 ClickPosition { get; private set; }
    public float ClickProgress {
        get {
            return pushFader.value;
        }
    }

    public List<GameObject> listeners = new List<GameObject>();

    ZigFader pushFader;

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


    void Start()
	{
        pushFader = gameObject.AddComponent<ZigFader>();
        pushFader.direction = -Vector3.forward;
	}

	void Session_Start(Vector3 focusPosition) {
        pushFader.size = size;
        pushFader.initialValue = initialValue;
        pushFader.MoveTo(focusPosition, initialValue);
	}
    bool timeExpired = false;
	void Session_Update(Vector3 handPosition)
	{
		// move slider if hand is out of its bounds (that way it always feels responsive)
		pushFader.MoveToContain(handPosition);
        pushFader.UpdatePosition(handPosition);
	
		// click logic
		if (!IsClicked) {
            if (ClickProgress == 1.0f) {
				ClickPosition = handPosition;
                IsClicked = true;
				clickPushTime = Time.time;
                timeExpired = false;
                notifyListeners("PushDetector_Push", this);				
            }      
        }
        else { // clicked
           if (ClickProgress < 0.5) {
                if (IsClick(clickPushTime, ClickPosition, Time.time, handPosition)) {
                    notifyListeners("PushDetector_Click",this);                    
				}               
                notifyListeners("PushDetector_Release", this);                
                IsClicked = false;
            }
           else
           {
               if (!timeExpired && (Time.time - clickPushTime > clickTimeFrame))
               {
                   timeExpired = true;
                   Debug.Log("Push Detector HOLD");
                   notifyListeners("PushDetector_Hold", this);
               }
           }
        }
       

		// drift the slider to the initial position, if we aren't clicked
		if (!IsClicked) {
			float delta = initialValue - ClickProgress;
            pushFader.MoveTo(handPosition, ClickProgress + (delta * driftSpeed));
		}	
	}
	
	void Session_End() {
		if (IsClicked) {
            notifyListeners("PushDetector_Release",this);    
		}
	}
	
    bool IsClick(float t1, Vector3 p1, float t2, Vector3 p2) {
        Vector3 delta = (p2 - p1);
        delta.z = 0;
        return ((t2 - t1 < clickTimeFrame) && (delta.magnitude < clickMaxDistance));
    }
}
