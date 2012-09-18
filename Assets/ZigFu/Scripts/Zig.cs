using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zig : MonoBehaviour {
    public ZigInputType inputType = ZigInputType.Auto;
    //public bool UpdateDepthmap = true;
    //public bool UpdateImagemap = false;
    //public bool UpdateLabelmap = false;
    //public bool AlignDepthToRGB = false;
    public ZigInputSettings settings = new ZigInputSettings();
    public List<GameObject> listeners = new List<GameObject>();
    public bool Verbose = true;

	void Awake () {
        #if UNITY_WEBPLAYER
        #if UNITY_EDITOR
        Debug.LogError("Depth camera input will not work in editor when target platform is Webplayer. Please change target platform to PC/Mac standalone.");
        return;
        #endif
        #endif

        ZigInput.InputType = inputType;
        ZigInput.Settings = settings;
        //ZigInput.UpdateDepth = UpdateDepthmap;
        //ZigInput.UpdateImage = UpdateImagemap;
        //ZigInput.UpdateLabelMap = UpdateLabelmap;
        //ZigInput.AlignDepthToRGB = AlignDepthToRGB;
        ZigInput.Instance.AddListener(gameObject);
        
	}

    void notifyListeners(string msgname, object arg) {
        //SendMessage(msgname, arg, SendMessageOptions.DontRequireReceiver);
        //Zig.cs doesn't send message to self
        for (int i = 0; i < listeners.Count; ) {
            GameObject go = listeners[i];
            if (go) {
                go.SendMessage(msgname, arg, SendMessageOptions.DontRequireReceiver);
                i++;
            }
            else {
                listeners.RemoveAt(i);
            }
        }
    }

    void Zig_UserFound(ZigTrackedUser user) {
        if (Verbose) Debug.Log("Zig: Found user  " + user.Id);
        notifyListeners("Zig_UserFound", user);
    }

    void Zig_UserLost(ZigTrackedUser user) {
        if (Verbose) Debug.Log("Zig: Lost user " + user.Id);
        notifyListeners("Zig_UserLost", user);
    }

    void Zig_Update(ZigInput zig) {
        notifyListeners("Zig_Update", zig);
    }
}
