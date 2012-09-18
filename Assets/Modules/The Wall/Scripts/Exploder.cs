using UnityEngine;
using System.Collections;

public class Exploder : MonoBehaviour {

    public Wall WallController;

	void Start () {
	
	}
	
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space)) {
	        WallController.Explode();
	    }
	}
}
