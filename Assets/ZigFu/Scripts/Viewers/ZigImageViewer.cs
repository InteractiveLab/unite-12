using UnityEngine;
using System.Collections;

public class ZigImageViewer : MonoBehaviour {
    public Renderer target;
    public ZigResolution TextureSize = ZigResolution.QQVGA_160x120;
    Texture2D texture;
    ResolutionData textureSize;

    Color32[] outputPixels;
    // Use this for initialization
    void Start()
    {
        if (target == null) {
            target = renderer;
        }
        textureSize = ResolutionData.FromZigResolution(TextureSize);
        texture = new Texture2D(textureSize.Width, textureSize.Height);
        texture.wrapMode = TextureWrapMode.Clamp;
        renderer.material.mainTexture = texture;
        outputPixels = new Color32[textureSize.Width * textureSize.Height];
        ZigInput.Instance.AddListener(gameObject);
    }

    void UpdateTexture(ZigImage image)
    {
        Color32[] rawImageMap = image.data;
        int srcIndex = 0;
        int factorX = image.xres / textureSize.Width;
        int factorY = ((image.yres / textureSize.Height) - 1) * image.xres;
        // invert Y axis while doing the update
        for (int y = textureSize.Height - 1; y >= 0; --y, srcIndex += factorY) {
            int outputIndex = y * textureSize.Width;
            for (int x = 0; x < textureSize.Width; ++x, srcIndex += factorX, ++outputIndex) {
                outputPixels[outputIndex] = rawImageMap[srcIndex];
            }
        }
        texture.SetPixels32(outputPixels);
        texture.Apply();
    }

    void Zig_Update(ZigInput input)
    {
        UpdateTexture(ZigInput.Image);
    }
}
