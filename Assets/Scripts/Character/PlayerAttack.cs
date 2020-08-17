using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerAttack : MonoBehaviourPunCallbacks
{
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    private bool canAttack;
    private float nextAttack;

    private Camera playerCamera;
    private Animator myAnimator;

    private Transform attackPoint;
    private Transform weaponIK;

    private void Awake()
    {
        playerCamera = transform.Find("PlayerCamera").GetComponent<Camera>();
        myAnimator = transform.GetComponent<Animator>();

        attackPoint = transform.Find("Weapons/Weapon_Sword_Gold/Weapon_IK_Animation/AttackPoint");

        weaponIK = transform.Find("Weapons/Weapon_Sword_Gold");
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        Vector2 dir = playerCamera.ScreenToWorldPoint(Input.mousePosition) - weaponIK.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + 45, Vector3.forward);
        weaponIK.rotation = rotation;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!canAttack) return;
            //MeleeAttack();
            myAnimator.SetTrigger("MeleeAttack");
        }

        if (!canAttack)
        {
            nextAttack -= Time.deltaTime;

            if(nextAttack <= Time.deltaTime)
            {
                canAttack = true;
            }
        }
    }

    public void MeleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            PhotonView pView = enemy.GetComponent<PhotonView>();

            //if (pView.IsMine) return;

            if (pView)
            {
                StartCoroutine(WaitTimer(0.1f, pView));
                canAttack = false;
                nextAttack = 0.3f;
            }
        }
    }

    private IEnumerator WaitTimer(float waitTime, PhotonView pView)
    {
        yield return new WaitForSeconds(waitTime);
        pView.RPC("TakeDamage", RpcTarget.All, 25);
        Debug.Log("We hit " + pView.Owner.NickName);
    }
}