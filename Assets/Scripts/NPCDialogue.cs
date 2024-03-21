using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogues;
    private int dialogueIndex = 0; 
    private bool inRange = false; 
    public GameObject dialoguePanel;
    public Text dialogueText;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue()
    {
        if (dialogueIndex < dialogues.Length)
        {
            dialoguePanel.SetActive(true); 
            dialogueText.text = dialogues[dialogueIndex];
            dialogueIndex++;
        }
        else
        {
            EndDialogue(); 
        }
    }

    private void EndDialogue()
    {
        dialogueIndex = 0; 
        dialoguePanel.SetActive(false);
        Debug.Log("End of dialogue");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.P))
        {
            StartDialogue();
        }
    }
}