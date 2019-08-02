using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuPrincipalScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {      
	  ScoreScript.puntos = 0;
    }
      

    public void StartGame()
    {

        SceneManager.LoadScene("Controles");

    }

    public void Quit()
    {
        Application.Quit();
    }
}