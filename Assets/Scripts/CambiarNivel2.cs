using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarNivel2 : MonoBehaviour
{

    public GameObject gameOver,pressLeftMouse,fondoTransparente;

    // Use this for initialization
    void Start()
    {
        gameOver.gameObject.SetActive(false);
        pressLeftMouse.gameObject.SetActive(false);
        fondoTransparente.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(LineasScript.lineas <= 0)
        {          
            SceneManager.LoadScene("Nivel2");

        }

        if(Tetris.finDeJuego)
        {
            gameOver.gameObject.SetActive(true);
            pressLeftMouse.gameObject.SetActive(true);
            fondoTransparente.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SceneManager.LoadScene("Menu");
            }

        }



    }
}
