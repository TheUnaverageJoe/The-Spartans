using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Core.Singletons;

public class PlayerCameraFollow : CustomSingleton<PlayerCameraFollow>
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    // Start is called before the first frame update
    void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void FollowPlayer(Transform transform){
        cinemachineVirtualCamera.Follow = transform;
    }

    public void LookAtPlayer(Transform transform){
        cinemachineVirtualCamera.LookAt = transform;
    }
}
