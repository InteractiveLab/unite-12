using UnityEngine;
using System;

public class PunchController : MonoBehaviour {
    public float HorizontalAngle = 120;
    public float VerticalAngle = 90;

    public float ExplosionMinLength = 6;
    public float ExplosionMinVerticalDelta = 0.2f;
    public float ExplosionMinVertical = 1.1f;

    public HitTarget LeftHitTarget;
    public HitTarget RightHitTarget;
    public Wall WallController;
    public KinectController KinectController;

    public EventHandler OnExplosion;

    private bool activated = true;

    public void Activate() {
        if (activated) return;

        activated = true;
        LeftHitTarget.gameObject.SetActive(true);
        RightHitTarget.gameObject.SetActive(true);
        LeftHitTarget.Move(new Vector2(-2, .5f));
        RightHitTarget.Move(new Vector2(2, .5f));
    }

    public void Deactivate() {
        if (!activated) return;

        activated = false;
        LeftHitTarget.gameObject.SetActive(false);
        RightHitTarget.gameObject.SetActive(false);
    }

	void Start () {
        Deactivate();
	}
	
	void Update () {
	    if (activated) {
            if (KinectController.ActiveSkeleton != null) {
                SkeletonController.HandCoords left = KinectController.ActiveSkeleton.NormalizedCoords(Hand.Left, HorizontalAngle, VerticalAngle);
                LeftHitTarget.Move(new Vector3(left.horizontal, left.vertical, left.length));
                SkeletonController.HandCoords right = KinectController.ActiveSkeleton.NormalizedCoords(Hand.Right, HorizontalAngle, VerticalAngle);
                RightHitTarget.Move(new Vector3(right.horizontal, right.vertical, right.length));
/*
                switch (explosionGestureState) {
                    case ExplosionState.Possible:
                        if (left.length > ExplosionMinLength && right.length > ExplosionMinLength) {
                            stateInPlace(left.vertical, right.vertical);
                        }
                        break;
                    case ExplosionState.InPlace:
                        if (left.length < ExplosionMinLength || right.length > ExplosionMinLength) {
                            statePossible();
                        } */
                        if (left.vertical > ExplosionMinVertical && right.vertical > ExplosionMinVertical && left.length > ExplosionMinLength && right.length > ExplosionMinLength)
                        {
                            stateExplosion();
                        }
                /*
                        break;
 
                        
                } */
            } else {
                LeftHitTarget.Move(new Vector2(-2, .5f));
                RightHitTarget.Move(new Vector2(2, .5f));
            }
	    };
	}

  /*  private void stateInPlace(float leftVertical, float rightVertical) {
        setExplosionState(ExplosionState.InPlace);
        leftHandInPlaceStartVertical = leftVertical;
        rightHandInPlaceStartVertical = rightVertical;
    }

    private void statePossible() {
        setExplosionState(ExplosionState.Possible);
    }
*/
    private void stateExplosion() {
        setExplosionState(ExplosionState.Explosion);
        if (OnExplosion != null) {
            OnExplosion(this, EventArgs.Empty);
        }
    }

    private void setExplosionState(ExplosionState s) {
        //Debug.Log(string.Format("{0} Transition from {1} to {2}", typeof(PunchController), explosionGestureState, s));
        explosionGestureState = s;
    }

    private enum ExplosionState {
        Possible,
        InPlace,
        Explosion
    }

    private ExplosionState explosionGestureState;
    private float leftHandInPlaceStartVertical, rightHandInPlaceStartVertical;
}
