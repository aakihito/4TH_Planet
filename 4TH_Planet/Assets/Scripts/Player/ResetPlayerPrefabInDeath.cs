using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerPrefabInDeath : MonoBehaviour
{

   #region PREFAB
    public static GameObject _prefab;
    private PlayerControllerTest _playerMovePrefab;
    public  Transform _spawnPoint;

    public static ResetPlayerPrefabInDeath resetPlayerPrefab;
	#endregion

    private void Awake() 
    {
        if(resetPlayerPrefab == null)
		{
			resetPlayerPrefab = this;
		}
		else
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);   

        Respawn();
    }

    public void Respawn()
    {
         _prefab = GetComponent<GameObject>();
        _prefab = Resources.Load<GameObject>("Player 1");
		Instantiate(_prefab, _spawnPoint.position, Quaternion.identity);
    }
}
