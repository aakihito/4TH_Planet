using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NpcController : MonoBehaviour
{
  
  [SerializeField] private GameObject _dialoguePanel;
  [SerializeField] private Text _dialogueText;
  [SerializeField] private string[] _dialogue;
  private int _index;

  [SerializeField] private bool _playerIsClose;
  [SerializeField] private float _wordSpeed;  
    void Update()
    {
        if(_playerIsClose)
        {
            if(_dialoguePanel.activeInHierarchy)
            {
                ZeroText();
            }
            else
            {
                _dialoguePanel.SetActive(true);
                StartCoroutine(TypingSpeed());
            }
        }
    }

    public void ZeroText()
    {
        _dialogueText.text = "";
        _index = 0;
        _dialoguePanel.SetActive(false);
    }

    IEnumerator TypingSpeed()
    {
        foreach(char letter in _dialogue[_index].ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(_wordSpeed);
        }
    }
    public void NextLine()
    {
        if(_index < _dialogue.Length - 1)
        {
            _index++;
            _dialogueText.text = "";
            StartCoroutine(TypingSpeed());
        }
        else
        {
            ZeroText();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.CompareTag("Player"))
        {
            _playerIsClose = true;

        }    
    }
    private void OnTriggerExit2D(Collider2D collider) 
    {
        if(collider.CompareTag("Player"))
        {
            _playerIsClose = false;
            ZeroText();

        }    
    }
}
