using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    //LISTA QUE IRA GUARDAR TODOS OS LAYOUTS DO MENU
    [SerializeField] private List<GameObject> _layouts;

    [SerializeField] private int _firstLayout;


    private void Start()
    {
        StartLayout();

       

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
        GameManager.Instance.SceneLoadManager.LoadScene("Lvl_01");
    }

}
