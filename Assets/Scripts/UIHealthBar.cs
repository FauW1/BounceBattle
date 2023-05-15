using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Image mask;
    public Text healthNumbers;
    float originalSize;

    void Start()
    {
        originalSize = mask.rectTransform.rect.width; //get size of mask
    }

    public void SetValue(float value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value); //change size of mask depending on float value (0 to 1)
    }

    public void SetText(string value)
    {
        healthNumbers.text = value;
    }
}
//https://learn.unity.com/tutorial/visual-styling-ui-head-up-display?uv=2019.2&projectId=5c6166dbedbc2a0021b1bc7c#5d6559afedbc2a0020986e55
