using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrefsGUI;

public enum SampleMode
{
    Luminance, Hue, RGB, HSV
}

[System.Serializable]
public class PrefsEnum : PrefsParam<SampleMode>
{
    public PrefsEnum(string key, SampleMode defaultValue = default(SampleMode)) : base(key, defaultValue) { }
}

public class Sampler : MonoBehaviour {

    PrefsEnum _sampleMode = new PrefsEnum("Sample Mode", SampleMode.RGB);
    [SerializeField] RenderTexture _targetRenderTexture;
    private Texture2D _targetTexture;
    

    [SerializeField] PrefsFloat _oscSendWait = new PrefsFloat("OSC Blank Time (s)", 1.0f);

    private int _width;
    private int _height;
    
    public RenderTexture targetRenderTexture { get { return _targetRenderTexture; } set { _targetRenderTexture = value; } }

	void Start () {

        _width = _targetRenderTexture.width;
        _height = _targetRenderTexture.height;

        _targetTexture = new Texture2D(_width, _height, TextureFormat.ARGB32, false);

        StartCoroutine(SampleLoop());
    }
	
    public void DebugMenuGUI()
    {
        _sampleMode.OnGUI();
        _oscSendWait.OnGUI();

        
    }
    
	void Update () {

        // Recreate texture if render texture is changed in runtime.
        if (_targetRenderTexture.width != _width || _targetRenderTexture.height != _height)
        {
            _width = _targetRenderTexture.width;
            _height = _targetRenderTexture.height;
            _targetTexture = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
        }

	}


    IEnumerator SampleLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_oscSendWait.Get());

            Sample(_sampleMode);
        }
    }


    void Sample(SampleMode sampleMode)
    {
        RenderTexture.active = _targetRenderTexture;
        _targetTexture.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
        _targetTexture.Apply();
        
        float a, b, c;

        for (int row = 0; row < _height; row++)
        {
            for(int column = 0; column < _width; column++)
            {
                int id = row * _width + column;

                switch (sampleMode)
                {
                    case SampleMode.Luminance:
                        Color.RGBToHSV(_targetTexture.GetPixel(row, column), out a, out b, out c);
                        OSCHandler.Instance.SendMessageToClient(OSCManager.Instance.ServerID, "/ch" + id, c);
                        break;
                    case SampleMode.Hue:
                        Color.RGBToHSV(_targetTexture.GetPixel(row, column), out a, out b, out c);
                        OSCHandler.Instance.SendMessageToClient(OSCManager.Instance.ServerID, "/ch" + id, a);
                        break;
                    case SampleMode.RGB:
                        OSCHandler.Instance.SendMessageToClient(OSCManager.Instance.ServerID, "/ch" + id, string.Format(SerializeColor(_targetTexture.GetPixel(row, column))));
                        break;
                    case SampleMode.HSV:
                        Color.RGBToHSV(_targetTexture.GetPixel(row, column), out a, out b, out c);
                        OSCHandler.Instance.SendMessageToClient(OSCManager.Instance.ServerID, "/ch" + id, string.Format(SerializeColor(a, b, c)));
                        break;
                    default:
                        break;
                }
            }
        }

    }
    

    string SerializeColor(Color color)
    {
        return string.Format("{0}, {1}, {2}", color.r, color.g, color.b);
    }

    string SerializeColor(float h, float s, float v)
    {
        return string.Format("{0}, {1}, {2}", h, s, v);
    }


}
