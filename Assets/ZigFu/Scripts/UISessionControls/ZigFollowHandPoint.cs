using UnityEngine;
using System;

public class ZigFollowHandPoint : MonoBehaviour
{
	public Vector3 Scale = new Vector3(0.02f, 0.02f, -0.02f);
	public Vector3 bias;
	public float damping = 5;
    public Vector3 bounds = new Vector3(10, 10, 10);

    Vector3 focusPoint;
	Vector3 desiredPos;
	
	void Start() {
		desiredPos = transform.localPosition;
	}
	
	void Update() {
		transform.localPosition = Vector3.Lerp(transform.localPosition,  desiredPos, damping * Time.deltaTime);
	}

	void Session_Start(Vector3 focusPoint) {
        this.focusPoint = focusPoint;
	}
	
	void Session_Update(Vector3 handPoint) {
        Vector3 pos = handPoint - focusPoint;
        desiredPos = ClampVector(Vector3.Scale(pos, Scale) + bias, -0.5f * bounds, 0.5f * bounds);
	}
	
	void Session_End() {
        desiredPos = Vector3.zero;
	}

    Vector3 ClampVector(Vector3 vec, Vector3 min, Vector3 max) {
        return new Vector3(Mathf.Clamp(vec.x, min.x, max.x),
                           Mathf.Clamp(vec.y, min.y, max.y),
                           Mathf.Clamp(vec.z, min.z, max.z));
    }
}