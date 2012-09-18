using UnityEngine;
using System.Collections;
using System;

public class SwipeController : MonoBehaviour {
    public float MinSwipeStartLength = 6, MaxSwipeStartLength = 12f;
    public float MinSwipeStartHorizontalAngle = 0, MaxSwipeStartHorizontalAngle = 20;
    public float MinSwipeStartVerticalAngle = 80, MaxSwipeStartVerticalAngle = 120;
    public float MinSwipeEndHorizontalAngle = 45;
    public float AnimationDuration = 1, EndAnimationDuration = 1;

    public Vector3 ArrowPossiblePosition, ArrowReadyPosition, ArrowEndPosition;

    public Hand hand;
    public KinectController KinectController;
    public Transform Arrow;

    public class SwipeEventArgs : EventArgs {
        public Hand hand;
        public SwipeEventArgs(Hand h)
        {
            hand = h;
        }
    };
    public EventHandler<SwipeEventArgs> OnSwipe;

    public void AnimationToEnd() {
        stateAnimationToEnd();
    }

    public void Reset() {
        statePossible();
    }

    public void Activate()
    {
        statePossible();
        this.enabled = true;
    }

    public void Deactivate()
    {
        statePossible();
        this.enabled = false;
    }

	// Use this for initialization
	void Start () {
        statePossible();
	}
	
	// Update is called once per frame
	void Update () {
        checkHand();
	}

    private void checkHand()
    {
        SkeletonController.HandCoords cs = coords(hand);
        if (cs == null)
        {
            if ((state != GestureState.AnimationToEnd || state != GestureState.End) && state != GestureState.Possible)
            {
                statePossible();
            }
            return;
        }
        if (hand == Hand.Right)
        {
            cs.horizontal = 180 - cs.horizontal;
        }
        switch (state)
        {
            case GestureState.Possible:
                if (cs.vertical > MinSwipeStartVerticalAngle && cs.vertical < MaxSwipeStartVerticalAngle && cs.horizontal > MinSwipeStartHorizontalAngle && cs.horizontal < MaxSwipeStartHorizontalAngle)
                {
                    stateAnimationToCanStart();
                }
                break;
            case GestureState.AnimationToPossible:
                if (cs.vertical > MinSwipeStartVerticalAngle && cs.vertical < MaxSwipeStartVerticalAngle && cs.horizontal > MinSwipeStartHorizontalAngle && cs.horizontal < MaxSwipeStartHorizontalAngle)
                {
                    stateAnimationToCanStart();
                }
                else
                {
                    float v1 = (Time.time - animationStartTime) / animationStartDuration;
                    animationValue = Mathf.SmoothStep(animationStartValue, 0, v1);
                    //Debug.Log(string.Format("Animation to Possible: v1={0} animationStartTime={1} animationStartDuration={2} animationStartValue={3} animationValue={4}", v1, animationStartTime, animationStartDuration, animationStartValue, animationValue));
                    Arrow.position = Vector3.Lerp(ArrowPossiblePosition, ArrowReadyPosition, animationValue);
                    Arrow.renderer.material.color = new Color(1, 1, 1, animationValue);
                    if (v1 >= 1)
                    {
                        statePossible();
                    }
                }
                break;
            case GestureState.CanStart:
                if (cs.vertical < MinSwipeStartVerticalAngle || cs.vertical > MaxSwipeStartVerticalAngle || cs.horizontal < MinSwipeStartHorizontalAngle || cs.length < MinSwipeStartLength || cs.length > MaxSwipeStartLength)
                {
                    stateAnimationToPossible();
                }
                if (cs.horizontal > MinSwipeEndHorizontalAngle)
                {
                    stateSwipe();
                }
                break;
            case GestureState.AnimationToCanStart:
                if (cs.vertical < MinSwipeStartVerticalAngle || cs.vertical > MaxSwipeStartVerticalAngle || cs.horizontal < MinSwipeStartHorizontalAngle || cs.length < MinSwipeStartLength || cs.length > MaxSwipeStartLength)
                {
                    stateAnimationToPossible();
                }
                else
                {
                    float v2 = (Time.time - animationStartTime) / animationStartDuration;
                    animationValue = Mathf.SmoothStep(animationStartValue, 1, v2);
                    Arrow.position = Vector3.Lerp(ArrowPossiblePosition, ArrowReadyPosition, animationValue);
                    Arrow.renderer.material.color = new Color(1, 1, 1, animationValue);
                    //Debug.Log(string.Format("Animation to CanStart: v1={0} animationStartTime={1} animationStartDuration={2} animationStartValue={3} animationValue={4}", v2, animationStartTime, animationStartDuration, animationStartValue, animationValue));
                    if (v2 >= 1)
                    {
                        stateCanStart();
                    }
                }
                break;
            case GestureState.AnimationToEnd:
                float v3 = (Time.time - animationStartTime) / animationStartDuration;
                animationValue = Mathf.SmoothStep(0, 1, v3);
                Arrow.position = Vector3.Lerp(ArrowReadyPosition, ArrowEndPosition, animationValue);
                Arrow.renderer.material.color = new Color(1, 1, 1, 1 - animationValue);
                if (v3 >= 1) {
                    stateEnd();
                }
                break;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            statePossible();
        }
    }

