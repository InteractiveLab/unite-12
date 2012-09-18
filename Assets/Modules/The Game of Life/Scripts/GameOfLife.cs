using UnityEngine;
using System.Collections;

public class GameOfLife : MonoBehaviour {
    public bool ApplyGameOfLife = true;
    public bool ApplyEdgeDetection = true;
    public int Width = 1280;
    public int Height = 800;
    public float FPS = 10;
    public float DecayFactor = 0.97f, DecayLerp = 0f;
    public float EdgeThreshold = .2f;
    public float DepthYDisplacement = 0;
    public float DepthYScew = 1;
    public Shader GameShader;
    public Shader EdgeDetectShader;
    public Texture2D SeedTexture;
    public KinectDepthGrabber DepthGrabber;

    public Texture GameTexture {
        get { return currentTexture; }
    }

    private Texture2D blackTexture;
    private RenderTexture currentTexture;
    private RenderTexture nextTexture;
    private Material gameMaterial;
    private Material edgeDetectMaterial;
    private RenderTexture seedTexture;

    private Texture2D textureToApply;

    private IEnumerator Start() {
        createTextures();
        createMaterials();

        yield return new WaitForEndOfFrame();

        SeedTexture = DepthGrabber.DepthTexture;
        SeedTexture.wrapMode = TextureWrapMode.Clamp;
        //SeedTexture.filterMode = FilterMode.Point;

        StartCoroutine(step());
    }

    public void ApplyTexture(Texture2D texture)
    {
        textureToApply = texture;
    }

    private void Update()
    {
        
    }

    private IEnumerator step() {
        while (true) {
            if (enabled) {
                render();
            }
            yield return new WaitForSeconds(1/FPS);
        }
    }

    private void render() {
        if (textureToApply != null)
        {
            //for (var i = 0; i < textureToApply.width; i++)
            //{
            //    for (var j = 0; j < textureToApply.height; j++)
            //    {
            //        if (textureToApply.GetPixel(i, j) == Color.black)
            //        {
            //            SeedTexture.SetPixel(i, j, Color.black);
            //        }
            //    }
            //}
            //SeedTexture.Apply();
            //print(string.Format("textureToApply {0} {1} seedTexture {2} {3}", textureToApply.width, textureToApply.height, seedTexture.width, seedTexture.height));
            //applyTextureMaterial.SetTexture("_NewTex", textureToApply);
            //Graphics.Blit(seedTexture, seedTexture, applyTextureMaterial);
            SeedTexture = textureToApply;
        } else
        {
            SeedTexture = DepthGrabber.DepthTexture;
        }

        if (ApplyEdgeDetection)
        {
            edgeDetectMaterial.SetFloat("_Treshold", EdgeThreshold * EdgeThreshold);
        }
        else
        {
            edgeDetectMaterial.SetFloat("_Treshold", -100);
        }
        edgeDetectMaterial.SetFloat("_YScew", DepthYScew);
        edgeDetectMaterial.SetFloat("_YDisplacement", DepthYDisplacement);
        Graphics.Blit(SeedTexture, seedTexture, edgeDetectMaterial);

        if (textureToApply == null && ApplyGameOfLife)
        {
            gameMaterial.SetFloat("_DecayFactor", DecayFactor);
            gameMaterial.SetTexture("_NewTex", seedTexture);
            Graphics.Blit(currentTexture, nextTexture, gameMaterial);
        } else {
            Graphics.Blit(seedTexture, nextTexture);
        }

        var tmp = currentTexture;
        currentTexture = nextTexture;
        nextTexture = tmp;
    }

    private void clearSeed() {
        var clearPixels = new Color32[Width*Height];
        for (int i = 0; i < clearPixels.Length; i++) {
            clearPixels[i] = Color.black;
        }
        SeedTexture.SetPixels32(clearPixels);
        SeedTexture.Apply();
    }

    private void createMaterials() {
        gameMaterial = new Material(GameShader);
        gameMaterial.SetFloat("_GridStepX", 1.0f/Width);
        gameMaterial.SetFloat("_GridStepY", 1.0f/Height);

        edgeDetectMaterial = new Material(EdgeDetectShader);
    }

    private void createTextures() {
        blackTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false, false);
        blackTexture.SetPixel(0, 0, Color.clear);
        blackTexture.Apply();

        seedTexture = createRenderTexture();
        currentTexture = createRenderTexture();
        nextTexture = createRenderTexture();
    }

    private RenderTexture createRenderTexture() {
        RenderTexture rt = new RenderTexture(Width, Height, 0, RenderTextureFormat.ARGB32);
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.useMipMap = false;
        rt.filterMode = FilterMode.Point;

        Graphics.Blit(blackTexture, rt);

        return rt;
    }
}