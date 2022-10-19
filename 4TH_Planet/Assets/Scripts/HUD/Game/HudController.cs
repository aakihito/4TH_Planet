using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public static HudController hudController {get; private set;}

    #region LIFE

    [Header("LIFE")]
    public Image faceCharacter;
    public Image hearts;
    public GameObject faceDamage;
    public Sprite[] sprites = new Sprite[6];

    public int life = 6;
    public Sprite[] heartsInHud = new Sprite[7];

    #endregion
    
    private PlayerControllerTest _playerControl;

    private void Awake()
    {
        if (hudController == null)
        {
            hudController = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start() 
    {
        faceCharacter = faceCharacter.GetComponent<Image>();   
        _playerControl = PlayerControllerTest.instance; 
        hearts = hearts.GetComponent<Image>();
    }

    public void LosingLife()
    {
        life--;
        LifeHud();
    }
    public void LoseAllLife()
    {
        life = 0;
        LifeHud();
    }

    public int Life()
    {
        return life;
    }

    public void LifeHud()
    {
        if(life <= 0)
        {
            life = 0;
        }
        else if(life >= 6)
        {
            life = 6;
        }

        switch (life)
        {
            case 6:
                Damage();
                faceCharacter.sprite = sprites[6];
                hearts.sprite = heartsInHud[6];
            break;

            case 5:
                Damage();
                faceCharacter.sprite = sprites[5];
                hearts.sprite = heartsInHud[5];
            break;

            case 4:
                Damage();
                faceCharacter.sprite = sprites[4];
                hearts.sprite = heartsInHud[4];
            break;

            case 3:
                Damage();
                faceCharacter.sprite = sprites[3];
                hearts.sprite = heartsInHud[3];
            break;
            
            case 2:
                Damage();
                faceCharacter.sprite = sprites[2];
                hearts.sprite = heartsInHud[2];
            break;

            case 1:
                Damage();
                hearts.sprite = heartsInHud[1];
                faceCharacter.sprite = sprites[1];
            break;

            default:
                Damage();
                faceCharacter.sprite = sprites[0];
                hearts.sprite = heartsInHud[0];
                _playerControl.PlayerDeath();
                
            break;
            
        }
    }

    private void Damage()
    {
        StartCoroutine(DamageHUD());
    }

    IEnumerator DamageHUD()
    {
        faceDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        faceDamage.SetActive(false);
    }
}
