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

    public void OnClick_StartGame()
    {
        _uiHandler.PlayButtonClickSound();
        _uiHandler.StartGame();
    }

    public void OnClick_OpenMainMenuScreen()
	{
        _uiHandler.PlayButtonClickSound();
        _uiHandler.OpenMainMenuScreen();
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
    public void OnClick_LeaveRoom()
	{
        PhotonNetwork.LeaveRoom();
        _uiHandler.WaitingRoomLeft();
    }
}
