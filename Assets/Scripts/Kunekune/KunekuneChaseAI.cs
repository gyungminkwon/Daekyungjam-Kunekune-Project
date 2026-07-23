using UnityEngine;
using System.Collections;

public class KunekuneChaseAI : MonoBehaviour
{
    [Header("추격전 설정")]
    public Transform player;
    public float baseSpeed = 4f;
    public float heatSpeed = 16f;
    
    [Header("상태")]
    public bool isPlayerInHeat = false;
    private bool isChasing = false;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private float startY;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
        }
    }

    public void StartChaseFromProp(KunekuneProp prop)
    {
        gameObject.SetActive(true);
        StartCoroutine(IntroSequenceRoutine(prop));
    }

    private IEnumerator IntroSequenceRoutine(KunekuneProp prop)
    {
        isChasing = false;

        Vector2 groundPos = GetGroundPosition(prop.transform.position);
        transform.position = groundPos;
        startY = transform.position.y;

        prop.gameObject.SetActive(false);

        anim.Play(prop.transformAnimName);

        yield return new WaitForSeconds(prop.transformDuration);

        StartLinearChase(transform.position);
    }

    public void StartLinearChase(Vector2 spawnPos)
    {
        gameObject.SetActive(true);
        isChasing = true;
        
        Vector2 groundPos = GetGroundPosition(spawnPos);
        transform.position = groundPos;
        startY = transform.position.y;

        anim.Play("kunekune_move");
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (!isChasing) return;

        // spriteRenderer.flipX = player.position.x < transform.position.x;

        float currentSpeed = isPlayerInHeat ? heatSpeed : baseSpeed;

        anim.speed = isPlayerInHeat ? 1.2f : 0.75f;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        float newX = transform.position.x + (direction * currentSpeed * Time.deltaTime);
        
        transform.position = new Vector2(newX, transform.position.y);
    }

    private Vector2 GetGroundPosition(Vector2 targetPos)
    {
        Vector2 rayStartPos = targetPos + Vector2.up * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(rayStartPos, Vector2.down, 10f, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            return new Vector2(targetPos.x, hit.point.y);
        }
        return targetPos;
    }

    public void StopChase()
    {
        isChasing = false;
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ChaseSceneManager.Instance != null)
            {
                ChaseSceneManager.Instance.GameOver(ChaseSceneManager.DeathCause.Kunekune);
            }
        }
    }
}