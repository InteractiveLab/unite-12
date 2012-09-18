using UnityEngine;
using System.Collections;

public class ZigUsersRadar : MonoBehaviour {
	public Vector2 RadarRealWorldDimensions = new Vector2(4000, 4000);
	public int PixelsPerMeter = 35;
    public Color boxColor = Color.white;
    GUIStyle style;
    Texture2D texture;
	void Start()
	{
        style = new GUIStyle();
        texture = new Texture2D(1, 1);
        for (int y = 0; y < texture.height; ++y)
        {
            for (int x = 0; x < texture.width; ++x)
            {

                Color color = Color.white;
                texture.SetPixel(x, y, color);
            }                      
        }
        texture.Apply();
        style.normal.background = texture;
	}
	
	void OnGUI () 
	{
		if (!ZigInput.Instance.ReaderInited) return; 
		
		int width = (int)((float)PixelsPerMeter * (RadarRealWorldDimensions.x / 1000.0f));
		int height = (int)((float)PixelsPerMeter * (RadarRealWorldDimensions.y / 1000.0f));
        
		GUI.BeginGroup (new Rect (Screen.width - width - 20, 20, width, height));
        Color oldColor = GUI.color;
        GUI.color = boxColor;
		GUI.Box(new Rect(0, 0, width, height), "Users Radar", style);
        GUI.color = oldColor;
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
            Color orig = GUI.color;
            GUI.color = (currentUser.SkeletonTracked) ? Color.blue : Color.red;
			GUI.Box(new Rect(radarPosition.x * width - 10, radarPosition.y * height - 10, 20, 20), currentUser.Id.ToString());
            GUI.color = orig;
		}
		GUI.EndGroup();
	}
}
