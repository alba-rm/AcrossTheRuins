using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambioPersonaje : MonoBehaviour
{
   public List<GameObject> characters; 
    public float followDistance = 2f; 
    public float distanceBehind = 1.5f; 
    private int currentIndex = 0; 
    private CharacterController currentPlayerController; 

    void Start()
    {
        if (characters.Count > 0)
        {
            currentPlayerController = characters[currentIndex].GetComponent<CharacterController>();
            currentPlayerController.enabled = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeCharacter();
        }
    }

    void ChangeCharacter()
    {
        currentPlayerController.enabled = false;

        currentIndex = (currentIndex + 1) % characters.Count;
        currentPlayerController = characters[currentIndex].GetComponent<CharacterController>();
        currentPlayerController.enabled = true;
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = characters[currentIndex].transform.position;
       
        Vector3 behindDirection = -characters[currentIndex].transform.forward;

        foreach (GameObject character in characters)
        {
            if (character != characters[currentIndex])
            {
                
                CharacterController controller = character.GetComponent<CharacterController>();
                if (controller.enabled)
                {
                    Vector3 targetBehindPosition = targetPosition + behindDirection * distanceBehind;
                    controller.Move((targetBehindPosition - character.transform.position) * Time.fixedDeltaTime);
                }
            }
        }
    }
}
