using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class KinectController : MonoBehaviour {

    private List<SkeletonController> skeletons = new List<SkeletonController>();
    public SkeletonController ActiveSkeleton;
    public Renderer SkeletonIndicator;

    void Start () {
	
	}
	
	void Update () {
        ActiveSkeleton = findActiveSkeleton();
	    SkeletonIndicator.enabled = ActiveSkeleton != null;
	}

    public void RegisterSkeleton(SkeletonController value) {
        if (skeletons.Contains(value)) return;
        skeletons.Add(value);
        
    }

    public void RemoveSkeleton(SkeletonController value) {
        if (!skeletons.Contains(value)) return;
        skeletons.Remove(value);
    }

    private SkeletonController findActiveSkeleton() {
        float maxZ = float.MinValue;
        SkeletonController sc = null;
        int i = 0;
        foreach (var skeletonController in skeletons)
        {
            if (skeletonController.transform.position.z > maxZ)
            {
                maxZ = skeletonController.transform.position.z;
                sc = skeletonController;
            }
        }
        return sc;
    }

}
