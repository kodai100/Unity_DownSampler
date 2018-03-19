// KlakSpout - Spout realtime video sharing plugin for Unity
// https://github.com/keijiro/KlakSpout
using UnityEngine;

namespace Klak.Spout
{
    /// Spout sender class
    [AddComponentMenu("Klak/Spout/Spout Sender")]
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class SpoutSender : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] Vector2Int _resolution = new Vector2Int(0, 0);

        [SerializeField] bool _clearAlpha = true;

        public bool clearAlpha {
            get { return _clearAlpha; }
            set { _clearAlpha = value; }
        }

        [SerializeField] bool _sendOnly = true;

        public bool sendOnly {
            get { return _sendOnly; }
            set { _sendOnly = value; }
        }

        #endregion

        #region Private members

        System.IntPtr _sender;
        Texture2D _sharedTexture;
        Material _fixupMaterial;
        Camera _camera;

        #endregion

        #region MonoBehaviour functions

        void OnEnable()
        {
            _camera = GetComponent<Camera>();
            
            if (_resolution.x == 0) 
                _resolution.x = Screen.width;
            if (_resolution.y == 0)
                _resolution.y = Screen.height;

            _sender = PluginEntry.CreateSender(name, _resolution.x, _resolution.y);
        }

        void OnDisable()
        {
            if (_sender != System.IntPtr.Zero)
            {
                PluginEntry.DestroySharedObject(_sender);
                _sender = System.IntPtr.Zero;
            }

            if (_sharedTexture != null)
            {
                if (Application.isPlaying)
                    Destroy(_sharedTexture);
                else
                    DestroyImmediate(_sharedTexture);
                _sharedTexture = null;
            }
        }

        void Update()
        {
            PluginEntry.Poll();
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Lazy initialization for the shared texture.
            if (_sharedTexture == null)
            {
                var ptr = PluginEntry.GetTexturePointer(_sender);
                if (ptr != System.IntPtr.Zero)
                {
                    _sharedTexture = Texture2D.CreateExternalTexture(
                        PluginEntry.GetTextureWidth(_sender),
                        PluginEntry.GetTextureHeight(_sender),
                        TextureFormat.ARGB32, false, false, ptr
                    );
                }
            }

            // Update the shared texture.
            if (_sharedTexture != null)
            {
                // Lazy initialization for the fix-up shader.
                if (_fixupMaterial == null)
                    _fixupMaterial = new Material(Shader.Find("Hidden/Spout/Fixup"));

                // Parameters for the fix-up shader.
                _fixupMaterial.SetFloat("_ClearAlpha", _clearAlpha ? 1 : 0);

                // Apply the fix-up shader.
                var tempRT = RenderTexture.GetTemporary(_sharedTexture.width, _sharedTexture.height);
                Graphics.Blit(source, tempRT, _fixupMaterial, 0);

                // Copy the result to the shared texture.
                Graphics.CopyTexture(tempRT, _sharedTexture);

                // Release temporaries.
                RenderTexture.ReleaseTemporary(tempRT);
            }

            if (this.sendOnly)
            {
                // Clear previous image to render GUI.
                RenderTexture prev = RenderTexture.active;
                RenderTexture.active = null;
                GL.Clear(true, true, _camera.backgroundColor);
                RenderTexture.active = prev;
            }
            else 
            {
                // Just transfer the source to the destination.
                Graphics.Blit(source, destination);
            }
        }

        #endregion
    }
}
