using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ZigSteadyDetector : MonoBehaviour {
	public float maxVariance = 50.0f;
	public float timedBufferSize = 0.5f;
	public float minSteadyTime = 0.1f;
	TimedBuffer<Vector3> points;
	
	// these really should be { get; private set; }
	// but this way they're visible in the inspector
	public bool IsSteady;
    public float Variance;
    public Vector3 steadyPoint;

    public event EventHandler Steady;
    public List<GameObject> listeners = new List<GameObject>();

    void notifyListeners(string msgname, object arg)
    {
        SendMessage(msgname, arg, SendMessageOptions.DontRequireReceiver);
        for (int i = 0; i < listeners.Count; )
        {
            GameObject go = listeners[i];
            if (go)
            {
                go.SendMessage(msgname, arg, SendMessageOptions.DontRequireReceiver);
                i++;
            }
            else
            {
                listeners.RemoveAt(i);
            }
        }
    }

    protected virtual void OnSteady() {
        if (null != Steady) {
            Steady.Invoke(this, new EventArgs());
        }
        notifyListeners("SteadyDetector_Steady", this);
    }
	
	// Use this for initialization
	void Start () {
		points = new TimedBuffer<Vector3>(timedBufferSize);
	}
	
	Vector3 GetSingularValues()
	{
		var buffer = points.Buffer;
		if (buffer.Count < 4) {
			return Vector3.zero;
		}
		
		float[,] covarianceMatrix = new float[3, 3];
        Vector3 avg = Vector3.zero;
		foreach(var pt in buffer) {
            avg += pt.obj;
		}
        avg /= buffer.Count; // average of all the points
        foreach (var pt in buffer) {
            Vector3 relativeToCenter = pt.obj - avg;
            covarianceMatrix[0, 0] += relativeToCenter.x * relativeToCenter.x;
            covarianceMatrix[1, 1] += relativeToCenter.y * relativeToCenter.y;
            covarianceMatrix[2, 2] += relativeToCenter.z * relativeToCenter.z;
            covarianceMatrix[0, 0] += relativeToCenter.x * relativeToCenter.x;
            covarianceMatrix[0, 1] += relativeToCenter.x * relativeToCenter.y;
            covarianceMatrix[0, 2] += relativeToCenter.x * relativeToCenter.z;
        }
        covarianceMatrix[2, 0] = covarianceMatrix[0, 2]; //symmetry makes these assignments right
        covarianceMatrix[1, 2] = covarianceMatrix[1, 0] = covarianceMatrix[2, 1] = covarianceMatrix[0, 1];

        return getEigenvalues(new Matrix3(covarianceMatrix));

	}
    // Reference : Oliver K. Smith: Eigenvalues of a symmetric 3 × 3 matrix. Commun. ACM 4(4): 168 (1961) 
	// find the eigenvalues of a 3x3 symmetric matrix
    //NOTE: I'm doing some post-processing on them to turn them into singular values
	Vector3 getEigenvalues(Matrix3 mat) {
		var m = (mat.trace()) / 3;
		var K = mat - (Matrix3.I()*m); // K = mat - I*tr(mat)
		var q = K.determinant() / 2;
		var tempForm = K*K;
	 
		var p = tempForm.sumCells() / 6;
	 
		// NB in Smith's paper he uses phi = (1/3)*arctan(sqrt(p*p*p - q*q)/q), which is equivalent to below:
		var phi = (1/3)*Mathf.Acos(q/Mathf.Sqrt(p*p*p));
	 
		if (Mathf.Abs(q) >= Mathf.Abs(Mathf.Sqrt(p*p*p))) {
			phi = 0;
		}
	 
		if (phi < 0) {
			phi = phi + Mathf.PI/3;
		}
	 
		var eig1 = m + 2*Mathf.Sqrt(p)*Mathf.Cos(phi);
		var eig2 = m - Mathf.Sqrt(p)*(Mathf.Cos(phi) + Mathf.Sqrt(3)*Mathf.Sin(phi));
		var eig3 = m - Mathf.Sqrt(p)*(Mathf.Cos(phi) - Mathf.Sqrt(3)*Mathf.Sin(phi));
	    // return a singular values vector
		return new Vector3(Mathf.Sqrt(Mathf.Abs(eig1)),
                           Mathf.Sqrt(Mathf.Abs(eig2)),
                           Mathf.Sqrt(Mathf.Abs(eig3)));
	}
	
	public void Clear()
	{
		StopCoroutine("WaitForSteady");
		points.Clear();	
	}
	
	IEnumerator WaitForSteady()
	{
		yield return new WaitForSeconds(minSteadyTime);
        steadyPoint = points.Buffer[points.Buffer.Count - 1].obj;
        OnSteady();
	}
    void Session_Start(Vector3 focusPoint) {
        Clear();
        ProcessPoint(focusPoint);
    }

    void Session_Update(Vector3 handPoint) {
        ProcessPoint(handPoint);
    }

    void Session_End() {
        Clear();
    }

	void ProcessPoint(Vector3 position)
	{
		// add current point
        points.AddDataPoint(position);
        Variance = GetSingularValues().x;
        bool currentFrameSteady = Variance < maxVariance;
        //WebplayerLogger.Log(Variance.ToString());
		if (!IsSteady && currentFrameSteady) {
			StartCoroutine("WaitForSteady");
		}
		if (IsSteady && !currentFrameSteady) {
			StopCoroutine("WaitForSteady");
		}
		IsSteady = currentFrameSteady;
	}

}
