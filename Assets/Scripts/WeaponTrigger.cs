using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrigger : MonoBehaviour
{
    [SerializeField] Player parent;

    private void OnTriggerEnter(Collider other)
    {
        parent.WeaponTriggerEnter(other);
    }
}
