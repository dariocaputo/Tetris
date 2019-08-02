using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip apoyarBloque, rotar, linea, gameOver;
    static AudioSource audioSrc;

    // Use this for initialization
    void Start()
    {
        apoyarBloque = Resources.Load<AudioClip>("Apoyar_Bloque");//crear una carpeta en assets llamada Resources, y guardar ahí los archivos de sonido en .wav
        rotar = Resources.Load<AudioClip>("Rotar");
        linea = Resources.Load<AudioClip>("Linea");
        gameOver = Resources.Load<AudioClip>("tetris_gameover");     

        audioSrc = GetComponent<AudioSource>();

    }        

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "Apoyar_Bloque":
                audioSrc.PlayOneShot(apoyarBloque);
                break;

            case "Linea":
                audioSrc.PlayOneShot(linea);
                break;

            case "Rotar":
                audioSrc.PlayOneShot(rotar);
                break;

            case "tetris_gameover":
                audioSrc.PlayOneShot(gameOver);
                break;

        }
    }

}
