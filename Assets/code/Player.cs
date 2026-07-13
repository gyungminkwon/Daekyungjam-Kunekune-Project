using UnityEngine;

public class Player : MonoBehaviour
{
    // =========================
    // 이동 관련 변수
    // =========================

    // 기본 이동 속도
    public float moveSpeed = 5f;

    // 달리기 배율
    public float runMultiplier = 2f;

    // 웅크리기 배율
    public float crouchMultiplier = 0.5f;


    // 점프 힘
    public float jumpForce = 8f;


    private Rigidbody2D rb;

    private bool isGrounded = false;



    // =========================
    // 모습 오브젝트
    // =========================

    // 기본 직사각형
    public GameObject standObject;


    // 웅크리기 직사각형
    public GameObject crouchObject;


    // 달리기 직사각형
    public GameObject runObject;




    // =========================
    // 히트박스
    // =========================

    private BoxCollider2D boxCollider;


    private Vector2 originalSize;
    private Vector2 originalOffset;


    // 웅크리기 히트박스
    public Vector2 crouchSize = new Vector2(1, 0.5f);
    public Vector2 crouchOffset = new Vector2(0, -0.25f);




    // =========================
    // 시작
    // =========================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        boxCollider = GetComponent<BoxCollider2D>();


        originalSize = boxCollider.size;
        originalOffset = boxCollider.offset;


        standObject.SetActive(true);
        crouchObject.SetActive(false);
        runObject.SetActive(false);
    }




    void Update()
    {
        Move();
        Jump();
        ChangeState();
    }





    // =========================
    // 이동
    // =========================

    void Move()
    {
        float move = 0;


        if(Input.GetKey(KeyCode.A))
        {
            move = -1;
        }


        if(Input.GetKey(KeyCode.D))
        {
            move = 1;
        }



        float currentSpeed = moveSpeed;



        // 달리기
        if(Input.GetKey(KeyCode.LeftControl) ||
           Input.GetKey(KeyCode.RightControl))
        {
            currentSpeed *= runMultiplier;
        }



        // 웅크리기
        if(Input.GetKey(KeyCode.S))
        {
            currentSpeed *= crouchMultiplier;
        }



        rb.linearVelocity =
            new Vector2(move * currentSpeed, rb.linearVelocity.y);
    }







    // =========================
    // 점프
    // =========================

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity =
                new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }







    // =========================
    // 상태 변경
    // =========================

    void ChangeState()
    {

        // =================
        // 웅크리기
        // =================
        if(Input.GetKey(KeyCode.S))
        {
            standObject.SetActive(false);
            runObject.SetActive(false);
            crouchObject.SetActive(true);



            // 히트박스 감소
            boxCollider.size = crouchSize;
            boxCollider.offset = crouchOffset;
        }



        // =================
        // 달리기
        // =================
        else if(Input.GetKey(KeyCode.LeftControl) ||
                Input.GetKey(KeyCode.RightControl))
        {
            standObject.SetActive(false);
            crouchObject.SetActive(false);
            runObject.SetActive(true);



            // 히트박스 원상복구
            boxCollider.size = originalSize;
            boxCollider.offset = originalOffset;
        }



        // =================
        // 기본
        // =================
        else
        {
            standObject.SetActive(true);
            crouchObject.SetActive(false);
            runObject.SetActive(false);



            boxCollider.size = originalSize;
            boxCollider.offset = originalOffset;
        }

    }






    // =========================
    // 바닥 체크
    // =========================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Ground")
        {
            isGrounded = true;
        }
    }



    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Ground")
        {
            isGrounded = false;
        }
    }
}