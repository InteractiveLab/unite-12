using UnityEngine;
using System;
using System.Collections.Generic;

public class ZigMapJointToSession : MonoBehaviour {
    public ZigJointId joint = ZigJointId.None;
    bool InSession;

    void Zig_UpdateUser(ZigTrackedUser user) {
        if (!InSession && user.SkeletonTracked && joint != ZigJointId.None) {
            InSession = true;
            SendMessage("Session_Start", user.Skeleton[(int)joint].Position, SendMessageOptions.DontRequireReceiver);
        }

        if (InSession) {
            if (!user.SkeletonTracked) {
                SendMessage("Session_End", SendMessageOptions.DontRequireReceiver);
                InSession = false;
            }
            else {
                SendMessage("Session_Update", user.Skeleton[(int)joint].Position, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void Reset() {
        InSession = false;
    }
}
