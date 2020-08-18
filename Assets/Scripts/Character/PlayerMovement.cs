using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    void OnGUI()
    {
        if (!photonView.IsMine) return;
        GUI.Label(new Rect(10, 10, 150, 150), $"Speed: X: {Mathf.Round(Input.GetAxis("Horizontal"))} --- Y: {Mathf.Round(Input.GetAxis("Vertical"))}");
        GUI.Label(new Rect(10, 30, 150, 150), $"Position: X: {photonView.transform.position.x} --- Y: {photonView.transform.position.y}");
        GUI.Label(new Rect(10, 80, 150, 150), "Username: " + photonView.Owner.NickName);
    }

    public Transform playerInfo;
    public GameObject ownCamera;

    private Renderer myRenderer;
    private Animator myAnimator;
    private Rigidbody2D myRigbody;
    private PlayerManager playerInventory;

    Vector2 movement;

    private int sortingOrderBase = 5000;
    public float moveSpeed = 5f;

    private bool m_FacingRight = true;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        myAnimator = GetComponent<Animator>();
        myRigbody = GetComponent<Rigidbody2D>();
        playerInventory = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        ownCamera.SetActive(photonView.IsMine);
        playerInventory.StartCoroutine(playerInventory.LoadInventory());
    }

    private void Update()
    {
        myRenderer.sortingOrder = (int)(transform.position.y * -sortingOrderBase);

        if (!photonView.IsMine) return;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        Move();
    }

    private void Move()
    {
        myRigbody.MovePosition(myRigbody.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

        myAnimator.SetFloat("MoveSpeed", Mathf.Abs(movement.x) + Mathf.Abs(movement.y));

        if (movement.x > 0 && !m_FacingRight)
        {
            //Flip();
            //playerInfo.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (movement.x < 0 && m_FacingRight)
        {
            //Flip();
            //playerInfo.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}