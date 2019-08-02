using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetris : MonoBehaviour
{
    public GameObject celdaPrefab;
    public GameObject[,] celdas;
    public GameObject I, O, T, J, L, S, Z;
    GameObject forma;
    public int[,] tablero;
    public int alto, ancho;
    public float tiempo;
    public float tiempoCaida;
    Vector2[] piezaActual;
    public int rdm;
    public int proximoBloque;
    public int rotacionesTotales;
    public bool rotar;
    public static bool finDeJuego;

    // Use this for initialization
    void Start()
    {
        ancho = 10;
        alto = 20;
        celdas = new GameObject[ancho, alto];
        tablero = new int[ancho, alto];
        piezaActual = new Vector2[4];
        finDeJuego = false;
        LineasScript.lineas = 40;

        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                tablero[j, i] = -1;//pone todas las celdas en negro, apenas empieza el juego             
                GameObject celda = Instantiate(celdaPrefab);
                celda.transform.position = new Vector2(celda.transform.position.x + j, celda.transform.position.y + i);
                celdas[j, i] = celda;
            }
        }

        rdm = Random.Range(0, 21);//tira randoms de 0 a 20     
        GenerarPieza(rdm);//baja la primera pieza

        proximoBloque = Random.Range(0, 21);
        MostrarSiguiente(proximoBloque);//muestra la siguiente pieza
    }

    void ColorBloque()//segun que número tenga la posición, la celda tendrá determinado color
    {
        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                if (tablero[j, i] == -1)
                    celdas[j, i].transform.GetComponent<Renderer>().material.color = Color.black;

                if (tablero[j, i] == 0)
                    celdas[j, i].transform.GetComponent<Renderer>().material.color = Color.white;

                if (tablero[j, i] == 1 && forma == T)
                    celdas[j, i].transform.GetComponent<Renderer>().material.color = new Color32(178, 147, 24, 1);
                //se le pasa por parámetro los componentes R,G,B y el último es el alpha que, si esta en 255 se ve(opaco), 0 es transparente
                //c/u toma valores del cero al 255 (o sea en total 256 colores)

                if (tablero[j, i] == 1 && forma == I)
                    celdas[j, i].transform.GetComponent<Renderer>().material.color = Color.red;

                if (tablero[j, i] == 1 && forma == O)
                    celdas[j, i].transform.GetComponent<Renderer>().material.color = Color.blue;

                if (tablero[j, i] == 1 && forma == S)
                    celdas[j, i].transform.GetComponent<Renderer>().material.color = Color.green;

                if (tablero[j, i] == 1 && forma == Z)
                    celdas[j, i].transform.GetComponent<Renderer>().material.color = Color.magenta;

                if (tablero[j, i] == 1 && forma == J)
                    celdas[j, i].transform.GetComponent<Renderer>().material.color = Color.yellow;

                if (tablero[j, i] == 1 && forma == L)
                    celdas[j, i].transform.GetComponent<Renderer>().material.color = Color.cyan;

                // if (piezaActual[0] == new Vector2(j, i))//para saber donde esta el pivot
                //  celdas[j, i].transform.GetComponent<Renderer>().material.color = new Color32(255,144,204,1);

            }
        }
    }

    bool ReposicionarPieza(Vector2[] pieza)//en caso que no pueda rotar
    {
        for (int i = 0; i < pieza.Length; i++)
        {
            if ((int)pieza[i].x >= 0 && (int)pieza[i].x < ancho && (int)pieza[i].y >= 0 && (int)pieza[i].y < alto)//si no se pasa los bordes
            {
                if (tablero[(int)pieza[i].x, (int)pieza[i].y] == 0)//Pero, si el lugar donde se debe colocar la nueva pieza rotada, esta con un bloque blanco
                {                                                 //no la rota
                    return false;
                }
            }
        }

        for (int i = 0; i < pieza.Length; i++)
        {
            if ((int)pieza[i].y < 0 || (int)pieza[i].y >= alto)//si se pasa los bordes en y, ya no puede rotar
            {
                return false;
            }
        }

        for (int i = 0; i < pieza.Length; i++)
        {
            if ((int)pieza[i].x < 0)//si se pasa los bordes en x a la izq
            {
                ActualizarPieza(pieza, 1, 0);//retorna para derecha
                return ReposicionarPieza(pieza);//Uso Recursividad,y cuando se actualiza (acomoda), se debe verificar de nuevo
            }

            if ((int)pieza[i].x >= ancho)//si se pasa los bordes en x a la der
            {
                ActualizarPieza(pieza, -1, 0);//retorna para iz
                return ReposicionarPieza(pieza);
            }
        }
        return true;

    }

    void ActualizarPieza(Vector2[] pieza, float x, float y)
    {
        for (int i = 0; i < pieza.Length; i++)
        {
            pieza[i] += new Vector2(x, y);//pieza recibe las nuevas x e y
        }

    }

    void RotarPieza()
    {
        SoundManagerScript.PlaySound("Rotar");
        float x, y;
        Vector2[] piezaAux = new Vector2[4];
        piezaAux[0] = new Vector2(piezaActual[0].x, piezaActual[0].y);//valores de x e y para ese índice
        piezaAux[1] = new Vector2(piezaActual[1].x, piezaActual[1].y);
        piezaAux[2] = new Vector2(piezaActual[2].x, piezaActual[2].y);
        piezaAux[3] = new Vector2(piezaActual[3].x, piezaActual[3].y);

        for (int i = 1; i < piezaAux.Length; i++)//a partir de uno, porque el índice 0(pivot), no cambia la posición
        {
            if (rotacionesTotales == 4)//cada vez que toco la tecla space, rota para el mismo lado
            {
                x = piezaAux[0].x + (piezaAux[i].y - piezaAux[0].y);//las nuevas x e y, luego de rotar
                y = piezaAux[0].y - (piezaAux[i].x - piezaAux[0].x);
                piezaAux[i] = new Vector2(x, y);//a ese bloque de pieza Aux le paso los nuevos x e y

            }
            else if (rotacionesTotales == 2)
            {
                if (!rotar)//si rotar es falso, rota para un lado
                {
                    x = piezaAux[0].x + (piezaAux[i].y - piezaAux[0].y);
                    y = piezaAux[0].y - (piezaAux[i].x - piezaAux[0].x);
                }
                else//si rotar es true , rota para el otro
                {
                    x = piezaAux[0].x - (piezaAux[i].y - piezaAux[0].y);
                    y = piezaAux[0].y + (piezaAux[i].x - piezaAux[0].x);
                }
                piezaAux[i] = new Vector2(x, y);
            }
        }

        rotar = !rotar;//cambia la rotación
        if (ReposicionarPieza(piezaAux)) //si puede rotar
            piezaActual = piezaAux;//la pieza actual toma los cambios de la aux

        DibujarPieza();
    }

    void GenerarPieza(int rdm)
    {
        piezaActual[0] = new Vector2(ancho / 2, alto - 3);//Ésta es la celda pivot , con sus pos 'x' e 'y', cuando aparece el bloque desde arriba                
        rotacionesTotales = 4;
        rotar = false;

        if (rdm <= 2)//T
        {
            piezaActual[1] = new Vector2(piezaActual[0].x - 1, piezaActual[0].y);
            piezaActual[2] = new Vector2(piezaActual[0].x + 1, piezaActual[0].y);
            piezaActual[3] = new Vector2(piezaActual[0].x, piezaActual[0].y + 1);
            forma = T;

        }
        else if (rdm >= 3 && rdm <= 5)//S
        {
            piezaActual[1] = new Vector2(piezaActual[0].x - 1, piezaActual[0].y);
            piezaActual[2] = new Vector2(piezaActual[0].x + 1, piezaActual[0].y + 1);
            piezaActual[3] = new Vector2(piezaActual[0].x, piezaActual[0].y + 1);
            rotacionesTotales = 2;
            forma = S;

        }
        else if (rdm >= 6 && rdm <= 8)//Z
        {
            piezaActual[1] = new Vector2(piezaActual[0].x + 1, piezaActual[0].y);
            piezaActual[2] = new Vector2(piezaActual[0].x - 1, piezaActual[0].y + 1);
            piezaActual[3] = new Vector2(piezaActual[0].x, piezaActual[0].y + 1);
            rotacionesTotales = 2;
            forma = Z;

        }
        else if (rdm >= 9 && rdm <= 11)//J
        {
            piezaActual[1] = new Vector2(piezaActual[0].x - 1, piezaActual[0].y + 1);
            piezaActual[2] = new Vector2(piezaActual[0].x - 1, piezaActual[0].y);
            piezaActual[3] = new Vector2(piezaActual[0].x + 1, piezaActual[0].y);
            forma = J;
        }
        else if (rdm >= 12 && rdm <= 14)//L
        {
            piezaActual[1] = new Vector2(piezaActual[0].x + 1, piezaActual[0].y + 1);
            piezaActual[2] = new Vector2(piezaActual[0].x + 1, piezaActual[0].y);
            piezaActual[3] = new Vector2(piezaActual[0].x - 1, piezaActual[0].y);
            forma = L;

        }
        else if (rdm >= 15 && rdm <= 18)//cuadrado
        {
            piezaActual[1] = new Vector2(piezaActual[0].x + 1, piezaActual[0].y);
            piezaActual[2] = new Vector2(piezaActual[0].x, piezaActual[0].y + 1);
            piezaActual[3] = new Vector2(piezaActual[0].x + 1, piezaActual[0].y + 1);
            rotacionesTotales = 0;//no rota
            forma = O;
        }
        else//I (la que menos sale)
        {
            piezaActual[1] = new Vector2(piezaActual[0].x + 1, piezaActual[0].y);
            piezaActual[2] = new Vector2(piezaActual[0].x + 2, piezaActual[0].y);
            piezaActual[3] = new Vector2(piezaActual[0].x - 1, piezaActual[0].y);
            rotacionesTotales = 2;
            forma = I;
        }

        DibujarPieza();
    }

    void DibujarPieza()
    {
        int[,] tableroAux;

        for (int i = 0; i < alto; i++)//para que cuando rote no queden mal dibujados los bloques
        {
            for (int j = 0; j < ancho; j++)
            {
                if (tablero[j, i] == 1)//sino al rotar, quedan los bloques que estaban en la anterior posición
                    tablero[j, i] = -1;
            }
        }

        tableroAux = tablero;

        for (int i = 0; i < piezaActual.Length; i++)
        {
            if (tableroAux[(int)piezaActual[i].x, (int)piezaActual[i].y] != 0)//si es distinto de blanco
            {
                tableroAux[(int)piezaActual[i].x, (int)piezaActual[i].y] = 1;//coloca la pieza pivot en ese lugar
            }
            else
            {
                SoundManagerScript.PlaySound("tetris_gameover");
                finDeJuego = true;//game over
                return;//si entra acá, no hace ningún cambio. La pieza pivot (o la que sea) no se dibuja             

            }
        }

        tablero = tableroAux;//si entra en el if(si se pudo colocar)
    }

    void MostrarSiguiente(int rdm)
    {
        I.gameObject.SetActive(false);
        O.gameObject.SetActive(false);
        S.gameObject.SetActive(false);
        Z.gameObject.SetActive(false);
        J.gameObject.SetActive(false);
        L.gameObject.SetActive(false);
        T.gameObject.SetActive(false);

        if (rdm <= 2)//T
        {
            T.gameObject.SetActive(true);
        }
        else if (rdm >= 3 && rdm <= 5)//S
        {
            S.gameObject.SetActive(true);

        }
        else if (rdm >= 6 && rdm <= 8)//Z
        {
            Z.gameObject.SetActive(true);

        }
        else if (rdm >= 9 && rdm <= 11)//J
        {
            J.gameObject.SetActive(true);

        }
        else if (rdm >= 12 && rdm <= 14)//L
        {
            L.gameObject.SetActive(true);

        }
        else if (rdm >= 15 && rdm <= 18)//cuadrado
        {
            O.gameObject.SetActive(true);

        }
        else
        {
            I.gameObject.SetActive(true);

        }

    }

    bool MoverBloque(int direccionX)
    {
        int i, j, inicioJ;

        bool OK = MoverBloqueOK(direccionX);

        if (direccionX == -1)
        {
            inicioJ = 0;//si se quiere mover  hacia la derecha
        }
        else //si dir es 1 (si se quiere mover  hacia izqu)
        {
            inicioJ = 9; //largo -1 (10-1) 
        }

        for (i = 0; i < alto; i++)
        {
            j = inicioJ;//j empezará en 0 o 9

            while (j >= 0 && j < ancho)//siempre caera aqui
            {
                if (tablero[j, i] == 1)//para el bloque rojo
                {
                    if (OK)//si se pudo mover
                    {
                        tablero[j, i] = -1;
                        tablero[j + direccionX, i] = 1;//la pos destino se pinta de rojo
                    }

                }
                j -= direccionX;//la j va hacia izq o der, según que direccion haya puesto     

            }

        }

        return true;
    }

    bool MoverBloqueOK(int direccionX)
    {
        int i, j, inicioJ;

        if (direccionX == -1)
        {
            inicioJ = 0;//si se quiere mover  hacia la derecha
        }
        else //si dir es 1 (si se quiere mover  hacia izqu)
        {
            inicioJ = ancho - 1; //ancho del tablero -1 ( o sea 10-1) 
        }

        for (i = 0; i < alto; i++)
        {
            j = inicioJ;//j empezará en 0 o 9

            while (j >= 0 && j < ancho)
            {
                if (tablero[j, i] == 1)//para el bloque rojo
                {
                    if ((j + direccionX) < 0 || (j + direccionX) >= ancho)//si se mueve a izq o derecha de los límites
                    {
                        return false;//si se pasa de los limites

                    }
                    else
                    {
                        if (tablero[j + direccionX, i] == 0)//si se mueve a iz o der y  hay un bloque blanco(queda trabada)
                        {
                            return false;//retorna falso  y sale de la funcion

                        }
                    }
                }
                j -= direccionX;//la j va hacia izq o der, según que direccion haya puesto         
                                //luego retornara true, significa que se pudo mover
            }
        }
        ActualizarPieza(piezaActual, direccionX, 0);
        return true;
    }

    void BajarBloque()
    {
        bool OK = BajarBloqueOK();

        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                if (tablero[j, i] == 1)
                {
                    if (OK)//si puede bajar
                    {
                        tablero[j, i] = -1;//la posicion actual cambia a gris
                        tablero[j, i - 1] = 1;//la pos destino es ahora roja                     
                    }
                    else//si no puede bajar
                    {
                        tablero[j, i] = 0;//la pieza se pone blanca                     
                    }
                }
            }
        }

        if (!OK)//una vez que encaja la pieza se genera una nueva pieza
        {
            SoundManagerScript.PlaySound("Apoyar_Bloque");
            VerificarLinea();
            GenerarPieza(proximoBloque);//llega acá y genera la pieza que antes se estaba mostrando como "siguiente"
            proximoBloque = Random.Range(0, 21);//tira otro random           
            MostrarSiguiente(proximoBloque);//muestra el próximo bloque a salir
            //cuando se apoye la pieza actual, caera nuevamente dentro de este if, y GenerarPieza spawnea la que antes se mostraba
            //como siguiente, y así sucesivamente        
        }

    }

    bool BajarBloqueOK()
    {
        for (int i = 0; i < alto; i++)
        {
            for (int j = 0; j < ancho; j++)
            {
                if (tablero[j, i] == 1)
                {
                    if (i > 0)
                    {
                        if (tablero[j, i - 1] == 0)//evita que si choca  con algo blanco abajo, se despedace
                        {
                            return false;//si lo que esta abajo es blanco no baja
                        }
                    }
                    else
                    {
                        return false;//en la fila i=0(piso) el bloque frena y se pone blanco 
                    }
                }
            }
        }
        ActualizarPieza(piezaActual, 0, -1); //la mueve para abajo
        return true;//si no ocurre nada de lo anterior, es porque la pieza pudo descender
    }

    void RebajarLineas(int linea)//esta función se ejecuta, si toda la línea está completa
    {
        SoundManagerScript.PlaySound("Linea");

        for (int i = linea; i < alto - 1; i++)//comienza desde la linea en la que estoy. Va -1 porque no puede ser la altura máxima
        {
            for (int j = 0; j < ancho; j++)
            {
                tablero[j, i] = tablero[j, i + 1];//todos los bloques de esa linea, van a tener el valor de los de la línea de arriba
            }

        }

    }

    void VerificarLinea()
    {
        bool lineaOK;
        bool salirFuncion;

        for (int i = 0; i < alto; i++)
        {
            lineaOK = true;//empieza todo como que la línea está completa
            salirFuncion = true;

            for (int j = 0; j < ancho; j++)
            {
                if (tablero[j, i] != 0)
                    lineaOK = false;//si en la línea hay algo que no es blanco, quiere decir que la línea no está completa entonces falso

                if (tablero[j, i] != -1)//si hay algo que no es gris(o sea si ve que no está vacío)
                    salirFuncion = false;//no sale de la funcion, porque debe verificar

            }

            if (salirFuncion)
                return;//sale  de la funcion(se deja de verificar la línea)

            if (lineaOK)//si la linea está completa(todos los bloques son blancos)
            {
                RebajarLineas(i);
                i--;
                ScoreScript.puntos += 100;
                LineasScript.lineas--;

            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!finDeJuego)
        {
            tiempo += Time.deltaTime;//tiempo 

            if (Input.GetKeyDown(KeyCode.Space))
                RotarPieza();

            if (Input.GetKey(KeyCode.S))//cae la pieza más rapido
                tiempo += 9 * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.A))
                MoverBloque(-1);

            if (Input.GetKeyDown(KeyCode.D))
                MoverBloque(1);

            if (tiempo > tiempoCaida)
            {
                BajarBloque();
                tiempo = 0;//tiempo se reinicia, hasta alcanzar de nuevo el tiempo que tarda en caer el bloque
            }

            ColorBloque();//actualiza los colores de las piezas en el tablero

        }

    }


}
