using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    //LISTA QUE IRA GUARDAR TODOS OS LAYOUTS DO MENU
    [SerializeField] private List<GameObject> _layouts;

    [SerializeField] private int _firstLayout;

    [SerializeField] private AudioSource _audioSourceButton;


    private void Start()
    {
        StartLayout();

       GameManager.Instance.AudioManager.PlayBackgroundMusic(0);

    }


    private void StartLayout()
    {
        for(int i = 0; i <_layouts.Count; i++)
        {
            _layouts[i].gameObject.SetActive(false);
        }

        _layouts[_firstLayout].gameObject.SetActive(true);
    }


    public void EnableLayout(int indexLayout)
    {
        _layouts[indexLayout].gameObject.SetActive(true);
        GameManager.Instance.AudioManager.PlaySoundEffect(_audioSourceButton,1);
    }

    public void DisableLayout(int indexLayout)
    {
        _layouts[indexLayout].gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        GameManager.Instance.SceneLoadManager.LoadScene("Tutorial");
    }

    public void SaveConfig()
    {
        SaveSystem.Save();
    }

}
