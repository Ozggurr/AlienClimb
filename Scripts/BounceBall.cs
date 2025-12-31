using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BounceBall : MonoBehaviour
{
    [Header("Zıplatma Ayarları")]
    [Tooltip("Oyuncuya verilecek yukarı doğru anlık hız (m/sn). 8-15 arası mobil için güzel.")]
    public float bounceVelocity = 12f;

    [Tooltip("Aynı anda tekrarlı tetiklemeyi önlemek için bekleme süresi (sn).")]
    public float globalCooldown = 0.15f;

    [Header("Filtre")]
    [Tooltip("Sadece bu etiketli objeleri zıplat (örn. Player). Boşsa herkesi zıplatır.")]
    public string requiredTag = "Player";

    [Header("Görsel/Ses (opsiyonel)")]
    public AudioSource sfx;             // İstersen ses ver
    public Transform squashTarget;      // Topu hafif esnetmek için
    public float squashAmount = 0.15f;  // 0.1–0.2 iyi
    public float squashTime = 0.08f;

    private bool cooling;

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true; // otomatik işaretle
    }

    void OnTriggerEnter(Collider other)
    {
        if (cooling) return;
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;

        // Hedefte Rigidbody arıyoruz (karakter)
        Rigidbody rb = other.attachedRigidbody ? other.attachedRigidbody : other.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Yukarı doğru güvenli zıplatma:
        // - Aşağı hızlı düşüyorsa bile hissedilir bir zıplama ver
        // - Yatay hızını bozma
        Vector3 v = rb.velocity;
        if (v.y < 0f) v.y = 0f;                          // düşüş süratini sıfırla (daha tutarlı his)
        rb.velocity = new Vector3(v.x, v.y, v.z);
        rb.AddForce(Vector3.up * bounceVelocity, ForceMode.VelocityChange);

        // (İsteğe bağlı) animasyon/ses
        if (squashTarget != null) StartCoroutine(Squash());
        if (sfx != null) sfx.Play();

        // Küçük cooldown
        if (globalCooldown > 0f) StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        cooling = true;
        yield return new WaitForSeconds(globalCooldown);
        cooling = false;
    }

    IEnumerator Squash()
    {
        Vector3 baseScale = squashTarget.localScale;
        Vector3 squashed = new Vector3(baseScale.x + squashAmount, baseScale.y - squashAmount, baseScale.z + squashAmount);
        float t = 0f;

        // Sıkıştır
        while (t < squashTime)
        {
            t += Time.deltaTime;
            float k = t / squashTime;
            squashTarget.localScale = Vector3.Lerp(baseScale, squashed, k);
            yield return null;
        }

        // Geri aç
        t = 0f;
        while (t < squashTime)
        {
            t += Time.deltaTime;
            float k = t / squashTime;
            squashTarget.localScale = Vector3.Lerp(squashed, baseScale, k);
            yield return null;
        }
        squashTarget.localScale = baseScale;
    }
}