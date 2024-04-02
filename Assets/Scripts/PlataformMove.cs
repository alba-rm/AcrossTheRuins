using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformMove : MonoBehaviour
{
   public Transform puntoInicial;
    public Transform puntoFinal;
    public float velocidad = 2.0f;

    private Vector3 direccion;
    private Vector3 objetivo;

    void Start()
    {
        CalcularDirection();
        CalcularObjetivo();
    }

    void Update()
    {
        MoverPlataforma();
    }

    void MoverPlataforma()
    {
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.deltaTime);

        if (transform.position == puntoFinal.position)
        {
            CalcularDirection();
            CalcularObjetivo();
        }
        else if (transform.position == puntoInicial.position)
        {
            CalcularDirection();
            CalcularObjetivo();
        }
    }

    void CalcularDirection()
    {
        direccion = (puntoFinal.position - puntoInicial.position).normalized;
    }

    void CalcularObjetivo()
    {
        objetivo = (transform.position == puntoInicial.position) ? puntoFinal.position : puntoInicial.position;
    }
}


