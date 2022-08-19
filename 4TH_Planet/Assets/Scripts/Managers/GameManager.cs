using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    [Header("Managers")]    
    [SerializeField] private SceneLoadManager _sceneLoadManager;
    
    //Criando a referencia(a variavel pública e estática) global da classe

    public static GameManager Instance;

    private void Awake() {
        
        //Verificando se existe uma instancia da classe do GameManager

        if(Instance != null)
        {
            //Se existir, sera exibido essa mensagem:
            Debug.Log("Já existe uma instancia dessa classe!");
            Destroy(this.gameObject);

        }
        else
        {
            //Se não existir a instancia será armazenado na variavel
            Instance = this;
        }

        //garantindo que a classe GameManager não seja destruida ao trocar de cena

        DontDestroyOnLoad(this.gameObject);

        InitializeSystems();
    }

    private void Start() {
        
        //Acessando a classe do SceneLoadM para carregar uma cena através do método LoadScene.
        _sceneLoadManager.LoadScene("Menu");
    }

    private void InitializeSystems()
    {
        SaveSystem.Load();
    }

    public SceneLoadManager SceneLoadManager => _sceneLoadManager;
}
