using UnityEngine;
using System.Collections;
using System;

public class HandRaiseEventArgs : EventArgs
{
    public ZigJointId Joint { get; private set; }
    public HandRaiseEventArgs(ZigJointId joint)
    {
        Joint = joint;
    }
}


public class ZigHandRaiseDetector : MonoBehaviour {
	ZigSteadyDetector leftHandSteady;
	ZigSteadyDetector rightHandSteady;
    GameObject leftHandDetector;
    GameObject rightHandDetector;
	ZigTrackedUser trackedUser;
    public float angleThreshold = 30; // degrees

    public event EventHandler<HandRaiseEventArgs> HandRaise;
    protected void OnHandRaise(ZigJointId joint)
    {
       if (null != HandRaise) {
            HandRaise.Invoke(this, new HandRaiseEventArgs(joint));
        }
       SendMessage("HandRaiseDetector_HandRaise", joint, SendMessageOptions.DontRequireReceiver);
    }

    // Use this for initialization
	void Awake () {
        leftHandDetector = new GameObject("LeftHandDetector");
        leftHandDetector.transform.parent = gameObject.transform;
        ZigMapJointToSession leftMap = leftHandDetector.AddComponent<ZigMapJointToSession>();
        leftMap.joint = ZigJointId.LeftHand;

        rightHandDetector = new GameObject("RightHandDetector");
        rightHandDetector.transform.parent = gameObject.transform;
        ZigMapJointToSession rightMap = rightHandDetector.AddComponent<ZigMapJointToSession>();
        rightMap.joint = ZigJointId.RightHand;

		leftHandSteady = leftHandDetector.AddComponent<ZigSteadyDetector>();
		rightHandSteady = rightHandDetector.AddComponent<ZigSteadyDetector>();
        leftHandSteady.Steady += delegate(object sender, EventArgs e) {
            ZigInputJoint hand = trackedUser.Skeleton[(int)ZigJointId.LeftHand];
            ZigInputJoint elbow = trackedUser.Skeleton[(int)ZigJointId.LeftElbow];
            if (IsHandRaise(hand.Position, elbow.Position)) {
                OnHandRaise(ZigJointId.LeftHand);
            }
        };
        rightHandSteady.Steady += delegate(object sender, EventArgs e) {
            ZigInputJoint hand = trackedUser.Skeleton[(int)ZigJointId.RightHand];
            ZigInputJoint elbow = trackedUser.Skeleton[(int)ZigJointId.RightElbow];
            if (IsHandRaise(hand.Position, elbow.Position)) {
                OnHandRaise(ZigJointId.RightHand);
            }
        }; 
	}
	
    void Zig_Attach(ZigTrackedUser user)
    {
        trackedUser = user;
        user.AddListener(leftHandDetector);
        user.AddListener(rightHandDetector);
    }

    void Zig_UpdateUser(ZigTrackedUser user)
    {
        trackedUser = user;
    }

    void Zig_Detach(ZigTrackedUser user)
    {
        user.RemoveListener(leftHandDetector);
        user.RemoveListener(rightHandDetector);
        trackedUser = null;
    }

    bool IsHandRaise(Vector3 handPosition, Vector3 elbowPosition)
    {
        ZigInputJoint torso = trackedUser.Skeleton[(int)ZigJointId.Torso];
        ZigInputJoint head = trackedUser.Skeleton[(int)ZigJointId.Head];

        Vector3 armDirection = (handPosition - elbowPosition).normalized;
        Vector3 torsoDirection = (head.Position - torso.Position).normalized;
        double angle = Math.Acos(Vector3.Dot(armDirection, torsoDirection)) * 180 / Math.PI;
        return (angle < angleThreshold);
    }
}
