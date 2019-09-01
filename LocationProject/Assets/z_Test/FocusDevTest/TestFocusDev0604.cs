using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestFocusDev0604 : MonoBehaviour {

    public InputField devId;
    public InputField depId;

    public Button focusDevButton;
	// Use this for initialization
	void Start () {
        focusDevButton.onClick.AddListener(FoucsDev);

    }
	
	private void FoucsDev()
    {
        int dev =int.Parse(devId.text);
        int dep = int.Parse(depId.text);
        RoomFactory.Instance.FocusDev(dev.ToString(), dep);
    }
}
