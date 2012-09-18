using UnityEngine;
using System.Collections;

public class TexturesController : MonoBehaviour {

    public Texture2D Intro;
    public Texture2D Intro2;
    public Slide[] Slides;
    public Texture2D EmptyTexture;

	void Start () {
	    foreach (var s in Slides) {
            for (int i = 0; i < s.Textures.Length; i++ )
            {
                if (s.Textures[i] == null)
                {
                    s.Textures[i] = EmptyTexture;
                }
            }
	    }
	}
	
	void Update () {
	
	}

    public Slide GetSlides(int n) {
        Slide s;
        if (n == 0) {
            s = new Slide();
            s.Textures = new Texture2D[Slides[0].Textures.Length];
            for (int i = 0; i < s.Textures.Length; i++) {
                s.Textures[i] = EmptyTexture;
            }
        } else {
            s = Slides[n-1];
        }
        return s;
    }
}

[System.Serializable]
public class Slide {
    public Texture2D[] Textures;
    public string videoPath;
}
