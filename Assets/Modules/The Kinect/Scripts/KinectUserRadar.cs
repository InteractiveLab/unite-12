using UnityEngine;
using System.Collections;

public class KinectUserRadar : MonoBehaviour {

    public Vector2 RadarRealWorldDimensions = new Vector2(4000, 4000);

    private GUIStyle style;

	void Start () {
	    style = new GUIStyle();
	    style.fontSize = 100;
        style.fontStyle = FontStyle.Bold;
	    style.normal.textColor = Color.green;
	}

    void OnGUI()
    {
        if (!ZigInput.Instance.ReaderInited) return;

        foreach (ZigTrackedUser currentUser in ZigInput.Instance.TrackedUsers.Values)
        {
            // normalize the center of mass to radar dimensions
            Vector3 com = currentUser.Position;
            Vector2 radarPosition = new Vector2(com.x / RadarRealWorldDimensions.x, -com.z / RadarRealWorldDimensions.y);

            // X axis: 0 in real world is actually 0.5 in radar units (middle of field of view)
            radarPosition.x += 0.5f;

            // clamp
            radarPosition.x = Mathf.Clamp(radarPosition.x, 0.0f, 1.0f);
            radarPosition.y = Mathf.Clamp(radarPosition.y, 0.0f, 1.0f);

            // draw
            style.normal.textColor = (currentUser.SkeletonTracked) ? Color.green : Color.red;
            GUI.Label(new Rect(radarPosition.x * Screen.width - 50, radarPosition.y * Screen.height - 50, 100, 100), currentUser.Id.ToString(), style);
        }
    }

}
