using UnityEngine;

public class KinectDepthGrabber : MonoBehaviour {
    public bool ShowDebug = false;
    public int MaxDepth = 10000;
    public Texture2D DepthTexture;

    private const int WIDTH = 320;
    private const int HEIGHT = 240;

    private Color32[] depthToColor;
    private Color32[] outputPixels;

    private void Awake() {
        DepthTexture = new Texture2D(WIDTH, HEIGHT);
        DepthTexture.wrapMode = TextureWrapMode.Clamp;

        depthToColor = new Color32[MaxDepth];
        outputPixels = new Color32[WIDTH*HEIGHT];
        depthToColor[0] = Color.black;
        for (int i = 1; i < MaxDepth; i++) {
            float intensity = 1.0f - (i/(float) MaxDepth);
            depthToColor[i].r = (byte) (255*intensity);
            depthToColor[i].g = (byte) (255*intensity);
            depthToColor[i].b = (byte) (255*intensity);
            depthToColor[i].a = 255;
        }

        ZigInput.Instance.AddListener(gameObject);
    }

    private void updateTexture(ZigDepth depth) {
        short[] rawDepthMap = depth.data;
        int depthIndex = 0;
        int factorX = depth.xres/WIDTH;
        int factorY = ((depth.yres/HEIGHT) - 1)*depth.xres;
        for (int y = HEIGHT - 1; y >= 0; --y, depthIndex += factorY) {
            int outputIndex = y*WIDTH;
            for (int x = 0; x < WIDTH; ++x, depthIndex += factorX, ++outputIndex) {
                outputPixels[outputIndex] = depthToColor[rawDepthMap[depthIndex]];
            }
        }
        DepthTexture.SetPixels32(outputPixels);
        DepthTexture.Apply();
    }

    private void Zig_Update(ZigInput input) {
        if (!enabled) return;
        updateTexture(ZigInput.Depth);
    }

    private void OnGUI() {
        if (ShowDebug) {
            GUI.DrawTexture(new Rect(Screen.width - WIDTH - 10, Screen.height - HEIGHT - 10, WIDTH, HEIGHT), DepthTexture);
        }
    }
}