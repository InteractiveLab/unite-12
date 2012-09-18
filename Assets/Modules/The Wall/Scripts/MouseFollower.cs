using UnityEngine;
using System.Collections;

public class MouseFollower : MonoBehaviour {

    public HitTarget Target;

    private float startZ;

	void Start () {
	    startZ = Target.transform.position.z;
	}
	
	void Update () {
	    Target.Move(new Vector2(1 - Input.mousePosition.x / Screen.width*2, 1 - Input.mousePosition.y / Screen.height*2));
        if (Input.GetMouseButton(0)) {
            Target.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, 0);
        } else {
            Target.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, startZ);
        }
	}
}
