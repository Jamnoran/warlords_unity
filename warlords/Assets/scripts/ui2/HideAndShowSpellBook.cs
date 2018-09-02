using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideAndShowSpellBook : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ToggleSpellBook()
    {
        if (getUIWindow().IsVisible)
        {
            getUIWindow().Hide();
        }
        else
        {
            getUIWindow().Show();
        }
    }

    public bool IsVisible()
    {
        return getUIWindow().IsVisible;
    }
    public void Show()
    {
        getUIWindow().Show();
    }

    public void Hide()
    {
        getUIWindow().Hide();
    }

    UIWindow getUIWindow()
    {
        return ((UIWindow)transform.GetComponent(typeof(UIWindow)));
    }
}
