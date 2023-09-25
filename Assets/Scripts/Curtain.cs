using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Curtain : MonoBehaviour
{
    public static Curtain Instance;
    public TextMeshProUGUI textUI;
    public string label;
    public Animator animator;
    bool down;
    private void Awake()
    {
        Instance = this;
    }

    public void CurtainDown(string text, float length)
    {
        label = text;
        textUI.text = text;
        down = true;
        StartCoroutine("Elipses");
        animator.Play("CurtainDown");
        Invoke("CurtainUp", length);
    }

    IEnumerator Elipses()
    {
        while (down)
        {
            textUI.text = label;
            yield return new WaitForSeconds(.5f);
            textUI.text = label + ".";
            yield return new WaitForSeconds(.5f);
            textUI.text = label + "..";
            yield return new WaitForSeconds(.5f);
            textUI.text = label + "...";
            yield return new WaitForSeconds(.5f);
        }
    }

    public void CurtainUp()
    {
        StopCoroutine("Elipses");
        down = false;
        textUI.text = "Done!";
        animator.Play("CurtainUp");
    }
}
