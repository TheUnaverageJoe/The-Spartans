using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Core.Singletons;

public class PlayerCameraFollow : CustomSingleton<PlayerCameraFollow>
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    [HideInInspector] new public Camera camera;

    // Start is called before the first frame update
    void Awake()
    {
        camera = GetComponent<Camera>();
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void FollowPlayer(Transform transform){
        cinemachineVirtualCamera.Follow = transform;
    }

    public void LookAtPlayer(Transform transform){
        cinemachineVirtualCamera.LookAt = transform;
    }
}
