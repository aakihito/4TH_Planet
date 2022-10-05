using Cinemachine;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
   [SerializeField] private GameObject _tPlayer;
   [SerializeField] private Transform _tFollowTarget;
    private CinemachineVirtualCamera vcam;
 
    // Use this for initialization
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }
 
    // Update is called once per frame
    void Update()
    {
        if (_tPlayer == null)
        {
            _tPlayer = GameObject.FindWithTag("Player");
            if (_tPlayer != null)
            {
                _tFollowTarget = _tPlayer.transform;
                vcam.LookAt = _tFollowTarget;
                vcam.Follow = _tFollowTarget;
            }
        }
    }
}
