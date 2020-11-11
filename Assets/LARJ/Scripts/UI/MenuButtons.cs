using Photon.Pun;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{

    [SerializeField] private UIHandler _uiHandler;

    public void OnClick_ExitGame()
    {
        _uiHandler.PlayButtonClickSound();
        _uiHandler.ExitGame();
    }
    public void OnClick_OpenStartPlayScreen()
    {
        _uiHandler.PlayButtonClickSound();
        _uiHandler.OpenStartPlayScreen();
    }

    public void OnClick_StartLevel1()
    {
        _uiHandler.PlayButtonClickSound();
        _uiHandler.Startlevel1();
    }
    public void OnClick_StartLevel2()
    {
        _uiHandler.PlayButtonClickSound();
        _uiHandler.Startlevel2();
    }
    public void OnClick_StartLevel3()
    {
        _uiHandler.PlayButtonClickSound();
        _uiHandler.Startlevel3();
    }

    public void OnClick_OpenMainMenuScreen()
	{
        _uiHandler.PlayButtonClickSound();
        _uiHandler.OpenMainMenuScreen();
    }
    public void OnClick_OpenSettingsScreen()
    {
        _uiHandler.OpenSettingsScreen();
        _uiHandler.PlayButtonClickSound();
    }
    public void OnClick_OpenCreditsScreen()
    {
        _uiHandler.OpenCreditsScreen();
        _uiHandler.PlayButtonClickSound();
    }
    public void OnClick_OpenLocalLevelSelectionScreen()
    {
        _uiHandler.OpenLocalLevelSelectionScreen();
        _uiHandler.PlayButtonClickSound();
    }

    public void OnClick_OpenNetworkedLevelSelectionScreen()
    {
        _uiHandler.OpenNetworkedLevelSelectionScreen();
        _uiHandler.PlayButtonClickSound();
    }

    public void OnClick_EnterLocalSection()
	{
        _uiHandler.PlayButtonClickSound();
        _uiHandler.EnterLocalSection();
    }

	public void OnClick_EnterNetworkSection()
	{
        _uiHandler.PlayButtonClickSound();
        _uiHandler.TryEnterNetworkSection();
    }

    public void OnClick_CloseFailedToConnectDialog()
	{
        _uiHandler.EnableFailedToConnectDialog(false);
	}

    public void OnClick_BackToWaitingForPlayers()
	{
        _uiHandler.WaitingRoomJoined();
        _uiHandler.UpdatePlayerList();
    }

    public void OnClick_LeaveRoom()
	{
        PhotonNetwork.LeaveRoom();
        _uiHandler.WaitingRoomLeft();
    }
}
