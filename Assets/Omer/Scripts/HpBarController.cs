using System;
using UnityEngine;
using UnityEngine.UI;
public class HpBarController : MonoBehaviour
{

    public Slider hPBar;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Can", StringComparison.Ordinal))
        {
            hPBar.value += 10;
            if (hPBar.value >= 100)
            {
                hPBar.value = 100;
                Debug.Log("Player HP is Full!");
            }
        }
        else if (other.gameObject.tag.Equals("Enemy", StringComparison.Ordinal))
        {
            hPBar.value -= 5;
            if (hPBar.value <= 0)
            {
                hPBar.value = 0;
            }
        }
        else if (other.gameObject.tag.Equals("Enemy2", StringComparison.Ordinal))
        {
            hPBar.value -= 10;
            if (hPBar.value <= 0)
            {
                hPBar.value = 0;
            }
        }
        else if (other.gameObject.tag.Equals("Kaktus", StringComparison.Ordinal))
        {
            hPBar.value -= 1;
            if (hPBar.value <= 0)
            {
                hPBar.value = 0;
            }
        }
    }
}
