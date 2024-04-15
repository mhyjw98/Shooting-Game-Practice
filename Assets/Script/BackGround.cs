using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;

    float viewHeight;

    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;
    }

    void Update()
    {
        Move();
        Scrolling();
    }

    private void Move()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;      
    }

    void Scrolling()
    {
        if (sprites[endIndex].position.y < viewHeight * (-1))
        {
            // Sprite 재사용
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * viewHeight;

            // Index 변경
            int starIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (starIndexSave - 1 == -1) ? sprites.Length - 1 : starIndexSave - 1;
        }
    }
}
