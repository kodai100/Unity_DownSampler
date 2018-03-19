using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrefsGUI;
using Klak.Spout;

public class ReceiveTextureGenerator : SingletonMonoBehaviour<ReceiveTextureGenerator> {

    [SerializeField] SpoutReceiver receiver;
    [SerializeField] Sampler sampler;

    PrefsVector2 _textureDimension = new PrefsVector2("Texture Dimension", new Vector2(4, 2));
    RenderTexture _texture = null;

    public RenderTexture GetTexture { get { return _texture; } }

    public void DebugMenuGUI()
    {
        _textureDimension.OnGUI();
        sampler.DebugMenuGUI();

        if (GUILayout.Button("Recreate"))
        {
            RecreateTexture(ref _texture, (int)_textureDimension.Get().x, (int)_textureDimension.Get().y);

            receiver.targetTexture = _texture;
            sampler.targetRenderTexture = _texture;
        }
    }
    

	void Start () {

        RecreateTexture(ref _texture, (int)_textureDimension.Get().x, (int)_textureDimension.Get().y);

        receiver.targetTexture = _texture;
        sampler.targetRenderTexture = _texture;
    }
    

	void Update () {
		
	}

    
    void RecreateTexture(ref RenderTexture texture, int width, int height)
    {
        if(_texture != null)
        {
            ReleaseTexture(texture);
            texture = null;
        }

        texture = new RenderTexture(width, height, 1, RenderTextureFormat.ARGB32);
        texture.filterMode = FilterMode.Point;

    }

    void ReleaseTexture(RenderTexture texture)
    {
        texture.Release();
    }
}
