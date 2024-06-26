using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Invoke("Disable", 2);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
    public void StartExplosion(string target)
    {
        anim.SetTrigger("OnExplosion");

        switch(target)
        {
            case "S":
                transform.localScale = Vector3.one * 0.7f;
                break;
            case "M":
            case "P":
                transform.localScale = Vector3.one * 1f;
                break;
            case "L":
                transform.localScale = Vector3.one * 2;
                break;
            case "B":
                transform.localScale = Vector3.one * 3;
                break;
        }
    }
}
