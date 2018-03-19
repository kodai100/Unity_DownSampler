using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Klak.Spout;

public class DebugMenu : SingletonMonoBehaviour<DebugMenu>
{

    public bool menuEnable = false;
    public List<GameObject> disableOnMenu;
    public List<GameObject> enableOnMenu;

    [SerializeField] SpoutReceiver receiver;

    protected GUIUtil.Folds folds = new GUIUtil.Folds();
    protected string objName = "";

    protected virtual void Start()
    {

        folds.Add("Spout", () =>
        {
            receiver.DebugMenuGUI();
        });

        folds.Add("Sampler", () =>
        {
            ReceiveTextureGenerator.Instance.DebugMenuGUI();
        });
        
        folds.Add("OSC", () =>
        {
            OSCManager.Instance.DebugMenuGUI();
        });
        
    }
    
    protected virtual void Update()
    {
        if (Input.GetKeyUp(KeyCode.D))
        {
            menuEnable = !menuEnable;
            if (!menuEnable)
            {
                PrefsGUI.Prefs.Save();
            }

            disableOnMenu.ForEach(o => o.SetActive(!menuEnable));
            enableOnMenu.ForEach(o => o.SetActive(menuEnable));
        }
    }
    
    protected Rect _windowRect = new Rect(20f, 20f, 700f, 500f);
    protected virtual void OnGUI()
    {
        if (menuEnable)
        {
            if (disableOnMenu.Any())
            {
                GUILayout.Label("<color=red>Disable On Menu: " + string.Join(",", disableOnMenu.Select(obj => obj.name).ToArray()) + "</color>");
            }
            if(enableOnMenu.Any())
            {
                GUILayout.Label("<color=green>Enable On Menu: " + string.Join(",", enableOnMenu.Select(obj => obj.name).ToArray()) + "</color>");
            }

            _windowRect = GUILayout.Window(0, _windowRect, (i) =>
            {
                folds.OnGUI();
                GUI.DragWindow();
            },"DebugMenu");
        }
    }
}
