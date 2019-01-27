﻿using Game.Scripts;
using UnityEngine;

public class BreakableGate : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == GameConstants.ShellLayer)
        {
            gameObject.SetActive(false);
        }
    }
}
