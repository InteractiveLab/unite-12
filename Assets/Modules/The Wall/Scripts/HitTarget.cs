using UnityEngine;
using System.Collections;

public class HitTarget : MonoBehaviour {

    public float HalfWidth = 1;
    public float HalfHeight = 1;
    public float MinLength = 4, MaxLength = 12;
    public float MinZ = 2.390961f, MaxZ = 0f;

	void Start () {
	
	}
	
	void Update () {
	
	}

    public void Move(Vector3 position)
    {
        float z = position.z;
        
        if (position.x < 0 || position.x > 1 || position.y < 0 || position.y > 1) {
            z = 0;
        }
        float v = Mathf.InverseLerp(MinLength, MaxLength, z);

        transform.position = new Vector3(Mathf.Lerp(-HalfWidth, HalfWidth, position.x), Mathf.Lerp(-HalfHeight, HalfHeight, position.y), Mathf.Lerp(MinZ, MaxZ, v));
        //Debug.Log(transform.name + " " + position);
    }

}
