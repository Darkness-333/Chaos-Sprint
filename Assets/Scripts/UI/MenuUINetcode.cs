using Mirror;
using TMPro;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using kcp2k;
using Unity.Networking.Transport.Relay;

using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class MenuUINetcode : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    public void StartGame() {
        // Загрузка сцены игры
        SceneManager.LoadScene("Game"); // Название сцены игры
    }

    private async void Start() {
        // Инициализация Unity Services
        //await InitializeUnityServices();

        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn) {

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

    }
    private async Task InitializeUnityServices() {
        await UnityServices.InitializeAsync();
        Debug.Log("Unity Services Initialized");
        await SignIn(); // Вход в систему
    }
    private async Task SignIn() {
        if (!AuthenticationService.Instance.IsSignedIn) {
            try {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in anonymously");
            }
            catch (System.Exception ex) {
                Debug.LogError("Failed to sign in: " + ex.Message);
            }
        }
    }



    public async void RelayHostGame() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(10);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Join Code: " + joinCode + " ipv4: " + allocation.RelayServer.IpV4 + " port: " + allocation.RelayServer.Port + " region: " + allocation.Region); // Убедитесь, что код корректен
            PlayerPrefs.SetString("Code", "Join Code: " + joinCode + " ipv4: " + allocation.RelayServer.IpV4 + " port: " + allocation.RelayServer.Port + " region: " + allocation.Region);
            PlayerPrefs.Save();

            var relayServerData = new RelayServerData(allocation, "dtls");

            Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Unity.Netcode.NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException ex) {
            Debug.LogError("Failed to create host: " + ex.Message);
        }


    }

    public async void RelayJoinGame() {

        string joinCode = inputField.text;
        print("joinCode "+joinCode);

        print("join game before allocation second");
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        print("join game after allocation");

        Debug.Log("Join Code: " + joinCode + " ipv4: " + joinAllocation.RelayServer.IpV4 + " port: " + joinAllocation.RelayServer.Port + " region: " + joinAllocation.Region); // Убедитесь, что код корректен

        var relayServerData = new RelayServerData(joinAllocation,"dtls");
        Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        Unity.Netcode.NetworkManager.Singleton.StartClient();


    }

    public void HostGame() {
        Mirror.NetworkManager.singleton.maxConnections = 10;
        // Запуск хоста
        Mirror.NetworkManager.singleton.StartHost();
    }

    public void JoinGame() {

        // Подключение клиента
        string inputFieldText=inputField.text;
        if(inputFieldText.Length==0) {
            inputFieldText = "localhost";
        }
        Mirror.NetworkManager.singleton.networkAddress = inputFieldText;
        Mirror.NetworkManager.singleton.StartClient();

    }

    public void SinglePlayerGame() {
        Mirror.NetworkManager.singleton.maxConnections = 1;
        Mirror.NetworkManager.singleton.StartHost();

    }
    public void ExitGame() {
        Application.Quit();
    }
}
