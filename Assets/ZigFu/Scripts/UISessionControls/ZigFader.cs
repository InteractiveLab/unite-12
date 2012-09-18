using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ZigFader : MonoBehaviour {
	public Vector3 direction = Vector3.right;
	public float size = 200;
    public float initialValue = 0.5f;
    public int itemCount = 1;
    public float hysteresis = 0.2f;
    public bool AutoMoveToContain = false;
    public float driftAmount = 0.0f;
    public bool Visualize = false;

    // these should be private set but this way they're visible in the inspector
    public float value; // { get; private set; }
    public int hoverItem = -1; // { get; private set; }

    Vector3 start;
    bool isEdge;
    float lastUpdate;

    public List<GameObject> listeners = new List<GameObject>();

    public event EventHandler Edge;

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

    protected void OnEdge() {
        notifyListeners("Fader_Edge", this);
        if (null != Edge) {
            Edge.Invoke(this, new EventArgs());
        }
    }

    void Start() {
        if (0 == itemCount) {
            itemCount = 1;
        }
        
        value = initialValue;
    }

	// move the slider to contain pos within its bounds
	public void MoveToContain(Vector3 pos) {
		float dot = Vector3.Dot(direction, pos - start);
		if (dot > size) {
            start += direction * (dot - size);
		}
		if (dot < 0) {
            start += direction * dot;
		}
	}
	
	// move the slider so that pos will be mapped to val (0-1)
	public void MoveTo(Vector3 pos, float val) {
        start = pos - (direction * (val * size));
	}
	
	public void UpdatePosition(Vector3 pos) {
        if (AutoMoveToContain) {
            MoveToContain(pos);
        }

        float dot = Vector3.Dot(direction, pos - start);
        start += (pos - start) - (dot * direction);

        UpdateValue(GetValue(pos));

        if (driftAmount > 0.0f) {
            float dt = Time.time - lastUpdate;
            lastUpdate = Time.time;
            float delta = initialValue - value;
            MoveTo(pos, value + (delta * driftAmount * dt));
        }
	}
	
	public float GetValue(Vector3 pos) {
		float dot = Vector3.Dot(direction, pos - start);
        float val = Mathf.Clamp01(dot / size);
        return val;
    }

    public void UpdateValue(float val) {
        this.value = val;
    
        // value change
        notifyListeners("Fader_ValueChange", this);

        // edge
        bool isEdgeThisFrame = Mathf.Approximately(val, 0) || Mathf.Approximately(val, 1.0f);
        if (!isEdge && isEdgeThisFrame) {
            OnEdge();
        }
        isEdge = isEdgeThisFrame;

        // item hover
        int newHover = hoverItem;
        float minValue = (hoverItem * (1.0f / itemCount)) - hysteresis;
        float maxValue = (hoverItem + 1.0f) * (1.0f / itemCount) + hysteresis;

        if (val > maxValue)
        {
            newHover++;
        }
        if (val < minValue)
        {
            newHover--;
        }
        if (newHover < 0)
            newHover = -1;
        if (newHover >= itemCount)
            newHover = itemCount - 1;

        if (newHover != hoverItem)
        {
            if (hoverItem != -1) notifyListeners("Fader_HoverStop", this);
            hoverItem = newHover;
            notifyListeners("Fader_HoverStart", this);
        }
    }
	
	public Vector3 GetPosition(float val) {
        return start + (direction * (val * size));
	}

    // hand point session messages
	
    void Session_Start(Vector3 focusPosition)
    {
        MoveTo(focusPosition, initialValue);
        value = initialValue;
    }

    void Session_Update(Vector3 handPosition)
    {
        UpdatePosition(handPosition);
    }

    void Session_End()
    {
        value = initialValue;
    }

    void OnGUI() {
        if (Visualize) {
            GUILayout.BeginVertical("box");

            GUILayout.Label("Fader " + gameObject.name);
            GUILayout.HorizontalSlider(value, 0, 1);

            GUILayout.EndVertical();
        }
    }
}
