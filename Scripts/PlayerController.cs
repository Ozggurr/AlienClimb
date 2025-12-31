using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private bool loseShown = false;
    [Header("Hareket Ayarlari")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Bilesenler")]
    public Rigidbody rb;
    public Joystick joystick;
    public Button jumpButton;

    private int jumpCount = 0;
    public int maxJumpCount = 2;

    [Header("Respawn Sistemi")]
    public Transform respawnPoint;         // Son checkpoint
    private Vector3 initialSpawnPoint;     // İlk doğma noktası
    private float lastGroundY;             // Son temas edilen Ground yüksekliği
    public float fallDistanceToDie = 10f;  // 10 birim düşünce respawn

    // ---------- ANIM ENTEGRASYONU ----------
    private Animator anim;
    [Header("Animasyon Ayarlari")]
    [Tooltip("Speed parametresine giden degeri yumuşatmak için")]
    public float speedLerp = 12f;
    private static readonly int HashSpeed = Animator.StringToHash("Speed");
    private static readonly int HashIsJumping = Animator.StringToHash("IsJumping");
    
    // ---------------------------------------
    public Transform fallbackRespawn; // (Inspector’dan Respawn objeni sürükle)
    // ---------- JUMP KONTROL (YENİ) ----------
    [Header("Jump Kontrol")]
    [Tooltip("Düşerken double jump'a izin ver (false = düşüşte zıplama kapalı)")]
    [SerializeField] private bool allowDoubleJumpWhileFalling = false;

    [Tooltip("Bundan daha negatif Y-hızı varsa 'düşüyor' say")]
    [SerializeField] private float fallingThreshold = -0.05f;

    [Tooltip("Air jump'a izin vermek için min yukarı hız (0 = sadece düşmüyor olsun)")]
    [SerializeField] private float airJumpMinUpVel = 0.0f;

    private bool isGrounded = false;
    // -----------------------------------------

    void Start()
    {
        // Respawn objesini otomatik bul (atamadıysan)
        if (fallbackRespawn == null)
        {
            var go = GameObject.FindWithTag("Respawn");
            if (go) fallbackRespawn = go.transform;
        }
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (joystick == null)
            joystick = FindObjectOfType<Joystick>();

        if (jumpButton == null)
            jumpButton = FindObjectOfType<Button>();

        if (jumpButton != null)
            jumpButton.onClick.AddListener(Jump);

        // Başlangıç konumunu ve respawn noktasını kaydet
        initialSpawnPoint = transform.position;
        if (respawnPoint == null)
            respawnPoint = transform;

        // Başlangıç zemini Y değeri
        lastGroundY = transform.position.y;

        // Animator
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.LogWarning("[PlayerController] Animator bulunamadi! Karakterine Animator component ekle ve Controller bagla.");
        else
        {
            anim.SetBool(HashIsJumping, false);
            anim.SetFloat(HashSpeed, 0f);
        }

        Debug.Log("[PlayerController] Başlangıç noktası kaydedildi: " + initialSpawnPoint);
    }

    void Update()
    {
        HandleMovement();

        // Göreceli düşüş kontrolü
        // YENI: önce Lose panelini aç, respawn etme
        if (!loseShown && transform.position.y < lastGroundY - fallDistanceToDie)
        {
            Debug.Log("[PlayerController] 10 birimden fazla düşüldü. Lose panel açılıyor.");
            loseShown = true;

            // Lose paneli varsa onu göster, yoksa güvenli geri dönüş olarak Respawn yap
            if (LosePanelController.Instance != null)
            {
                LosePanelController.Instance.ShowLose();
            }
            else
            {
                Debug.LogWarning("LosePanelController.Instance bulunamadı, dogrudan Respawn ediliyor.");
                Respawn();
                loseShown = false; // panel olmadıgı icin bayragi geri bırak
            }
        }
        // Animator Speed güncelle
        if (anim != null)
        {
            Vector3 planarVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            float targetSpeed = planarVel.magnitude / Mathf.Max(0.0001f, moveSpeed); // ~0..1
            float current = anim.GetFloat(HashSpeed);
            float smoothed = Mathf.Lerp(current, targetSpeed, Time.deltaTime * speedLerp);
            anim.SetFloat(HashSpeed, smoothed);
        }
    }

    void HandleMovement()
    {
        if (joystick == null || Camera.main == null) return;

        Vector3 inputDirection = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;
            moveDirection.Normalize();

            rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
            transform.forward = moveDirection;
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    public void Jump()
    {
        if (rb == null) return;

        bool falling = rb.velocity.y < fallingThreshold;

        // 1) Yerdeyken her zaman zıpla
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount = 1;        // yer zıplaması
            isGrounded = false;   // artık havadayız
            AudioManager.Instance.PlaySfx(SfxType.Jump); //ZIPLAMA SESİ 
            if (anim) anim.SetBool(HashIsJumping, true);
            return;
        }

        // 2) Havada: yalnızca 1 air-jump (maxJumpCount=2 ise)
        if (jumpCount < maxJumpCount)
        {
            // Düşerken air jump'ı engelle (flag ile açılabilir)
            if (!falling || allowDoubleJumpWhileFalling)
            {
                // (opsiyonel) en azından düşmüyor/yukarı gidiyor olsun
                if (rb.velocity.y >= airJumpMinUpVel || allowDoubleJumpWhileFalling)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    jumpCount++;
                    AudioManager.Instance.PlaySfx(SfxType.Jump); //İKİNCİ ZIPLAMA SESİ 
                    if (anim) anim.SetBool(HashIsJumping, true);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
            isGrounded = true; // <<< eklendi
            lastGroundY = transform.position.y; // Yeni zemin yüksekliğini kaydet
            Debug.Log("[PlayerController] Ground'a temas edildi. lastGroundY = " + lastGroundY);

            if (anim) anim.SetBool(HashIsJumping, false);
        }
    }

    // Yüzeyde kaldığın sürece grounded kalsın (eğimli yüzeylerde güvenilirlik artar)
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    public void Respawn()
    {
        AudioManager.Instance.PlaySfx(SfxType.Lose);

        rb.velocity = Vector3.zero;

        // 1) Checkpoint alındıysa (respawnPoint bu durumda checkpoint olur)
        // 2) değilse fallbackRespawn (tag=Respawn) 
        // 3) o da yoksa initialSpawnPoint
        Transform target =
            (respawnPoint != null && respawnPoint != transform) ? respawnPoint :
            (fallbackRespawn != null ? fallbackRespawn : null);

        if (target != null)
            transform.position = target.position;
        else
            transform.position = initialSpawnPoint;

        lastGroundY = transform.position.y;

        if (anim != null)
        {
            anim.SetBool(HashIsJumping, false);
            anim.SetFloat(HashSpeed, 0f);
        }

        isGrounded = false;
        jumpCount = 0;
    }
    public void ResetLoseFlag()
    {
        loseShown = false;
    }

}