using UnityEngine;
using System.Collections;

public class GameOfLifeRender : MonoBehaviour {
    
    public GameOfLife FireController;
    public GameOfLife GameOfLifeController;

    public float InverseColor = 0f;
    public float TextureLerp = 0f;
    public float Saturation = 1f;

    private void Start() {}

    private void Update() {
        renderer.material.mainTexture = FireController.GameTexture;
        renderer.material.SetTexture("_SecondTex", GameOfLifeController.GameTexture);
        renderer.material.SetFloat("_Lerp", TextureLerp);
        renderer.material.SetFloat("_InverseColor", InverseColor);
        renderer.material.SetFloat("_Saturation", Saturation);
    }

    //private void OnGUI() {
    //    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.width), FireController.GameTexture);
    //}
}