using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
    private int maxConnections = 3;

    [SerializeField]
    Button m_CreateRelayButton;
    [SerializeField]
    Button m_JoinRelayButton;
    [SerializeField]
    TMP_InputField m_JoinInput;
    [SerializeField]
    TextMeshProUGUI m_InGameCode;
    [SerializeField]
    Canvas m_Canvas;
    [SerializeField]
    Canvas m_Canvas1;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed In {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        m_CreateRelayButton.onClick.AddListener(CreateRelay);
        m_JoinRelayButton.onClick.AddListener(() => JoinRelay(m_JoinInput.text));
    }

    async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Comment it Later
            //Debug.Log($"Code : {joinCode}");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );

            NetworkManager.Singleton.StartHost();
            //return joinCode;
            m_Canvas.enabled = false;
            m_InGameCode.text = $"Code: {joinCode}";
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
            //return null;
        }
    }

    async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log($"Joining Relay with : {joinCode}");
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
                );

            NetworkManager.Singleton.StartClient();


            m_Canvas.enabled = false;
            m_Canvas1.enabled = false;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
        }
    }
}
