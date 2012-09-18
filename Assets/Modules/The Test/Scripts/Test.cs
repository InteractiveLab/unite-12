using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
    private void Start() {}

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return)) {
            Application.LoadLevel("Main");
        }
    }
}