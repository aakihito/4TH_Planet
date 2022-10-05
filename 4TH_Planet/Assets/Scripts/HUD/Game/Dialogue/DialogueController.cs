using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{

    public static DialogueController instance;

#region EVENTS
    public static event System.Action<Dialogue> NewCharacter;
    public static event System.Action ResetText;
    public static event System.Action<string> ShowMessage;
    public static event System.Action<bool> UIState;
#endregion    

    private DialogueSpeaker currentDialogue;
    private bool endCurrentDialogue = true;

    private bool buttonClicked = false;


    private void Awake() 
    {
      if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);   
    }

    public void StartConversation(DialogueSpeaker speaker)
    {
        currentDialogue = speaker;
        StartCoroutine(StartDialogue());
        UIState?.Invoke(true);
    }

    private IEnumerator StartDialogue()
    {
        //laço para definir os personagens
        for(int i = 0; i < currentDialogue.dialogues.Length; i++)
        {
            ResetText?.Invoke();
            NewCharacter?.Invoke(currentDialogue.dialogues[i]);
            StartCoroutine(ShowDialogue(currentDialogue.dialogues[i].dialogue));
        
            yield return new WaitUntil(() => endCurrentDialogue);
        }

        UIState.Invoke(false);
    }

    private IEnumerator ShowDialogue(string[] messages)
    {
        endCurrentDialogue = false;

        foreach(var message in messages)
        {
            ShowAllMessages(message);
            yield return new WaitUntil(() => buttonClicked);
        }

        endCurrentDialogue = true;
    }

    private void ShowAllMessages(string message)
    {
        ShowMessage?.Invoke(message);
        buttonClicked = false;
    }

    private void ButtonWasClickewd() =>
        buttonClicked = true;
}
