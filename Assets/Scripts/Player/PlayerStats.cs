using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerStats : MonoBehaviourPunCallbacks
{
    [Header("Player Info")]
    public Canvas playerInfo;
    public Text cvUsername;

    [Header("Sliders")]
    public Slider hpSlider;
    public Slider plyInfoHPSlider;
    public Slider armorSlider;
    public Slider manaSlider;

    [Header("Gameobjects")]
    public GameObject playerWeapon;
    public GameObject gravestone;
    public GameObject deathScreenUI;

    private SpriteRenderer spriteRenderer;
    private Animator myAnimator;
    private BoxCollider2D playerCollider;
    private PlayerMovement playerMovement;

    private float timeForUI = 3f;
    private float speed = 3f;
    private bool antiLoop = true;

    private SpawnPoints spawnPoints;

    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    [SerializeField] private int maxArmor = 100;
    public int currentArmor;

    [SerializeField] private int maxMana = 100;
    public int currentMana;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();
        playerMovement = GetComponent<PlayerMovement>();

        spawnPoints = GameObject.Find("Map").GetComponent<SpawnPoints>();
    }

    private void Start()
    {
        cvUsername.text = photonView.Owner.NickName;

        SetMaxHealth(100);
        SetHealth(60);

        SetMaxArmor(100);
        SetArmor(10);

        SetMaxMana(100);
        SetMana(60);
    }

    private void Update()
    {
        SetHealth(currentHealth);
        SetArmor(currentArmor);

        // Check if player's health is zero
        if(currentHealth <= 0)
        {
            timeForUI -= Time.deltaTime;

            Vector3 gravePosition = gravestone.transform.localPosition;
            if (gravePosition.y > 0)
            {
                gravePosition.y -= (speed * Time.deltaTime);
                gravestone.transform.localPosition = gravePosition;
            }
            else if (gravePosition.y <= 0 && antiLoop)
            {
                myAnimator.SetTrigger("PlayerIsDeath");
                antiLoop = false;
            }
        }

        if (timeForUI <= 0.0f)
        {
            deathScreenUI.SetActive(true);
        }
    }

    public void SetMaxHealth(int health)
    {
        hpSlider.maxValue = health;
        plyInfoHPSlider.maxValue = health;
        maxHealth = health;
    }

    public void SetHealth(int health)
    {
        currentHealth = health;
        hpSlider.value = health;
        plyInfoHPSlider.value = health;
    }

    public void SetMaxArmor(int armor)
    {
        armorSlider.maxValue = armor;
        maxArmor = armor;
    }

    public void SetArmor(int armor)
    {
        currentArmor = armor;
        armorSlider.value = armor;
    }

    public void SetMaxMana(int mana)
    {
        manaSlider.maxValue = mana;
        maxMana = mana;
    }

    public void SetMana(int mana)
    {
        currentMana = mana;
        manaSlider.value = mana;
    }

    public void RespawnPlayer()
    {
        int posCount = Random.Range(0, spawnPoints.spawnPos.Length);
        transform.position = spawnPoints.spawnPos[posCount];
        SetHealth(60);
        SetArmor(100);
        SetMana(60);
        timeForUI = 3f;
        gravestone.SetActive(false);
        deathScreenUI.SetActive(false);
        spriteRenderer.enabled = true;
        playerCollider.enabled = true;
        playerInfo.enabled = true;
        playerMovement.enabled = true;
        playerWeapon.SetActive(true);
        Debug.Log("Respawn...");
    }


    //[PunRPC]
    /*private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            stream.SendNext(currentArmor);
        }
        else if (stream.IsReading)
        {
            currentHealth = (int)stream.ReceiveNext();
            currentArmor = (int)stream.ReceiveNext();
        }
    }*/

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if(currentArmor >= 0)
        {
            if(damage <= currentArmor)
            {
                currentArmor -= damage;
            }
            else
            {
                damage -= currentArmor;
                currentArmor = 0;
                currentHealth -= damage;
            }
        }
        else
        {
            currentHealth -= damage;

            Debug.Log(transform.name + " lost " + damage + " health");
        }

        if (currentHealth <= 0)
        {
            gravestone.SetActive(true);
            spriteRenderer.enabled = false;
            playerCollider.enabled = false;
            playerInfo.enabled = false;
            playerMovement.enabled = false;
            playerWeapon.SetActive(false);
        }
    }
}
