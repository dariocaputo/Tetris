using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineasScript : MonoBehaviour
{
    public static int lineas;
    Text lines;

    // Use this for initialization
    void Start()
    {
        lines = GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        lines.text = "Líneas: " + lineas;
    }

}
