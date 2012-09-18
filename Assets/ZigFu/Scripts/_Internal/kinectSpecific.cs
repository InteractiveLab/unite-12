using UnityEngine;
using System;
using System.Collections;
using System.Threading;
public class kinectSpecific : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Kinect specific comopnent started");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    string longWord = "20"; //-27 to 27
    public static int angle;
    static void setAngle()
    {       
        long a = (long)angle;              
        NuiWrapper.NuiCameraElevationSetAngle(a);
        Thread.Sleep(0);     
    }

    
    static int getAngle(){
        long angleOut;    
        NuiWrapper.NuiCameraElevationGetAngle(out angleOut);
        return (int)angleOut;
    }
    bool readingAngle = false;
    bool SeatedMode = false;
    bool TrackSkeletonInNearMode = false;
    bool NearMode = false;
    private Thread t;
    void OnGUI()
    {

        longWord = GUI.TextField(new Rect(10, 10, 200, 30), readingAngle ? getAngle().ToString() : longWord, 20);
        
        if (GUI.Button(new Rect(10, 40, 200, 30), "SetElevation"))
        {
            
            angle = int.Parse(longWord);
            NuiWrapper.NuiCameraElevationSetAngle(angle);
            t = new Thread(setAngle);    //attempted a Paramaterized Thread to no avail       
            t.Start();
            Thread.Sleep(0);           
         
        }

        readingAngle = GUI.Toggle(new Rect(10, 80, 200, 30), readingAngle, "Read Angle");      

      
        bool nNearMode = GUI.Toggle(new Rect(10, 160, 200, 20), NearMode, "Near Mode");
        if (nNearMode != NearMode)
        {
            NearMode = nNearMode;
            ZigInput.Instance.SetNearMode(NearMode);
        }
        bool nSeatedMode = GUI.Toggle(new Rect(10, 190, 200, 20), SeatedMode, "Seated Mode");
        bool nTrackSkeletonInNearMode = GUI.Toggle(new Rect(10, 220, 200, 20), TrackSkeletonInNearMode, "Track Skeleton In NearMode");
        if ((nSeatedMode != SeatedMode) || (TrackSkeletonInNearMode != nTrackSkeletonInNearMode))
        {
            SeatedMode = nSeatedMode;
            TrackSkeletonInNearMode = nTrackSkeletonInNearMode;
            ZigInput.Instance.SetSkeletonTrackingSettings(SeatedMode, TrackSkeletonInNearMode);
        }
        
    }
}
