using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SceneText : MonoBehaviour
{
    public Font font;

    //public List<Text> AllText=new List<Text>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [ContextMenu("SetFont")]
    public void SetFont()
    {
        //foreach (var text in AllText)
        //{
        //    text.font = font;
        //}
    }

    [ContextMenu("GetAllText")]
    public void GetAllText()
    {
        //AllText.Clear();
        //var texts=GameObject.FindObjectsOfTypeAll(typeof(Text));
        //foreach (var text in texts)
        //{
        //    AllText.Add((Text)text);
        //}
    }

}
