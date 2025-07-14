using UnityEngine;
using System.Collections;

public class PineNut : MonoBehaviour
{
    public int pointValue = 1;
    [SerializeField] private ParticleSystem collectParticles;
    [SerializeField] private AudioClip collectSound;

    [Header("音效设置")]
    [Tooltip("音效音量")] [Range(0f, 1f)] public float volume = 1f; // 添加音量控制

    private Collider nutCollider;
    private MeshRenderer nutRenderer;
    private AudioSource audioSource;
    private bool isCollected = false;

    void Start()
    {
        nutCollider = GetComponent<Collider>();
        nutRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();

        // 确保AudioSource正确配置
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = volume; // 应用音量设置
        }

        if (collectParticles == null)
            collectParticles = GetComponentInChildren<ParticleSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            StartCoroutine(CollectSequence());
        }
    }

    IEnumerator CollectSequence()
    {
        // 播放收集效果
        PlayCollectEffect();

        // 通知GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.CollectPineNut(pointValue);
        }
        else
        {
            Debug.LogError("GameManager实例不存在!");
        }

        // 隐藏松子（但保持脚本运行）
        if (nutCollider) nutCollider.enabled = false;
        if (nutRenderer) nutRenderer.enabled = false;

        // 等待特效完成
        float delay = CalculateEffectDuration();
        yield return new WaitForSeconds(delay);

        // 销毁松子对象
        Destroy(gameObject);
    }

    void PlayCollectEffect()
    {
        // 粒子效果
        PlayParticleEffect();

        // 音效
        PlayCollectSound();
    }

    void PlayParticleEffect()
    {
        if (collectParticles != null)
        {
            collectParticles.transform.parent = null;
            collectParticles.Play();
            Destroy(collectParticles.gameObject, collectParticles.main.duration);
        }
    }

    void PlayCollectSound()
    {
        if (collectSound == null)
        {
            Debug.LogWarning("松子缺少收集音效!");
            return;
        }

        // 使用AudioSource播放音效（带音量控制）
        if (audioSource != null)
        {
            audioSource.volume = volume; // 确保应用当前音量
            audioSource.PlayOneShot(collectSound, volume);
        }
        else
        {
            // 如果没有AudioSource，使用备用方法播放
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
        }
    }

    float CalculateEffectDuration()
    {
        float maxDuration = 1.0f;

        if (collectParticles != null)
        {
            float particleDuration = collectParticles.main.duration + collectParticles.main.startLifetime.constantMax;
            maxDuration = Mathf.Max(maxDuration, particleDuration);
        }

        if (collectSound != null)
        {
            float soundDuration = collectSound.length;
            maxDuration = Mathf.Max(maxDuration, soundDuration);
        }

        return maxDuration + 0.1f; // 额外0.1秒缓冲
    }

    // 在编辑器中测试音效
    [ContextMenu("测试音效播放")]
    public void TestSoundPlay()
    {
        if (collectSound == null)
        {
            Debug.LogError("未分配音效剪辑!");
            return;
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(collectSound, volume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
        }
    }
}