using UnityEngine;
using System.Collections;

public enum Hand
{
    Left, Right
}

public class SkeletonController : MonoBehaviour {
    public Vector3 RotationOffset = Vector3.zero;
    public Transform PlaneObject;

    public class HandCoords {
        public float vertical, horizontal, length;
    }

    public HandCoords AngleCoords(Hand h) {
        Transform hand = h == Hand.Left ? leftHand : rightHand;
        Transform shoulder = h == Hand.Left ? leftShoulder : rightShoulder;
        Vector3 directionL = (hand.position - shoulder.position).normalized;

        float horizontal, vertical;
        if (directionL.z > 0) {
            horizontal = Mathf.Acos(-directionL.x)*Mathf.Rad2Deg;
        } else if (directionL.x > 0) {
            horizontal = 360 - Mathf.Acos(-directionL.x)*Mathf.Rad2Deg;
        } else {
            horizontal = - Mathf.Acos(-directionL.x) * Mathf.Rad2Deg;
        }

        if (directionL.z > 0)
        {
            vertical = Mathf.Acos(-directionL.y) * Mathf.Rad2Deg;
        }
        else if (directionL.y > 0)
        {
            vertical = 360 - Mathf.Acos(-directionL.y) * Mathf.Rad2Deg;
        }
        else
        {
            vertical = -Mathf.Acos(-directionL.y) * Mathf.Rad2Deg;
        }

        HandCoords coords = new HandCoords() {
                                    horizontal = horizontal,
                                    vertical = vertical,
                                    length = (hand.position - shoulder.position).magnitude
                                };
        
        return coords;
    }

    public HandCoords NormalizedCoords(Hand h, float HorizontalAngle, float VerticalAngle) {
        HandCoords coords = AngleCoords(h);
        coords.horizontal = (coords.horizontal - (90 - HorizontalAngle/2))/HorizontalAngle;
        coords.vertical = (coords.vertical - (90 - VerticalAngle / 2)) / VerticalAngle;
        return coords;
    }

    private void Start() {
        var k = FindObjectOfType(typeof (KinectController)) as KinectController;
        if (k == null) {
            print("Error! KinectController not found!");
        } else {
            k.RegisterSkeleton(this);
        }

        transform.Rotate(RotationOffset);
        leftHand = transform.FindChild("Left Hand");
        leftShoulder = transform.FindChild("Left Shoulder");
        rightHand = transform.FindChild("Right Hand");
        rightShoulder = transform.FindChild("Right Shoulder");
    }

    private void OnDestroy() {
        var k = FindObjectOfType(typeof (KinectController)) as KinectController;
        if (k == null) {
            print("Error! KinectController not found!");
        } else {
            k.RemoveSkeleton(this);
        }
    }

	void Update () {
	  //  HandCoords ac = AngleCoords(Hand.Left);
	  //  HandCoords sc = NormalizedCoords(Hand.Left, 180, 90);
	  //  Debug.Log(string.Format("Left Hand: mag={0} horizontalAngle={1} verticalAngle={2} horizontalScreen={3} verticalScreen={4}", ac.length, ac.horizontal, ac.vertical, sc.horizontal, sc.vertical));
	}

    private Transform leftShoulder, leftHand, rightShoulder, rightHand;
}
