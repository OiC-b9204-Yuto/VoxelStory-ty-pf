using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int currentHealth = 5;
    public int CurrentHealth { get { return currentHealth; } }
    [SerializeField] private int maxHealth = 5;
    public int MaxHealth { get { return maxHealth; } }
    private float invincible = 0.2f;
    private float invincibleTimer = 0.2f;

    private Material material;

    Color defaultColor;

    [SerializeField]GameObject heart;

    Vector3 startPos;
    Vector3 startRotation;

    bool move = false;

    [SerializeField] int moveIndex = 0;
    [SerializeField] int beforeMoveIndex = -1;

    [SerializeField] Vector3[] movePos;
    [SerializeField] float speed;

    Vector3 checkSize = new Vector3(20,10);
    [SerializeField]LayerMask layerMask;

    [SerializeField]int score = 3000;

    void Start()
    {
        layerMask = LayerMask.GetMask("Player");
        startPos = transform.position;
        startRotation = transform.eulerAngles;
        material = transform.Find("robot_6_body").GetComponent<Renderer>().material;
        defaultColor = material.color;
    }

    private void Update()
    {
        move = Physics.CheckBox(transform.position, checkSize, Quaternion.identity, layerMask) ||
            Physics.CheckBox(startPos, checkSize, Quaternion.identity, layerMask);

        if (!move)
        {
            transform.position = startPos;
            transform.eulerAngles = startRotation;
            moveIndex = 0;
            beforeMoveIndex = -1;
            return;
        }

        if (beforeMoveIndex != moveIndex)
        {
            StartCoroutine(Move());
        }
        beforeMoveIndex = moveIndex;
    }

    IEnumerator Move()
    {
        while (true)
        {
            if (!move)
            {
                yield break;
            }

            float distance = Vector3.Distance(transform.position, movePos[moveIndex]);
            if (distance < 0.1f)
            {
                StartCoroutine(Next());
                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, movePos[moveIndex], speed * Time.deltaTime);
            yield return null;

        }
    }


    IEnumerator Next()
    {
        if (!move)
        {
            yield break;
        }

        moveIndex++;
        if (moveIndex >= movePos.Length)
        {
            moveIndex = 0;
        }
        StartCoroutine(Rotation());

        yield return null;
    }

    IEnumerator Rotation()
    {
        if (!move)
        {
            yield break;
        }

        var currentRotation = transform.localRotation;
        Quaternion newRotation;

        if (movePos[moveIndex].x - this.transform.position.x > 0)
        {
            newRotation = currentRotation * Quaternion.AngleAxis(180, Vector3.up);
        }
        else
        {
            newRotation = currentRotation * Quaternion.AngleAxis(-180, Vector3.up);
        }
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
            {
                Quaternion rotation = Quaternion.Lerp(currentRotation, newRotation, t * 2);
                transform.localRotation = rotation;
                yield return null;
            }
            //ちょうど90度刻みになるように調整
            transform.localRotation = newRotation;

        yield return null;

    }

    void FixedUpdate()
    {
        if (GameManager.instance)
        {
            if (!GameManager.instance.GetStart || GameManager.instance.GetPause)
            {
                return;
            }
        }

        if (!move)
        {
            return;
        }


        if (currentHealth == 0)
        {
            Color color = material.color;
            color.a -= 1.0f/ 1 * Time.deltaTime;
            if (color.a <= 0)
            {
                Destroy(this.gameObject);
            }
            material.color = color;
            return;
        }

        if (invincibleTimer < invincible)
        {
            invincibleTimer += Time.fixedDeltaTime;
            Color color = material.color;
            color.a = Mathf.Sin(invincibleTimer * 10) / 2 + 0.5f;
            material.color = color;
        }
        else
        {
            material.color = defaultColor;
        }
    }

    public void TakeDamage(int damage)
    {
        if (GameManager.instance)
        {
            if (!GameManager.instance.GetStart || GameManager.instance.GetPause)
            {
                return;
            }
        }

        if (currentHealth == 0)
        {
            return;
        }

        if (damage > 0 && invincibleTimer >= invincible)
        {
            for (int i = 0; i < damage; i++)
            {
                GameObject gameObject = Instantiate(heart, transform.position + new Vector3(i * 0.1f, 4, 0), Quaternion.identity,GameObject.Find("DamageUI").transform);
            }

            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                this.GetComponent<Collider>().enabled = false;
                currentHealth = 0;
                GameManager.instance.AddScore(score);
            }
            invincibleTimer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().TakeDamage(1);
            GetComponent<Animator>().SetTrigger("Attack");
        }
    }
}
