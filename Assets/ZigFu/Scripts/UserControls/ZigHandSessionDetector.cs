using UnityEngine;
using System;
using System.Collections.Generic;

public class SessionStartEventArgs : EventArgs
{
    public Vector3 FocusPoint { get; private set; }
    public SessionStartEventArgs(Vector3 fp) {
        FocusPoint = fp;
    }
}

public class SessionUpdateEventArgs : EventArgs
{
    public Vector3 HandPoint { get; private set; }
    public SessionUpdateEventArgs(Vector3 hp) {
        HandPoint = hp;
    }
}

public class ZigHandSessionDetector : MonoBehaviour {
    public bool StartOnSteady = false;
    public bool StartOnWave = true;
    public bool RotateToUser = true;
    
    public List<GameObject> listeners = new List<GameObject>();

    public Vector3 SessionBoundsOffset = new Vector3(0, 250, -300);
    public Vector3 SessionBounds = new Vector3(1500, 700, 1000);

    GameObject leftHandDetector;
    GameObject rightHandDetector;
    ZigJointId jointInSession;
    Vector3 focusPoint;
    ZigTrackedUser trackedUser;
    bool InSession;
    Bounds currentSessionBounds;

    public event EventHandler<SessionStartEventArgs> SessionStart;
    public event EventHandler<SessionUpdateEventArgs> SessionUpdate;
    public event EventHandler SessionEnd;

    protected virtual void OnSessionStart(Vector3 focusPoint) {
        notifyListeners("Session_Start", focusPoint);
        if (null != SessionStart) {
            SessionStart.Invoke(this, new SessionStartEventArgs(focusPoint));
        }
    }

    protected virtual void OnSessionUpdate(Vector3 handPoint) {
        notifyListeners("Session_Update", handPoint);
        if (null != SessionUpdate) {
            SessionUpdate.Invoke(this, new SessionUpdateEventArgs(handPoint));
        }
    }

    protected virtual void OnSessionEnd() {
        notifyListeners("Session_End", null);
        if (null != SessionEnd) {
            SessionEnd.Invoke(this, new EventArgs());
        }
    }

    void Awake() {
        leftHandDetector = new GameObject("LeftHandDetector");
        leftHandDetector.transform.parent = gameObject.transform;
        ZigMapJointToSession leftMap = leftHandDetector.AddComponent<ZigMapJointToSession>();
        leftMap.joint = ZigJointId.LeftHand;

        rightHandDetector = new GameObject("RightHandDetector");
        rightHandDetector.transform.parent = gameObject.transform;
        ZigMapJointToSession rightMap = rightHandDetector.AddComponent<ZigMapJointToSession>();
        rightMap.joint = ZigJointId.RightHand;

        if (StartOnSteady) {
            ZigSteadyDetector steadyLeft = leftHandDetector.AddComponent<ZigSteadyDetector>();
            steadyLeft.Steady += delegate(object sender, EventArgs ea) {
                CheckSessionStart((sender as ZigSteadyDetector).steadyPoint, ZigJointId.LeftHand);
            };

            ZigSteadyDetector steadyRight = rightHandDetector.AddComponent<ZigSteadyDetector>();
            steadyRight.Steady += delegate(object sender, EventArgs ea) {
                CheckSessionStart((sender as ZigSteadyDetector).steadyPoint, ZigJointId.RightHand);
            };
        }

        if (StartOnWave) {
            ZigWaveDetector waveLeft = leftHandDetector.AddComponent<ZigWaveDetector>();
            waveLeft.Wave += delegate(object sender, EventArgs ea) {
                Debug.Log("Wave from left");
                CheckSessionStart((sender as ZigWaveDetector).wavePoint, ZigJointId.LeftHand);
            };

            ZigWaveDetector waveRight = rightHandDetector.AddComponent<ZigWaveDetector>();
            waveRight.Wave += delegate(object sender, EventArgs ea) {
                Debug.Log("Wave from right");
                CheckSessionStart((sender as ZigWaveDetector).wavePoint, ZigJointId.RightHand);
            };
        }
    }

    public void AddListener(GameObject listener) {
        if (listeners.Contains(listener)) return;

        listeners.Add(listener);
        if (InSession) {
            listener.SendMessage("Session_Start", focusPoint, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void RemoveListener(GameObject listener) {
        if (InSession) {
            listener.SendMessage("Session_End", SendMessageOptions.DontRequireReceiver);
        }
        listeners.Remove(listener);
    }

    void Zig_Attach(ZigTrackedUser user) {
        trackedUser = user;
        user.AddListener(leftHandDetector);
        user.AddListener(rightHandDetector);
    }

    void Zig_UpdateUser(ZigTrackedUser user) {
        if (InSession) {
            // get hand point for this frame, rotate if neccessary
            Vector3 hp = user.Skeleton[(int)jointInSession].Position;
            if (RotateToUser) hp = RotateHandPoint(hp);
            // make sure hand point is still within session bounds
            currentSessionBounds.center = (RotateToUser) ? RotateHandPoint(trackedUser.Position) : trackedUser.Position;
            currentSessionBounds.center += SessionBoundsOffset;
            if (!currentSessionBounds.Contains(hp)) {
                EndSession();
                return;
            }
            OnSessionUpdate(hp);
        }
    }

    void Zig_Detach(ZigTrackedUser user) {
        user.RemoveListener(leftHandDetector);
        user.RemoveListener(rightHandDetector);
        EndSession();
        trackedUser = null;
    }

    public void EndSession()
    {
        if (InSession) {
            InSession = false;
            OnSessionEnd();
            
        }
    }

    void CheckSessionStart(Vector3 point, ZigJointId joint) {
        if (InSession) { Debug.Log("CheckSessionStart when already in session, leaving"); return; }

        Vector3 boundsCenter = (RotateToUser) ? RotateHandPoint(trackedUser.Position) : trackedUser.Position;
        boundsCenter += SessionBoundsOffset;
        currentSessionBounds = new Bounds(boundsCenter, SessionBounds);
        Vector3 fp = (RotateToUser) ? RotateHandPoint(point) : point;
        if (currentSessionBounds.Contains(fp)) {
            focusPoint = fp;
            jointInSession = joint;
            InSession = true;
            OnSessionStart(fp);
        }
    }

    void notifyListeners(string msgname, object arg) {
        SendMessage(msgname, arg, SendMessageOptions.DontRequireReceiver);
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

    Vector3 RotateHandPoint(Vector3 handPoint)
    {
        //TODO: Smoothing on CoM (so sudden CoM changes won't mess with the hand
        //      point too much)
        Vector3 rotateTarget = trackedUser.Position.normalized;

        // use line between com and sensor as Z, decompose rotation into
        // rotations around the Y and X axes

        Vector3 firstTarget = rotateTarget;
        // project onto XZ plane
        firstTarget.y = 0;
        firstTarget = firstTarget.normalized;
        // find rotation around the X axis
        Quaternion xRotation = Quaternion.FromToRotation(rotateTarget, firstTarget);
        // rotation around Y axis
        Quaternion yRotation = Quaternion.FromToRotation(firstTarget, Vector3.forward);
        return yRotation * xRotation * handPoint;
    }

}
