using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnim : MonoBehaviourPun
{
    public PlayerManager playerManager;
    private Animator Player_ani;
    private float nowSpeed;
    public BoxCollider Shield, Sword;
    public GameObject sword;
    public bool Laying;
    public bool isAction, isInvincible;
    public SpUI spui;

    void Start()
    {
        Laying = false;
        Player_ani = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        #region Photon部份
        if (!photonView.IsMine)
        {
            Player_ani.applyRootMotion = false;
            return;
        }
        #endregion
        if (Input.GetMouseButton(1))
        {
            Player_ani.SetBool("Block", true);
        }
        else
        {
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
        /*
        if() 被打倒時
        {
            Player_ani.SetBool("KnockDown", true);
        }
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
            return;

        if (other.CompareTag("Sword") && !isInvincible)
        {
            if (!Shield.enabled)
            {
                Player_ani.SetBool("Hurt", true);
                playerManager.CoHp(10);
            }
            else if (Shield.enabled)
            {
                playerManager.CoSp(15);
                
                if (playerManager.PlayerSp <= 0)
                {
                    Player_ani.SetBool("BreakShield", true);
                }
                else
                {
                    Player_ani.SetBool("Hurt", true);
                }
            }
        }
    }
    public void isActionTrue()
    {
        isAction = true;
    }
    public void ComboSp()
    {
        isActionTrue();
        playerManager.CoSp(10);
    }
    public void HeavyAttackSp()
    {
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
