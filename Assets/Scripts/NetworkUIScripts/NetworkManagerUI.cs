using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    Button m_StartServerButton;
    [SerializeField]
    Button m_StartHostButton;
    [SerializeField]
    Button m_StartClientButton;

    void Awake()
    {
        if (!FindAnyObjectByType<EventSystem>())
        {
            var inputType = typeof(StandaloneInputModule);

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
            eventSystem.transform.SetParent(transform);
        }
    }

    void Start()
    {
        m_StartServerButton.onClick.AddListener(StartServer);
        m_StartHostButton.onClick.AddListener(StartHost);
        m_StartClientButton.onClick.AddListener(StartClient);
    }

    void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        //DeactivateButtons();
    }
    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        //DeactivateButtons();
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        //DeactivateButtons();
    }

    void DeactivateButtons()
    {
        m_StartServerButton.interactable = false;
        m_StartHostButton.interactable = false;
        m_StartClientButton.interactable = false;
    }
}
