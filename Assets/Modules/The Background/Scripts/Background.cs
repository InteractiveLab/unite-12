using UnityEngine;

public class Background : MonoBehaviour {

    private delegate void UpdateFunc();

    public enum BackgroundState {
        None,
        Inactive,
        White,
        BlackFire,
        WhiteFire,
        WhiteGoL
    }

    public float WhiteToBlackTime = 1f;
    public float BlackToWhiteTime = 1f;
    public float WhiteToGoLTime = 1f;
    public float SaturationUnderSlide = 0.5f;
    public float EvolutionTime = 3f;

    public GameOfLife[] Effects;
    public GameOfLifeRender GoFRender;
    public Camera GoFCamera;
    public KinectDepthGrabber DepthGrabber;

    public Texture2D EvolutionTexture;
    public float evolutionTime;

    private BackgroundState state = BackgroundState.None;
    private SimpleTweener tweener;
    private UpdateFunc update;

    public BackgroundState State {
        get { return state; }
        set {
            //print("Change background state to " + value);
            switch (value)
            {
                case BackgroundState.Inactive:
                    foreach (var effect in Effects) {
                        effect.enabled = false;
                    }
                    DepthGrabber.enabled = false;
                    break;
                case BackgroundState.White:
                    //foreach (var effect in Effects) {
                    //    effect.enabled = true;
                    //}
                    Effects[0].enabled = true;
                    DepthGrabber.enabled = true;
                    break;
                case BackgroundState.BlackFire:
                    switch (state) {
                        case BackgroundState.White:
                            tweener = new SimpleTweener(1, 0, WhiteToBlackTime, SimpleTweener.EaseType.easeOutCubic);
                            update = updateFromWhiteToBlack;
                            break;
                    }
                    break;
                case BackgroundState.WhiteFire:
                    switch (state) {
                        case BackgroundState.BlackFire:
                            tweener = new SimpleTweener(0, 1, BlackToWhiteTime, SimpleTweener.EaseType.easeOutCubic);
                            update = updateFromBlackToWhite;
                            break;
                        case BackgroundState.WhiteGoL:
                            tweener = new SimpleTweener(1, 0, WhiteToGoLTime, SimpleTweener.EaseType.easeOutCubic);
                            update = updateFromGoLToWhite;
                            Effects[1].enabled = false;
                            break;
                    }
                    break;
                case BackgroundState.WhiteGoL:
                    tweener = new SimpleTweener(0, 1, WhiteToGoLTime, SimpleTweener.EaseType.easeOutCubic);
                    update = updateFromWhiteToGoL;
                    //evolutionTime = EvolutionTime;
                    Effects[1].enabled = true;
                    break;

            }
            state = value;
        }
    }

    private void Awake() {
        print(string.Format("{0} {1}", Screen.width, Screen.height));
        GoFRender.transform.localScale = new Vector3(-Screen.width/1000f, 1, Screen.height/1000f);
        GoFCamera.orthographicSize = Screen.height/200f;
        var screenDif = Screen.width/1280f;
        var aspectRatio = (float)Screen.height/Screen.width;
        foreach (var effect in Effects) {
            effect.Width = (int)(effect.Width * screenDif);
            effect.Height = (int)(effect.Width * aspectRatio);
        }
    }

    private void Start() {
        State = BackgroundState.Inactive;
    }

    private void Update() {
        if (update != null) update();
        switch (state)
        {
            case BackgroundState.WhiteGoL:
                //GameOfLife life = Effects[1];
                //if (evolutionTime > 0)
                //{
                //    life.ApplyTexture(EvolutionTexture);
                //} else
                //{
                //    life.ApplyTexture(null);
                //}
                //evolutionTime -= Time.deltaTime;
                break;
        }
    }

    private void updateFromWhiteToBlack() {
        if (tweener == null) return;
        var value = tweener.UpdateValue(Time.deltaTime);

        GoFRender.InverseColor = value;

        if (value <= 0) {
            update = null;
            tweener = null;
        }
    }

    private void updateFromBlackToWhite()
    {
        if (tweener == null) return;
        var value = tweener.UpdateValue(Time.deltaTime);

        GoFRender.InverseColor = value;
        GoFRender.Saturation = SaturationUnderSlide + (1 - SaturationUnderSlide)*(1-value);

        if (value >= 1)
        {
            update = null;
            tweener = null;
        }
    }

    private void updateFromWhiteToGoL()
    {
        if (tweener == null) return;
        var value = tweener.UpdateValue(Time.deltaTime);

        GoFRender.TextureLerp = value;

        if (value >= 1)
        {
            update = null;
            tweener = null;
        }
    }

    private void updateFromGoLToWhite()
    {
        if (tweener == null) return;
        var value = tweener.UpdateValue(Time.deltaTime);

        GoFRender.TextureLerp = value;

        if (value <= 0)
        {
            update = null;
            tweener = null;
        }
    }

}