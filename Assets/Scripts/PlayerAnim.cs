﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerAnim : MonoBehaviourPunCallbacks, IPunObservable
{
    public PlayerManager playerManager;
    private GameManager GameManager;
    public Animator Player_ani;
    private float nowSpeed;
    public BoxCollider Shield, Sword;
    public GameObject sword, SwordTeam;
    public bool Laying;
    public bool isAction, isInvincible;
    public SpUI spui;

    public Text te;
    public Text ReadyButtonText;
    public bool isReady;


    void ff(string ss)
    {
        // 我們不假設有一個feedbackText定義。
        if (te == null)
        {
            return;
        }

        // 將新消息添加為新行並位於日誌底部。
        te.text += System.Environment.NewLine + ss;
    }


    void Start()
    {
        ReadyButtonText = GameObject.Find("ReadyButton").GetComponentInChildren<Text>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Laying = false;
    }
    void FixedUpdate()
    {
        #region Photon部份
        if (!photonView.IsMine)
        {
            Player_ani.applyRootMotion = false;
            return;
        }
        else
        {
            SwordTeam.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.P))
            te.text = "";

        #endregion
        if (Input.GetMouseButton(1))
        {
            Player_ani.SetBool("Block", true);
        }
        else
        {
            Player_ani.SetBool("Hurt", false);
            Player_ani.SetBool("Blocking", false);
            Player_ani.SetBool("Block", false);
            Shield.enabled = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            /*可以處決時
            if ()
            {
                Player_ani.SetBool("DeathAttack", true);
            }
            else*/
            if (Input.GetKey(KeyCode.LeftControl) && playerManager.PlayerSp >= 40)
            {
                Player_ani.SetBool("HeavyAttack", true);
            }
            else if (playerManager.PlayerSp >= 10)
            {

                Player_ani.SetBool("Attack", true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && playerManager.PlayerSp >= 20)
        {
            Player_ani.SetBool("Dodge", true);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Player_ani.SetBool("Throw", true);
        }
        if (Input.GetKeyDown(KeyCode.F1) && !GameManager.isStartGame)
        {
            if (gameObject.tag == "RedPlayer")
            {
                GameManager.RedTeamNumber--;
            }
            if (gameObject.tag == "BluePlayer")
            {
                GameManager.BlueTeamNumber--;
            }
            GameObject.Find("PlayerUI " + playerManager.photonView.Owner.NickName).GetComponent<PlayerUI>().playerNameText.color = new Color(255, 255, 255);
            gameObject.transform.position = GameObject.Find("StartTp").GetComponent<Transform>().position;
            SwordTeam.tag = "Sword";
            gameObject.tag = "Player";
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReady)
            {
                GameManager.playerReady++;
                ReadyButtonText.text = "已就緒(R鍵)";
                ReadyButtonText.color = Color.red;
                isReady = !isReady;
            }
            else
            {
                GameManager.playerReady--;
                ReadyButtonText.text = "準備(R鍵)";
                ReadyButtonText.color = Color.black;
                isReady = !isReady;
            }
        }
        /*
        if (Laying)
        {
            //if() 被處決決判時
            //Player_ani.SetBool("BeDeathAttacking", true);
        }
        被處決判定打到時呼叫
        public void BeDeathAttack()
        {
            Player_ani.SetBool("BeDeathAttack", true);
        }
        */
        if (playerManager.PlayerHp <= 0)
        {
            Player_ani.SetBool("Dead", true);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        ff("碰到trigger");
        if (gameObject.tag == "RedPlayer")
        {
            if (other.CompareTag("BlueSword") && !isInvincible)
            {
                ff("被籃隊打到");
                Player_ani.SetBool("Hurt", true);
                Player_ani.SetBool("Blocking", false);
                playerManager.CoHp(10);
            }
            else if (other.CompareTag("BlueBigSword") && !isInvincible)
            {
                ff("被籃隊重攻擊打到");
                Player_ani.SetBool("KnockDown", true);
                playerManager.CoHp(20);
            }
        }
        else if (gameObject.tag == "BluePlayer")
        {
            if (other.CompareTag("RedSword") && !isInvincible)
            {
                ff("被紅隊打到");
                Player_ani.SetBool("Hurt", true);
                Player_ani.SetBool("Blocking", false);
                playerManager.CoHp(10);
            }
            else if (other.CompareTag("RedBigSword") && !isInvincible)
            {
                ff("被紅隊重攻擊打到");
                Player_ani.SetBool("KnockDown", true);
                playerManager.CoHp(20);
            }
        }
        if (other.CompareTag("RedTeam"))
        {
            GameObject.Find("PlayerUI " + playerManager.photonView.Owner.NickName).GetComponent<PlayerUI>().playerNameText.color = new Color(255, 0, 0);
            SwordTeam.tag = "RedSword";
            gameObject.tag = "RedPlayer";
            ff("加入紅隊");
            GameManager.RedTeamNumber++;
            gameObject.transform.position = GameObject.Find("RedTp").GetComponent<Transform>().position;
        }
        if (other.CompareTag("BlueTeam"))
        {
            GameObject.Find("PlayerUI " + photonView.Owner.NickName).GetComponent<PlayerUI>().playerNameText.color = new Color(0, 0,255);
            SwordTeam.tag = "BlueSword";
            gameObject.tag = "BluePlayer";
            ff("加入藍隊");
            GameManager.BlueTeamNumber++;
            gameObject.transform.position = GameObject.Find("BlueTp").GetComponent<Transform>().position;
        }
    }
    #region IPunObservable implementation  Photon部分

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.tag);
            stream.SendNext(SwordTeam.tag);
            stream.SendNext(GameObject.Find("PlayerUI " + photonView.Owner.NickName).GetComponent<PlayerUI>().playerNameText.color.r);
            stream.SendNext(GameObject.Find("PlayerUI " + photonView.Owner.NickName).GetComponent<PlayerUI>().playerNameText.color.g);
            stream.SendNext(GameObject.Find("PlayerUI " + photonView.Owner.NickName).GetComponent<PlayerUI>().playerNameText.color.b);
            stream.SendNext(GameManager.playerReady);
            stream.SendNext(GameManager.RedTeamNumber);
            stream.SendNext(GameManager.BlueTeamNumber);

        }
        else
        {
            this.tag = (string)stream.ReceiveNext();
            SwordTeam.tag = (string)stream.ReceiveNext();
            GameObject.Find("PlayerUI " + photonView.Owner.NickName).GetComponent<PlayerUI>().playerNameText.color = new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
            GameManager.playerReady = (int)stream.ReceiveNext();
            GameManager.RedTeamNumber = (int)stream.ReceiveNext();
            GameManager.BlueTeamNumber = (int)stream.ReceiveNext();
        }
    }

    #endregion

    public void isActionTrue()
    {
        isAction = true;
    }
    public void ComboSp()
    {
        if (SwordTeam.tag.Contains("Blue"))
        {
            SwordTeam.tag = "BlueSword";
        }
        else if (SwordTeam.tag.Contains("Red"))
        {
            SwordTeam.tag = "RedSword";
        }
        isActionTrue();
        playerManager.CoSp(10);
    }
    public void HeavyAttackSp()
    {
        if (SwordTeam.tag.Contains("Blue"))
        {
            SwordTeam.tag = "BlueBigSword";
        }
        else if (SwordTeam.tag.Contains("Red"))
        {
            SwordTeam.tag = "RedBigSword";
        }
        playerManager.CoSp(50);
    }
    public void AttackFalse()
    {
        Player_ani.SetBool("Attack", false);
        Player_ani.SetBool("HeavyAttack", false);
    }
    public void SwordTrue()
    {
        Sword.enabled = true;
    }
    public void SwordFalse()
    {
        Sword.enabled = false;
    }
    public void DodgeSp()
    {
        Shield.enabled = false;
        sword.SetActive(true);
        isInvincible = true;
        playerManager.CoSp(20);
    }
    public void DodgingFalse()
    {
        Player_ani.SetBool("Dodging", false);
        Player_ani.SetBool("Dodge", false);
    }
    public void DodgingTrue()
    {
        Player_ani.SetBool("Dodging", true);
    }
    public void KnockDownFalse()
    {
        Player_ani.SetBool("Hurt", false);
        sword.SetActive(true);
        isInvincible = true;
        Player_ani.SetBool("KnockDown", false);
    }
    public void Throw()
    {
        isActionTrue();
        sword.SetActive(false);
    }
    public void AllEnd()
    {
        Player_ani.SetBool("Hurt", false);
        sword.SetActive(true);
        isInvincible = false;
        isAction = false;
        spui.Slow = 1;
        AttackFalse();
        DodgingFalse();
        DeathAttackFalse();
        Player_ani.SetBool("BeDeathAttack", false);
        Player_ani.SetBool("Throw", false);
    }
    public void Defense()
    {
        isAction = false;
        spui.Slow = 0.5f;
        Player_ani.SetBool("Blocking", true);
        Shield.enabled = true;
        DodgingFalse();
    }
    public void DeathAttackFalse()
    {
        Player_ani.SetBool("DeathAttack", false);
    }
    public void HurtFalse()
    {
        sword.SetActive(true);
        SwordFalse();
        Player_ani.SetBool("Hurt", false);
        DodgingTrue();
    }
    public void BreakShieldFalse()
    {
        Player_ani.SetBool("BreakShield", false);
    }
    public void LayingTrue()
    {
        Laying = true;
    }
    public void StandUp()
    {
        Player_ani.SetBool("BeDeathAttacking", false);
        Laying = false;
    }
    public void Die()
    {
        sword.SetActive(true);
        Player_ani.SetBool("Die", true);
    }
}
