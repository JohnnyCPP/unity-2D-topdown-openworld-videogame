using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SoulsController : MonoBehaviour
{
    public static SoulsController instance;

    public TextMeshProUGUI text;

    int soulCounter = 0;
    void Start()
    {
        instance = this;
    }

    public void addSouls(int souls) {        
        soulCounter += souls;
        text.text = ""+soulCounter;
    }
}
