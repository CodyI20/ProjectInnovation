using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnGameStart : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        GameManager.OnMatchStart += Enable;
    }
    private void OnDestroy()
    {
        GameManager.OnMatchStart -= Enable;
    }
    
    void Enable()
    {
        gameObject.SetActive(true);
    }
}
