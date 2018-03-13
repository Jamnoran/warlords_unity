using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Sct : MonoBehaviour {

    public Animator _animator;
    private Text _damageText;

    void Start()
    {
        //Get clipinfo
        AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        //Destroy when clip is over
        Destroy(this.gameObject, clipInfo[0].clip.length);
        //Get the textobject
        _damageText = _animator.GetComponent<Text>();
        
    }

    public void SetText(string text)
    {
        _animator.GetComponent<Text>().text = text;
    }
}
