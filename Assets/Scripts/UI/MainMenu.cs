using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    private static MainMenuUI instance;

    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_Text joinCodeText;

    private RelayManager relayManager;

    private void Awake()
    {
        // Ensure RelayManager is available
        relayManager = FindFirstObjectByType<RelayManager>();
        if (relayManager == null)
        {
            Debug.LogError("RelayManager not found in the scene.");
            enabled = false;
            return;
        }

        // Add listeners to buttons
        hostButton.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
        // Ensure this is the only instance of RelayManager
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnHostButtonClicked()
    {
        Debug.Log("Host Button Clicked");
        relayManager.SendMessage("CreateRelay");
        joinCodeText.SetText($"Code: {relayManager.GetJoinCode()}");
    }

    private void OnJoinButtonClicked()
    {
        string joinCode = joinCodeInputField.text.Trim();
        if (string.IsNullOrEmpty(joinCode))
        {
            Debug.LogWarning("Join code cannot be empty.");
            return;
        }

        Debug.Log($"Join Button Clicked with Code: {joinCode}");
        relayManager.SendMessage("JoinRelay", joinCode);
    }
}
