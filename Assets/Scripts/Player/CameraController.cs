using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField]private CinemachineVirtualCamera m_Camera;

    private void Update()
    {
        if (IsOwner)
        {
            m_Camera.Priority = 1;
        }
        else
        {
            m_Camera.Priority = 0;
        }
    }
}
