using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class WallCollider : MonoBehaviour {

    private List<Transform> hitTargets = new List<Transform>();

	void Start () {
	
	}
	
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision) {
        if (hitTargets.Contains(collision.transform)) return;

        var fragment = collision.gameObject.GetComponent<WallFragment>();
        if (fragment == null) return;

        hitTargets.Add(collision.transform);
        fragment.Collide();
    }

}
