using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPun
{
    public PlayerManager playerManager;
    private Animator Player_ani;
    private float nowSpeed;
    private Camera PlayerCamera;
    public PlayerAnim player;
    void Start()
    {
        PlayerCamera = Camera.main;
        Player_ani = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        #region Photon部份
        if (!photonView.IsMine)
            return;
        #endregion
        if (Player_ani.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Walk") ||
          Player_ani.GetNextAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.Walk"))
        {
            if (Input.GetKey(KeyCode.LeftShift) && playerManager.PlayerSp > 0)
            {
                if(nowSpeed < Mathf.Max(Mathf.Abs(Input.GetAxisRaw("Horizontal")), Mathf.Abs(Input.GetAxisRaw("Vertical"))))
                {
                    nowSpeed += 0.02f;
                }
                playerManager.CoSp(0.1f);
                player.isActionTrue();
            }
            else
            {
                if(nowSpeed > 0)
                {
                    nowSpeed -= 0.02f;
                }
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    player.isAction = false;
                }
            }
            Vector3 RelativeDir = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            Debug.Log(RelativeDir + "/");
            Debug.Log("Right");
            CharMove(this.transform, PlayerCamera, RelativeDir, nowSpeed);
            Player_ani.SetFloat("Speed", nowSpeed);
        }
        else
        {
            if (nowSpeed > 0)
            {
                nowSpeed = 0;
                Player_ani.SetFloat("Speed", nowSpeed);
            }
        }
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            Player_ani.SetBool("Walk", true);
        }
        else
        {
            Player_ani.SetBool("Walk", false);
        }
    }
    void CharMove(Transform CharTarget, Camera Cam, Vector3 Direct, float Speed)
    {
        Vector3 CamForward = Cam.transform.forward;
        Vector3 CamRight = Cam.transform.right;
        Vector3 CamCoordToWorldCoord_Dir = new Vector3(Direct.x * CamForward.x + Direct.z * CamRight.x, 0,
            Direct.z * CamRight.z + Direct.x * CamForward.z);
        CamCoordToWorldCoord_Dir.Normalize();
        Debug.Log(CamCoordToWorldCoord_Dir);
        CharTarget.LookAt
            (
                Vector3.Lerp(CharTarget.position + CharTarget.forward,
                CharTarget.position + CamCoordToWorldCoord_Dir, 0.5f)
            );
        CharTarget.position += CamCoordToWorldCoord_Dir * (1 + Speed) * Time.deltaTime;
    }
}