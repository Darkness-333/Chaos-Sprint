using Mirror;
using TMPro;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Network;
using Utp;

public class MenuUI : MonoBehaviour {
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private NetworkManagerUTP networkManagerUTP;

    private async void Start() {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn) {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public void RelayHost() {
        int maxPlayers = 10;
        //NetworkManager.singleton.GetComponent<UtpTransport>().Port = ushort.Parse(ipInputField.text);
        networkManagerUTP.StartRelayHost(maxPlayers);
    }

    public void RelayClient() {
        networkManagerUTP.relayJoinCode = codeInputField.text;
        networkManagerUTP.JoinRelayServer();
    }

    public void LocalHost() {
        NetworkManager.singleton.maxConnections = 10;

        NetworkManager.singleton.StartHost();
    }

    public void LocalClient() {

        string inputFieldText = ipInputField.text;
        if (inputFieldText.Length == 0) {
            inputFieldText = "localhost";
        }
        NetworkManager.singleton.networkAddress = inputFieldText;
        NetworkManager.singleton.StartClient();

    }

    public void SinglePlayerGame() {
        NetworkManager.singleton.maxConnections = 1;
        NetworkManager.singleton.StartHost();

    }


    public void ExitGame() {
        Application.Quit();
    }
}



//public async void RelayHostGame() {

//    try {
//        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(10);
//        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

//        Debug.Log("Join Code: " + joinCode + " ipv4: " + allocation.RelayServer.IpV4 + " port: " + allocation.RelayServer.Port + " region: " + allocation.Region); // Убедитесь, что код корректен
//        PlayerPrefs.SetString("Code", "Join Code: " + joinCode + " ipv4: " + allocation.RelayServer.IpV4 + " port: " + allocation.RelayServer.Port + " region: " + allocation.Region);
//        PlayerPrefs.Save();

//        NetworkManager.singleton.networkAddress = allocation.RelayServer.IpV4;
//        NetworkManager.singleton.GetComponent<UtpTransport>().Port = (ushort)allocation.RelayServer.Port;

//        NetworkManager.singleton.maxConnections = 10;
//        NetworkManager.singleton.StartHost();
//    }
//    catch (RelayServiceException ex) {
//        Debug.LogError("Failed to create host: " + ex.Message);
//    }
//}

//public async void RelayJoinGame() {
//    string joinCode = ipInputField.text;
//    print("joinCode "+joinCode);

//    JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

//    Debug.Log("Join Code: " + joinCode + " ipv4: " + joinAllocation.RelayServer.IpV4 + " port: " + joinAllocation.RelayServer.Port + " region: " + joinAllocation.Region);

//    NetworkManager.singleton.networkAddress = joinAllocation.RelayServer.IpV4;

//    NetworkManager.singleton.GetComponent<UtpTransport>().Port = (ushort)joinAllocation.RelayServer.Port;

//    NetworkManager.singleton.StartClient();
//}