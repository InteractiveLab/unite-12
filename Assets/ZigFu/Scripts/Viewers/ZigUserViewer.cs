using UnityEngine;
using System.Collections;

public class ZigUserViewer : MonoBehaviour {
    public Renderer target;
    public ZigResolution TextureSize = ZigResolution.QQVGA_160x120;
  
    Texture2D texture;
    ResolutionData textureSize;

    public Color32 defaultColor;
    public Color32 bgColor;
    public Color32[] labelToColor;
    Color32[] outputPixels;
    
	// Use this for initialization
	void Start () {
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

    

    void UpdateTexture(ZigLabelMap labelmap)
    {
        short[] rawLabelMap = labelmap.data;
        int labelMapIndex = 0;
        int factorX = labelmap.xres / textureSize.Width;
        int factorY = ((labelmap.yres / textureSize.Height) - 1) * labelmap.xres;
        // invert Y axis while doing the update
        for (int y = textureSize.Height - 1; y >= 0; --y, labelMapIndex += factorY)
        {
            int outputIndex = y * textureSize.Width;
            for (int x = 0; x < textureSize.Width; ++x, labelMapIndex += factorX, ++outputIndex)
            {
                short label = rawLabelMap[labelMapIndex];
                outputPixels[outputIndex] = (label>0) ? ((label <= labelToColor.Length) ? labelToColor[label-1] : defaultColor) : bgColor;                
            }
        }
        texture.SetPixels32(outputPixels);
        texture.Apply();
    }

    void Zig_Update(ZigInput input)
    {
        UpdateTexture(ZigInput.LabelMap);
    }
}
