using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public ObjectManager objectManager;

    public AudioClip audioFollowerFire;

    public Vector3 followPos;
    public int followerDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;

    AudioSource audioSource;

    private void Awake()
    {
        parentPos = new Queue<Vector3>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Watch()
    {
        // Queue = FIFO 선입 선출
        if (!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);

        if (parentPos.Count > followerDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followerDelay)
            followPos = parent.position;
    }

    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;



        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        curShotDelay = 0;

        audioSource.clip = audioFollowerFire;
        audioSource.Play();
    }

    void Reload()
    {
    curShotDelay += Time.deltaTime;
    }
}
