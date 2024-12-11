using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class RelayManager : MonoBehaviour
{
    private static RelayManager instance;
    private int maxConnections = 3;
    private string joinCode;
    [SerializeField]private TMP_Text joinTextCode;

    private async void Awake()
    {
        // Ensure this is the only instance of RelayManager
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed In {AuthenticationService.Instance.PlayerId}");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            // Check if the current scene is not "MainMenu" before proceeding
            SceneManager.LoadScene("TestScene");  // Or your actual gameplay scene name
            Debug.Log("Creating Relay...");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log(joinCode);
            joinTextCode.SetText($"Code: {joinCode}");


            // Set relay server data for the host
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // Load the gameplay scene and start hosting
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError($"Failed to create relay: {ex}");
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            // Check if the current scene is not "MainMenu" before proceeding
            SceneManager.LoadScene("TestScene");  // Or your actual gameplay scene name
            Debug.Log($"Joining Relay with code: {joinCode}");
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // Set relay server data for the client
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            // Load the gameplay scene and start the client
            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException ex)
        {
            Debug.LogError($"Failed to join relay: {ex}");
        }
    }
    public string GetJoinCode()
    {
        return joinCode;
    }
}
