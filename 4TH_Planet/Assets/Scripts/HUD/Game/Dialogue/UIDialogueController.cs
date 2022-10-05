using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDialogueController : MonoBehaviour
{
    public GameObject dialoguePanel;
   public Image image;
   public TMP_Text characterName;
   public TMP_Text dialogue;

   private void Awake() 
   {
          DialogueController.NewCharacter += NewCharacter;
          DialogueController.ShowMessage += ShowText;
          DialogueController.ResetText += ResetText;
          DialogueController.UIState += DialoguePanelState;
   }

   private void OnDestroy() 
   {
          DialogueController.NewCharacter -= NewCharacter;
          DialogueController.ShowMessage -= ShowText;
          DialogueController.ResetText -= ResetText;
          DialogueController.UIState -= DialoguePanelState;
   }
    
   private void ShowText(string message) =>
        dialogue.text = message;

   private void ResetText() =>
        dialogue.text = string.Empty;

   private void DialoguePanelState(bool state) =>
        dialoguePanel.SetActive(state);

   private void NewCharacter(Dialogue CharacterInformation)
   {
        //image.sprite = CharacterInformation.dialogue.sprite;
        //characterName.text = CharacterInformation.dialogue.name;
   }               
}
