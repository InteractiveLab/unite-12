using UnityEngine;
using System.Collections;

public class WallCamera : MonoBehaviour {

    public float EffectClearTime = 1f;

    private SimpleTweener tweener;
    private float originalSunIntensity;
    private float originalBlurAmount;
    private float originalAperture;

    public void KillEffects() {
        var shafts = GetComponent<SunShafts>();
        if (shafts != null) {
            originalSunIntensity = shafts.sunShaftIntensity;
        }
        var blur = GetComponent<MotionBlur>();
        if (blur != null) {
            originalBlurAmount = blur.blurAmount;
        }
        var dof = GetComponent<DepthOfFieldScatter>();
        if (dof != null)
        {
            originalAperture = dof.aperture;
        }

        tweener = new SimpleTweener(1, 0, EffectClearTime, SimpleTweener.EaseType.easeOutCubic);
    }

    private void Start() {}

    private void Update() {
        if (tweener != null) {
            var value = tweener.UpdateValue(Time.deltaTime);
            var shafts = GetComponent<SunShafts>();
            if (shafts != null) {
                shafts.sunShaftIntensity = originalSunIntensity*value;
            }
            var blur = GetComponent<MotionBlur>();
            if (blur != null) {
                blur.blurAmount = originalBlurAmount*value;
            }
            var dof = GetComponent<DepthOfFieldScatter>();
            if (dof != null)
            {
                dof.aperture = originalAperture * value;
            }

            if (value <= 0) {
                tweener = null;
            }
        }
    }
}