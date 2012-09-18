using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public Material WallMaterial;
    public Vector3 InitialSpeedBoost = Vector3.one;
    public float MinSpeed = 1f;
    public float SpeedMul = .99f;
    public float MinAngularSpeed = 1f;
    public float AngularSpeedMul = .99f;

    public float ExplosionSpeed = 1;
    public Vector3 ExplosionRadiusVariation = Vector3.zero;
    public float ExplosionSpeedDeviation = .5f;

    public bool Gravity = false;

    private List<Transform> debris = new List<Transform>();
    private bool oldGravity = false;

    public void SetTexture(Texture value) {
        WallMaterial.mainTexture = value;
    }

    public void Explode() {
        transform.parent.FindChild("Constraints").gameObject.SetActive(false);

        var minExp = 1 - ExplosionSpeedDeviation;
        var maxExp = 1 + ExplosionSpeedDeviation;
        foreach (Transform child in transform) {
            var w = child.GetComponent<WallFragment>();
            if (w == null) return;

            w.Explode(
                Quaternion.Euler(Random.Range(-1f, 1f) * ExplosionRadiusVariation.x,
                                 Random.Range(-1f, 1f) * ExplosionRadiusVariation.y,
                                 Random.Range(-1f, 1f) * ExplosionRadiusVariation.z) 
                * Vector3.up * ExplosionSpeed * Random.Range(minExp, maxExp));
        }
    }

	void Start () {
	    foreach (Transform child in transform) {
	        debris.Add(child);
	        var c = child.gameObject.AddComponent<MeshCollider>();
	        c.convex = true;
	        var b = child.gameObject.AddComponent<Rigidbody>();
	        b.useGravity = Gravity;
	        var w = child.gameObject.AddComponent<WallFragment>();
	        w.InitialSpeedBoost = InitialSpeedBoost;
	        w.MinSpeed = MinSpeed;
	        w.SpeedMul = SpeedMul;
            if (w.renderer != null) {
                w.renderer.material = WallMaterial;
            }
	    }
	}
	
	void Update () {
	    if (Gravity != oldGravity) {
	        oldGravity = Gravity;
            foreach (Transform child in transform)
            {
                var b = child.gameObject.GetComponent<Rigidbody>();
                b.useGravity = Gravity;
            }
	    }
	}

}
