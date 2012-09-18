using System;
using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour
{
    public float DoublePressDelay = 1f;
    public float ExplodedStateNonInteractiveTime = 10f;
    public float ExplodedWallLiveTime = 10f;
    public float ChangeSlideAnimationDuration = 1;

    public PunchController Punch;
    public SwipeController LeftSwipe, RightSwipe;
    public Wall WallController;
    public TexturesController Textures;
    public Background BackgroundController;

    //public AVProWindowsMediaMovie MoviePlayer1;
    //public AVProWindowsMediaMovie MoviePlayer2;

    public WallCamera WallCameraController;

    public Transform slides;

    //private AVProWindowsMediaMovie currentPlayer;
    //private AVProWindowsMediaMovie prevPlayer;

    private float lastPressTime = 0;

    private void Start()
    {
        //currentPlayer = MoviePlayer1;
        //prevPlayer = MoviePlayer2;

        stateIntroSlide();
        foreach (AnimationState ass in slides.animation)
        {
            ass.normalizedSpeed = 0;
            ass.normalizedTime = 0;
        }
        setCurrentSlidesTextures(Textures.GetSlides(0));
        setNextSlidesTextures(Textures.GetSlides(0));
        Punch.OnExplosion += OnExplosion;
        LeftSwipe.OnSwipe += OnSwipe;
        RightSwipe.OnSwipe += OnSwipe;
        LeftSwipe.Deactivate();
        RightSwipe.Deactivate();
    }

    private void OnGUI()
    {
        if (state == State.IntroSlide)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Textures.Intro);
        }
    }

    private void OnSwipe(object sender, SwipeController.SwipeEventArgs swipeEventArgs)
    {
        if (state == State.Slide)
        {
            if (swipeEventArgs.hand == Hand.Right)
            {
                nextSlide();
            }
            else
            {
                prevSlide();
            }
        }
    }

    private void OnExplosion(object sender, EventArgs eventArgs)
    {
        if (state == State.BreakableSlide)
        {
            stateExplosion();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.PageDown))
        {
            if (Time.time - lastPressTime < DoublePressDelay) return;
            lastPressTime = Time.time;
            switch (state)
            {
                case State.BreakableSlide:
                    stateExplosion();
                    break;
                default:
                    nextSlide();
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.PageUp))
        {
            if (Time.time - lastPressTime < DoublePressDelay) return;
            lastPressTime = Time.time;
            switch (state)
            {
                case State.BreakableSlide:
                    stateExplosion();
                    break;
                default:
                    prevSlide();
                    break;
            }
        } else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //stateAnimationToSlide(7);
        }

        else if (Input.GetKeyDown(KeyCode.Greater))
        {
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            Application.LoadLevel("Tracking");
        }
        else if (Input.GetKeyUp(KeyCode.F1))
        {
            var fps = GetComponent<HUDFPS>();
            if (fps != null)
            {
                fps.enabled = !fps.enabled;
            }
        }


        switch (state)
        {
            case State.AnimationToSlide:
                float v1 = (Time.time - startAnimationTime)/ChangeSlideAnimationDuration;
                foreach (AnimationState ass in slides.animation)
                {
                    ass.normalizedTime = Mathf.Clamp01(v1);
                    //print(ass.normalizedTime);
                }
                if (v1 >= 1)
                {
                    stateSlide(animationSlide);
                }
                break;
            case State.Slide:
                if (!string.IsNullOrEmpty(Textures.GetSlides(currentSlide).videoPath))
                {
                    //slides.FindChild("slides/Slot_A/Layer_0_2/Placeholder_0_2/Plane_8").renderer.material.mainTexture =
                    //    currentPlayer.MovieInstance.OutputTexture;
                    //if (currentPlayer.MovieInstance.PositionFrames >= currentPlayer.MovieInstance.DurationFrames - 1)
                    //{
                    //    currentPlayer.MovieInstance.Pause();
                    //    currentPlayer.MovieInstance.PositionFrames = currentPlayer.MovieInstance.DurationFrames - 1;
                    //}
                }
                break;
        }
    }

    private IEnumerator killWallAfterTime()
    {
        yield return new WaitForSeconds(ExplodedWallLiveTime);
        WallController.gameObject.SetActive(false);
        WallCameraController.gameObject.SetActive(false);
    }

    private IEnumerator startSlidesAfterExplosion()
    {
        yield return new WaitForSeconds(ExplodedStateNonInteractiveTime);
        stateSlide(0);
    }

    private void nextSlide()
    {
        switch (state)
        {
            case State.IntroSlide:
                stateBreakableSlide();
                break;
            case State.Slide:
                stateAnimationToSlide(currentSlide + 1);
                break;
            case State.AnimationToSlide:
                break;
        }
    }

    private void prevSlide()
    {
        switch (state)
        {
            case State.BreakableSlide:
                stateIntroSlide();
                break;
            case State.Slide:
                stateAnimationToSlide(currentSlide - 1);
                break;
            case State.AnimationToSlide:
                break;
        }
    }

    private void stateIntroSlide()
    {
        setState(State.IntroSlide);

        //WallController.SetTexture(Textures.Intro);
    }

    private void stateBreakableSlide()
    {
        setState(State.BreakableSlide);

        WallController.SetTexture(Textures.Intro2);
        Punch.Activate();
        BackgroundController.State = Background.BackgroundState.White;
    }

    private void stateExplosion()
    {
        if (state != State.BreakableSlide) return;

        setState(State.Explosion);

        WallController.Explode();
        Punch.Deactivate();
        BackgroundController.State = Background.BackgroundState.BlackFire;
        WallCameraController.KillEffects();
        StartCoroutine(killWallAfterTime());
        StartCoroutine(startSlidesAfterExplosion());
    }

    private void stateSlide(int value)
    {
        if (value > Textures.Slides.Length) return;
        setState(State.Slide);

        currentSlide = value;
        //print(string.Format("Current slide: {0}", currentSlide));

        

        foreach (AnimationState ass in slides.animation)
        {
            ass.normalizedTime = 0;
        }
        slides.animation.enabled = false;
        setCurrentSlidesTextures(Textures.GetSlides(currentSlide));

        if (currentSlide <= 1)
        {
            LeftSwipe.Deactivate();
        }
        else
        {
            LeftSwipe.Activate();
        }
        if (currentSlide == Textures.Slides.Length)
        {
            RightSwipe.Deactivate();
        }
        else
        {
            RightSwipe.Activate();
        }
    }

    private void stateAnimationToSlide(int nextSlide)
    {
        if (nextSlide > Textures.Slides.Length || nextSlide <= 0) return;
        setState(State.AnimationToSlide);

        slides.animation.enabled = true;

        if (nextSlide == 1)
        {
            BackgroundController.State = Background.BackgroundState.WhiteFire;
        } else
        if (nextSlide == 7)
        {
            BackgroundController.State = Background.BackgroundState.WhiteGoL;
        }
        else if (currentSlide == 7)
        {
            BackgroundController.State = Background.BackgroundState.WhiteFire;
        }

        //var p = prevPlayer;
        //prevPlayer = currentPlayer;
        //currentPlayer = p;
        //if (!string.IsNullOrEmpty(Textures.GetSlides(currentSlide).videoPath))
        //{
        //    prevPlayer.Pause();
        //}
        startAnimationTime = Time.time;
        animationSlide = nextSlide;
        ((animationSlide > currentSlide) ? RightSwipe : LeftSwipe).AnimationToEnd();
        ((animationSlide > currentSlide) ? LeftSwipe : RightSwipe).Deactivate();
        setNextSlidesTextures(Textures.GetSlides(nextSlide));

        //if (!string.IsNullOrEmpty(Textures.GetSlides(nextSlide).videoPath))
        //{
        //    slides.FindChild("slides/Slot_B/Layer_0/Placeholder_0/Plane_4").renderer.material.mainTexture =
        //        currentPlayer.MovieInstance.OutputTexture;
        //    currentPlayer.Play();
        //}
    }

    private void setCurrentSlidesTextures(Slide s)
    {
        slides.FindChild("slides/Slot_A/Layer_3_2/Placeholder_3_2/Plane_5").renderer.material.mainTexture =
            s.Textures[3];
        slides.FindChild("slides/Slot_A/Layer_2_2/Placeholder_2_2/Plane_6").renderer.material.mainTexture =
            s.Textures[2];
        slides.FindChild("slides/Slot_A/Layer_1_2/Placeholder_1_2/Plane_7").renderer.material.mainTexture =
            s.Textures[1];
        slides.FindChild("slides/Slot_A/Layer_0_2/Placeholder_0_2/Plane_8").renderer.material.mainTexture =
            s.Textures[0];

        if (s.Textures[0] == null && string.IsNullOrEmpty(s.videoPath))
        {
            slides.FindChild("slides/Slot_A/Layer_0_2/Placeholder_0_2/Plane_8").renderer.material.SetFloat(
                "_Transparent", 1);
        }
        else
        {
            slides.FindChild("slides/Slot_A/Layer_0_2/Placeholder_0_2/Plane_8").renderer.material.SetFloat(
                "_Transparent", 0);
        }

        //if (!string.IsNullOrEmpty(s.videoPath)) {
        //    currentPlayer._filename = s.videoPath;
        //    currentPlayer.LoadMovie(true);
        //}
    }

    private void setNextSlidesTextures(Slide s)
    {


        slides.FindChild("slides/Slot_B/Layer_3/Placeholder_3/Plane").renderer.material.mainTexture = s.Textures[3];
        slides.FindChild("slides/Slot_B/Layer_2/Placeholder_2/Plane_2").renderer.material.mainTexture = s.Textures[2];
        slides.FindChild("slides/Slot_B/Layer_1/Placeholder_1/Plane_3").renderer.material.mainTexture = s.Textures[1];
        slides.FindChild("slides/Slot_B/Layer_0/Placeholder_0/Plane_4").renderer.material.mainTexture = s.Textures[0];

        //if (!string.IsNullOrEmpty(s.videoPath))
        //{
        //    currentPlayer._filename = s.videoPath;
        //    currentPlayer.LoadMovie(true);
        //}

        if (s.Textures[0] == null && string.IsNullOrEmpty(s.videoPath))
        {
            slides.FindChild("slides/Slot_B/Layer_0/Placeholder_0/Plane_4").renderer.material.SetFloat(
                "_Transparent", 1);
        }
        else
        {
            slides.FindChild("slides/Slot_B/Layer_0/Placeholder_0/Plane_4").renderer.material.SetFloat(
                "_Transparent", 0);
        }
    }

    private void setState(State value)
    {
        //Debug.Log(string.Format("{0}: transition from {1} to {2}", typeof (MainController), state, value));
        state = value;
    }

    private enum State
    {
        Init,
        IntroSlide,
        BreakableSlide,
        Explosion,
        Slide,
        AnimationToSlide
    }

    private State state = State.Init;
    private int currentSlide = 0;

    private float startAnimationTime;
    private int animationSlide;
}