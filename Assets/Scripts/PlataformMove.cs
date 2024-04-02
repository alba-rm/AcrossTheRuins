using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformMove : MonoBehaviour
{
    public Transform puntoInicial;
    public Transform puntoFinal;
    public float velocidad = 2.0f;
    
    private Vector3 objetivo;

    void Start()
    {
        objetivo = puntoFinal.position;
    }

    void Update()
    {
        MovePlataform();
    }

    void MovePlataform()
    {
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);

        if (transform.position == puntoFinal.position)
        {
            objetivo = puntoInicial.position;
        }
        else if (transform.position == puntoInicial.position)
        {
            objetivo = puntoFinal.position;
        }
    }
}

