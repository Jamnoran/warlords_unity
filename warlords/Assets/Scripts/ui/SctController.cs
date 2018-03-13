using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SctController : MonoBehaviour {

    private static GameObject _popupTextPrefab;
    private static GameObject _canvas;

    public static void Initialize()
    {
        _canvas = GameObject.Find("Canvas");

        _popupTextPrefab = Resources.Load<GameObject>("popuptextparent");

        var foo = "";

        
    }


	public static void CreateFloatingText(string text, Transform textLocation)
    {
        Sct instance = _popupTextPrefab.GetComponent(typeof(Sct)) as Sct;
        Instantiate(_popupTextPrefab);
        instance.transform.SetParent(_canvas.transform, false);
        instance.SetText(text);

    }
}
