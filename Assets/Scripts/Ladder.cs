using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TPSController elevacionJugador = other.GetComponent<TPSController>();
            if (elevacionJugador != null)
            {
                elevacionJugador.ActivarElevacion(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TPSController elevacionJugador = other.GetComponent<TPSController>();
            if (elevacionJugador != null)
            {
                elevacionJugador.ActivarElevacion(false);
            }
        }
    }
}
