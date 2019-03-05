using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICoroutine : MonoBehaviour {

    public static UICoroutine instance;

    // Use this for initialization
    void Awake () {
        instance = this;
    }
}
