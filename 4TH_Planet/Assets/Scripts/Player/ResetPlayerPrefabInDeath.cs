using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerPrefabInDeath : MonoBehaviour
{
   #region PREFAB
    public static GameObject _prefab;
    public static GameObject _inputPrefab;
    private PlayerController _playerMovePrefab;
	
	#endregion

    private void Awake() 
    {
        _prefab = GetComponent<GameObject>();
        _inputPrefab = GetComponent<GameObject>();
        _playerMovePrefab = PlayerController.playerControl;
        _prefab = Resources.Load<GameObject>("Player");
        _inputPrefab = Resources.Load<GameObject>("InputManager");
		Instantiate(_prefab, Vector3.zero, Quaternion.identity);
        Instantiate(_inputPrefab);
    }
}
