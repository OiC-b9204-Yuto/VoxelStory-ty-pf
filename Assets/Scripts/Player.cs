using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField] private int currentHealth = 5;
    public int CurrentHealth { get{ return currentHealth; } }
    [SerializeField] private int maxHealth = 5;
    public int MaxHealth { get { return maxHealth; } }
    [SerializeField] private float invincible = 2.0f;
    private float invincibleTimer = 2;

    [SerializeField] Collider weaponTrigger;
    [SerializeField] private int damage = 1;

    [SerializeField] private Transform topCenter;
    [SerializeField] private Transform bottomCenter;

    private Material material;

    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float multiplier = 10;

    [SerializeField] private float jumpPower = 7;
    [SerializeField] private float riseGravity = 30;
    [SerializeField] private float fallGravity = 60;

    private bool jumpFlag = false;
    private bool onGround = true;

    private float rayDistance = 0.7f;

    private Vector3 checkBoxNormal = new Vector3(0.5f, 0.05f, 0.5f);
    private Vector3 checkBoxSmall = new Vector3(0.4f, 0.05f, 0.4f);

    [SerializeField] private LayerMask layerMask;

    private Rigidbody rigidbody;
    private Animator animator;

    private Vector3 velocity;
    private Vector3 moveForce;
    private Vector3 moveDirection;
    //移動キー用変数
    private float inputDirection;


    [SerializeField] private float gravityForce;
    [SerializeField] private float minGravityForce = -20;
    private float jumpTime;
    [SerializeField] private int jumpCount;
    [SerializeField] private int maxJump = 1;
    private float maxJumpTime = 0.2f;

    private bool attack = false;

    Color defaultColor;

    Vector3 pos;

    [SerializeField] private Image[] images;

    void Awake()
    {
        layerMask = 1;
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        material = transform.Find("Full").GetComponent<Renderer>().material;
        defaultColor = material.color;
    }

    void Update()
    {
        if (GameManager.instance)
        {
            if (GameManager.instance.gameStatus > 0)
            {
                inputDirection = 0;
                moveDirection = Vector3.zero;
                return;
            }

            if (!GameManager.instance.GetStart || GameManager.instance.GetPause)
            {
                return;
            }
        }

        if (currentHealth == 0)
        {
            return;
        }

        //攻撃
        var clip = animator.GetCurrentAnimatorClipInfo(0);
        string clipName = "";
        if (clip.Length > 0)
        {
            clipName = clip[0].clip.name;
        }

        if (clipName == "Attack3")
        {
            attack = true;
            if (Input.GetButtonDown("Attack"))
            {
                animator.SetBool("Attack3", false);
                animator.SetBool("Attack2", true);
            }
        }
        else if (clipName == "Attack2")
        {
            attack = true;
            if (Input.GetButtonDown("Attack"))
            {
                animator.ResetTrigger("Attack1");
                animator.SetBool("Attack2", false);
                animator.SetBool("Attack3", true);
            }
        }
        else if (clipName == "Attack1")
        {
            attack = true;
            if (Input.GetButtonDown("Attack"))
            {
                animator.SetBool("Attack2", true);
            }
        }
        else
        {
            attack = false;
            if (Input.GetButtonDown("Attack"))
            {
                animator.SetTrigger("Attack1");
            }
            else
            {
                animator.SetBool("Attack2", false);
                animator.SetBool("Attack3", false);
            }
        }
        
        //移動
        inputDirection = Input.GetAxisRaw("Horizontal");
        if (inputDirection != 0)
        {
            animator.SetBool("Run",true);
        }
        else
        {
            animator.SetBool("Run", false);
        }

        if (jumpCount < maxJump) {
            if (jumpTime < maxJumpTime)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    jumpFlag = true;
                    animator.SetBool("Jump", true);
                }
                if (jumpFlag && Input.GetButtonUp("Jump"))
                {
                    jumpFlag = false;
                    jumpCount++;
                }
            }
            else if(jumpFlag)
            {
                jumpFlag = false;
                jumpCount++;
            }
        }

        if (!jumpFlag)
        {
            onGround = IsGrounded();
        }
        else
        {
            onGround = false;
        }

        //移動方向の取得
        moveDirection = new Vector3(inputDirection, 0, 0);
        RaycastHit raycastHit;
        //Ray飛ばして坂道などに当たった場合は移動方向
        if (onGround)
        {
            jumpCount = 0;
            if (Physics.Raycast(this.bottomCenter.position, moveDirection, out raycastHit, rayDistance, layerMask) || Physics.Raycast(this.bottomCenter.position, Vector3.down, out raycastHit, rayDistance, layerMask))
            {
                float dot = Vector3.Dot(moveDirection, raycastHit.normal);
                if (dot <= 0.71f && dot >= -0.71f)
                {
                    moveDirection = moveDirection - dot * raycastHit.normal;
                }
            }
            animator.SetBool("Jump", false);
        }
        else
        {
            animator.SetBool("Jump", true);
            if (gravityForce == minGravityForce)
            {
                if (transform.position == pos)
                {
                    pos = transform.position;
                    pos.y += 0.1f;
                    transform.position = pos;
                }
                else
                {
                    pos = transform.position;
                }

            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.instance)
        {
            if (/*!GameManager.instance.GetStart ||*/ GameManager.instance.GetPause)
            {
                return;
            }
        }

        /*
        if (currentHealth == 0)
        {
            rigidbody.velocity *= 0.9f;
            return;
        }
        */

        weaponTrigger.enabled = attack;

        if (invincibleTimer < invincible)
        {
            invincibleTimer += Time.fixedDeltaTime;
            Color color = material.color;
            color.a = Mathf.Sin(invincibleTimer * 20) / 2 + 0.5f;

            transform.Find("Full").GetComponent<Renderer>().material.color = color;
            weaponTrigger.transform.GetChild(0).GetComponent<Renderer>().material.color = color;

        }
        else
        {
            material.color = defaultColor;
            weaponTrigger.transform.GetChild(0).GetComponent<Renderer>().material.color = defaultColor;
        }

        if (onGround)
        {
            gravityForce *= 0.1f;
        }
        else
        {
            //上昇時落下時 によって重力を変える
            if (gravityForce > 0)
            {
                gravityForce -= riseGravity * Time.fixedDeltaTime;
            }
            else
            {
                gravityForce -= fallGravity * Time.fixedDeltaTime;
                if (gravityForce < minGravityForce)
                {
                    gravityForce = minGravityForce;
                }
            }
        }

        //ジャンプ中処理
        if (jumpFlag)
        {
            jumpTime += Time.fixedDeltaTime;
            gravityForce *= 0.4f;
            gravityForce += jumpPower;
        }
        else
        {
            jumpTime = 0;
        }

        //速度　攻撃時と非攻撃時で変化
        if (attack)
        {
            velocity = moveDirection * moveSpeed * 0.5f;
        }
        else
        {
            velocity = moveDirection * moveSpeed;
        }

        //移動力
        moveForce *= 0.9f;
        moveForce += multiplier * velocity * Time.fixedDeltaTime;

        if (!onGround)
        {
            if (Physics.Raycast(bottomCenter.position, moveDirection, 0.6f, layerMask) || Physics.Raycast(topCenter.position, moveDirection, 0.6f, layerMask))
            {
                moveForce = new Vector3(0, moveForce.y);
                rigidbody.velocity = new Vector3(rigidbody.velocity.x * 0.8f, gravityForce, 0);
            }
            else
            {
                rigidbody.velocity = new Vector3(0, gravityForce, 0);
                rigidbody.velocity += moveForce;
            }
        }
        else
        {
            rigidbody.velocity = new Vector3(0, gravityForce, 0);
            rigidbody.velocity += moveForce;
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(bottomCenter.position, Vector3.right, out hit, 0.7f, layerMask) || Physics.Raycast(topCenter.position, Vector3.right, out hit, 0.7f, layerMask) ||
            Physics.Raycast(bottomCenter.position, Vector3.left, out hit, 0.7f, layerMask) || Physics.Raycast(topCenter.position, Vector3.left, out hit, 0.7f, layerMask))
        {
            float dot = Vector3.Dot(Vector3.right, hit.normal);
            if (dot >= 0.72f || dot <= -0.72f)
            {
                return Physics.CheckBox(bottomCenter.position, checkBoxSmall, Quaternion.identity, layerMask);
            }
        }
        return Physics.CheckBox(bottomCenter.transform.position, checkBoxNormal, Quaternion.identity, layerMask);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth == 0)
        {
            return;
        }

        if (invincibleTimer >= invincible)
        {
            invincibleTimer = 0;
            currentHealth -= damage;

            for (int i = maxHealth - 1; i >= currentHealth  && i >= 0; i--)
            {
                images[i].color = Color.black;
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.SetBool("Death", true);
                weaponTrigger.enabled = false;
                jumpFlag = false;
                StartCoroutine(AnimationEnd("Death"));
            }
        }
    }

    IEnumerator AnimationEnd(string name)
    {
        while (true) {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == name)
            {
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                GameManager.instance.GameOver();
                yield break;
            }
            yield return null;
        }
    }

    public void healthRecovery(int num)
    {
        currentHealth = Mathf.Min(currentHealth + num, maxHealth);

        for (int i = 0; i < currentHealth; i++)
        {
            images[i].color = Color.white;
        }
    }


    public void WeaponTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bottomCenter.transform.position, checkBoxNormal * 2);
    }
#endif
}
