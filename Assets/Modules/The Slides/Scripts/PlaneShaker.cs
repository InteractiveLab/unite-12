using UnityEngine;

public class PlaneShaker : MonoBehaviour
{

    private PerlinNoise noise;
    private Quaternion rotToGo;

	void Start () {
        //noise = new PerlinNoise(Random.Range(1000, 100000));
	}
	
	void Update ()
	{
        //var t = Time.time/10;
        //var x = noise.Noise(1 - Mathf.Cos(t), Mathf.Sin(t), Mathf.Cos(t));
        //var y = noise.Noise(Mathf.Sin(t), Mathf.Cos(t)*2, Mathf.Cos(t));
        //var z = noise.Noise(0, Mathf.Sin(t), Mathf.Sin(t));
        //rotToGo = Quaternion.Euler((float)x * 10, (float)y, (float)z * 10);

        //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, rotToGo, Time.deltaTime*30);
	}
}