    private void stateCanStart()
    {
        setState(GestureState.CanStart);
        animationValue = 1;
        Arrow.position = ArrowReadyPosition;
        Arrow.renderer.material.color = new Color(1, 1, 1, 1);
    }

    private void stateAnimationToCanStart()
    {
        setState(GestureState.AnimationToCanStart);
        animationStartTime = Time.time;
        animationStartValue = animationValue;
        animationStartDuration = (1 - animationValue) * AnimationDuration;
        if (Mathf.Approximately(animationStartDuration, 0))
        {
            stateCanStart();
        }
    }

    private void stateAnimationToPossible()
    {
        setState(GestureState.AnimationToPossible);
        animationStartTime = Time.time;
        animationStartValue = animationValue;
        animationStartDuration = animationValue * AnimationDuration;
        if (Mathf.Approximately(animationStartDuration, 0))
        {
            statePossible();
        }
    }

    private void statePossible()
    {
        setState(GestureState.Possible);
        animationValue = 0;
        Arrow.position = ArrowPossiblePosition;
        Arrow.renderer.material.color = new Color(1, 1, 1, 0);
    }

    private void stateSwipe()
    {
        setState(GestureState.Swipe);
        if (OnSwipe != null)
        {
            OnSwipe(this, new SwipeEventArgs(hand));
        }
       // AnimationToEnd();
    }

    private void stateAnimationToEnd() {
        setState(GestureState.AnimationToEnd);
        animationStartTime = Time.time;
        animationStartValue = 0;
        animationStartDuration = EndAnimationDuration;
    }

    private void stateEnd() {
        setState(GestureState.End);
        Arrow.position = ArrowEndPosition;
        Arrow.renderer.material.color = new Color(1, 1, 1, 0);

    }

    private void setState(GestureState s)
    {
        //Debug.Log(string.Format("{0} Transition from {1} to {2}", hand, state, s));
        state = s;
    }

    private SkeletonController.HandCoords coords(Hand h)
    {
        SkeletonController.HandCoords cs = null;
        if (KinectController.ActiveSkeleton != null) {
            cs = KinectController.ActiveSkeleton.AngleCoords(h);
        }
        return cs;
    }

    private enum GestureState {
        Possible,
        AnimationToCanStart,
        AnimationToPossible,
        CanStart,
        Swipe,
        AnimationToEnd,
        End
    }
    private GestureState state;
    private float animationStartTime, animationStartValue, animationValue = 0, animationStartDuration;
}
