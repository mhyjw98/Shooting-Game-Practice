using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public int enemyScore;
    public Sprite[] sprites;

    public float maxShotDelay;
    public float curShotDelay;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject player;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public ObjectManager objectManager;
    public GameManager gameManager;

    public AudioClip audioEnemyFire;
    public AudioClip audioDieEnemy;
    public AudioClip audioDieBoss;

    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    Animator anim;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enemyName == "B")
            anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        switch(enemyName)
        {
            case "B":
                health = 2000;
                Invoke("Stop", 1.5f);
                break;
            case "L":
                health = 50;
                break;
            case "M":
                health = 15;
                break;
            case "S":
                health = 3;
                break;
        }
    }

    void Update()
    {
        if (enemyName == "B")
            return;

        Fire();
        Reload();
    }

    void Stop()
    {
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 1.5f);
    }

    void Think()
    {
        if(!gameObject.activeSelf) 
            return;

        if(gameManager.isGameClear) 
            return;

        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }  
    }

    void FireFoward()
    {
        if (!gameManager.isGameClear)
        {
            // Fire 4 Bullet Foward
            GameObject bulletR = objectManager.MakeObj("BulletBossA");
            bulletR.transform.position = transform.position + Vector3.right * 0.65f + Vector3.down * 1.5f;
            GameObject bulletRR = objectManager.MakeObj("BulletBossA");
            bulletRR.transform.position = transform.position + Vector3.right * 0.8f + Vector3.down * 1.5f;
            GameObject bulletL = objectManager.MakeObj("BulletBossA");
            bulletL.transform.position = transform.position + Vector3.left * 0.65f + Vector3.down * 1.5f;
            GameObject bulletLL = objectManager.MakeObj("BulletBossA");
            bulletLL.transform.position = transform.position + Vector3.left * 0.8f + Vector3.down * 1.5f;

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

            rigidR.AddForce(Vector2.down * 6.5f, ForceMode2D.Impulse);
            rigidRR.AddForce(Vector2.down * 6.5f, ForceMode2D.Impulse);
            rigidL.AddForce(Vector2.down * 6.5f, ForceMode2D.Impulse);
            rigidLL.AddForce(Vector2.down * 6.5f, ForceMode2D.Impulse);

            // Pattern Counting
            curPatternCount++;

            audioSource.clip = audioEnemyFire;
            audioSource.Play();

            if (curPatternCount < maxPatternCount[patternIndex])
                Invoke("FireFoward", 2);
            else
                Invoke("Think", 3);
        }
        else
            return;
    }
    void FireShot()
    {
        for(int index=0;index<7;index++)
        {
            if (!gameManager.isGameClear)
            {
                GameObject bullet = objectManager.MakeObj("BulletEnemyB");
                bullet.transform.position = transform.position;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Vector2 dirVec = player.transform.position - transform.position;

                float x = -0.75f + index * 0.25f;
                float y = index * 0.15f;
                Vector2 ranVec = new Vector2(x, y);

                dirVec += ranVec;
                rigid.AddForce(dirVec.normalized * 5f, ForceMode2D.Impulse);
            }
            else 
                return;         
        }

        audioSource.clip = audioEnemyFire;
        audioSource.Play();

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 3.5f);
        else
            Invoke("Think", 3);
    }
    void FireArc()
    {
        for (int index = 0; index < 5; index++)
        {
            if (!gameManager.isGameClear)
            {
                GameObject bullet = objectManager.MakeObj("BulletEnemyA");
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.identity;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 8 * curPatternCount / maxPatternCount[patternIndex]), -1);
                rigid.AddForce(dirVec.normalized * 7, ForceMode2D.Impulse);

                audioSource.clip = audioEnemyFire;
                audioSource.Play();
            }
            else
                return;         
        }

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.15f);
        else
            Invoke("Think", 3);
    }
    void FireAround()
    {
        int roundNumA = 40;
        int roundNumB = 30;
        int roundNum = curPatternCount%2==0 ? roundNumA : roundNumB;

        for (int index = 0; index < roundNum; index++)
        {
            if (!gameManager.isGameClear)
            {
                GameObject bullet = objectManager.MakeObj("BulletBossB");
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.identity;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));
                rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

                Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
                bullet.transform.Rotate(rotVec);
            }
            else
                return;           
        }

        audioSource.clip = audioEnemyFire;
        audioSource.Play();

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 0.7f);
        else
            Invoke("Think", 3);
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if (enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position; 

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 1.5f, ForceMode2D.Impulse);
        }else if (enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);

            rigidR.AddForce(dirVecR.normalized * 3, ForceMode2D.Impulse); 
            rigidL.AddForce(dirVecL.normalized * 3, ForceMode2D.Impulse);
        }

        audioSource.clip = audioEnemyFire;
        audioSource.Play();

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
    public void OnHit(int dmg)
    {
        if (health <= 0)
            return;

        if (gameManager.playerComponent.isPlayerDie)
            return;
       

        health -= dmg;

        if (enemyName == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite", 0.1f);
        }       

        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            // Random Ratio Item Drop
            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);
            if(ran < 3)
            {
                Debug.Log("Not Item");
            }
            else if (ran < 7)
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if (ran < 9)
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
            }
            else if (ran < 10)
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            }

            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            gameManager.CallExplosion(transform.position, enemyName);

            // Boss Kill
            if (enemyName == "B")
            {
                audioSource.clip = audioDieBoss;
                audioSource.Play();
                gameManager.StageEnd();
            }
                
            else
            {
                audioSource.clip = audioDieEnemy;
                audioSource.Play();
            }
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
           
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }
}