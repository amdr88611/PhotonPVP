using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviourPunCallbacks
{

    #region Public Fields

    static public GameManager Instance;

    #endregion

    #region Private Fields

    private GameObject instance;
    private Text ReadyButtonText;
    [Tooltip("玩家的Prefab")]
    [SerializeField]
    private GameObject playerPrefab;

    
    public Animator DoorAnim;
    public Text ttt;
    public int playerNumber;
    public int playerReady;
    public int RedTeamNumber;
    public int BlueTeamNumber;
    //public bool isReady;
    public bool isStartGame;

    #endregion

    #region MonoBehaviour CallBacks

    void Start()
    {
        Instance = this;
        //ReadyButtonText = GameObject.Find("ReadyButton").GetComponentInChildren<Text>();
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Launcher");
            return;
        }

        //生成玩家
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }

    void Update()
    {
        //ttt.text = "" + isReady + "\n" + "紅隊數量" +RedTeamNumber + "\n" + "藍隊數量" + BlueTeamNumber + "\n" + "準備數量" + playerReady;
        playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerNumber == playerReady && RedTeamNumber >=1 && BlueTeamNumber >=1)
        {
            DoorAnim.SetBool("DoorOpen", true);
            isStartGame = true;
        }
        //Esc離開遊戲
        if (Input.GetKeyDown(KeyCode.F12))
        {
            QuitApplication();
        }
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReady)
            {
                playerReady++;
                ReadyButtonText.text = "已就緒(R鍵)";
                ReadyButtonText.color = Color.red;
                isReady = !isReady;
            }
            else
            {
                playerReady--;
                ReadyButtonText.text = "準備(R鍵)";
                ReadyButtonText.color = Color.black;                
                isReady = !isReady;
            }
        }*/

    }

    #endregion

    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("OnPlayerEnteredRoom() " + other.NickName + "玩家 加入了房間！"); // not seen if you're the player connecting

    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    /*暫時別用 會閃退
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    */
    public void QuitApplication()
    {
        Application.Quit();
    }
    /*public void ReadyButton()
    {
        if (!isReady)
        {
            playerReady++;
            ReadyButtonText.text = "已就緒";
            isReady = !isReady;
        }
        else
        {
            playerReady--;
            ReadyButtonText.text = "準備";
            isReady = !isReady;
        }
    }*/
    #endregion

    #region IPunObservable implementation  Photon部分
    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerReady);
            stream.SendNext(RedTeamNumber);
            stream.SendNext(BlueTeamNumber);
        }
        else
        {
            this.playerReady = (int)stream.ReceiveNext();
            this.RedTeamNumber = (int)stream.ReceiveNext();
            this.BlueTeamNumber = (int)stream.ReceiveNext();
        }
    }
    */
    #endregion

}

