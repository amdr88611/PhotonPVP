﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public HpUI hpui;
    public SpUI spui;
    public float PlayerHp = 100;
    public float PlayerSp = 100;

    #region Photon部份
    [Tooltip("The Player's UI GameObject Prefab")]
    [SerializeField]
    private GameObject playerUiPrefab;

    public static GameObject LocalPlayerInstance;

    public GameObject PlayerMainCamera;
    public GameObject PlayerFreeCamera;
    public GameObject PlayerUI;

    bool Lock_Cursor;
    public void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            PlayerMainCamera.SetActive(false);
            PlayerFreeCamera.SetActive(false);
            PlayerUI.SetActive(false);
        }
        if (this.playerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        Lock_Cursor = true;
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (this.PlayerHp <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
            if (Input.GetKeyDown(KeyCode.Tab))
                Lock_Cursor = !Lock_Cursor;
            if (Lock_Cursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
    #endregion
    public void CoHp(float damage)
    {
        PlayerHp -= damage;
        hpui.cohp(damage);
    }
    public void CoSp(float damage)
    {
        PlayerSp -= damage;
        spui.CoSp(damage);
        if (PlayerSp < 0)
        {
            PlayerSp = 0;
        }
    }
    #region IPunObservable implementation  Photon部分

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.PlayerHp);
        }
        else
        {
            this.PlayerHp = (float)stream.ReceiveNext();
        }
    }

    #endregion
}
