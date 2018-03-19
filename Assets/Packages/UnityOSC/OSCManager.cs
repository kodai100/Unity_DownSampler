using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using UnityEngine;
using PrefsGUI;

public class OSCManager : SingletonMonoBehaviour<OSCManager> {

    [SerializeField] PrefsBool _send = new PrefsBool("Send OSC", true);
    [SerializeField] PrefsString _ip = new PrefsString("IP", "localhost");
    [SerializeField] PrefsInt _port = new PrefsInt("Port", 3000);

    public string ServerID { get; set; }

	void Start () {
        OSCHandler.Instance.Init();

        ServerID = "MaxMSP";

        if (_ip.Get().Equals("localhost"))
        {
            OSCHandler.Instance.CreateClient(ServerID, IPAddress.Parse("127.0.0.1"), _port.Get());
        }
        else
        {
            OSCHandler.Instance.CreateClient(ServerID, IPAddress.Parse(_ip), _port.Get());
        }

        Debug.Log(string.Format("OSC Initialized. {0} IP: {1}, Port: {2}", ServerID, _ip.Get(), _port.ToString()));
        
    }

    public void DebugMenuGUI()
    {
        _send.OnGUI();
        _ip.OnGUI();
        _port.OnGUI();
    }
	
	void Update () {
		
	}
}
