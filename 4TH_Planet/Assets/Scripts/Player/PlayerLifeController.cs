using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeController : MonoBehaviour
{
  HudController _hudManager;
  PlayerController _playerMove;

  public static BoxCollider2D boxCol;

    private void Start()
    {
        _hudManager = HudController.hudController;
        _playerMove = PlayerController.playerControl;

        boxCol = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.tag == "Dangerous")
        {
            _hudManager.LoseAllLife();
            boxCol.enabled = false;
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            //Debug.Log("Trigger ativado: " + collision.name);
            //Debug.Log("Box colider desativado:" + boxCol);
            _hudManager.LosingLife();
            boxCol.enabled = false;

           if (_hudManager.life > 0)
           {
                StartCoroutine(_playerMove.DamagePlayer());
           }
        }
    }
}
