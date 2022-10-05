using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueSpeaker dialogueSpeaker;
     private void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.CompareTag("Player"))
        {
            DialogueController.instance.StartConversation(dialogueSpeaker);

        }    
    }
}
