using UnityEngine;
using System.Collections;

public class CamerasController : MonoBehaviour {
    [System.Serializable]
    public class AspectRatio
    {
        public float Width;
        public float Height;
        public Vector3 CameraPosition;
    }

    public AspectRatio[] AspectRatios;

	// Use this for initialization
	void Start () {
        float aspectRatio = ((float)Screen.width) / Screen.height;
        foreach (AspectRatio ar in AspectRatios)
        {
            if (Mathf.Abs(aspectRatio - ar.Width/ar.Height) < 0.1f)
            {
                transform.position = ar.CameraPosition;
                break;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
