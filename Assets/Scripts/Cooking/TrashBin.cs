using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip discardSfx;
    [SerializeField] private AudioSource audioSource;

    public bool TryInteract(PlayerCarry carry)
    {
        if (carry == null) return false;

        if (!carry.HasFood())
        {
            return false;
        }

            PlayDiscardSfx();

        bool discarded = carry.TryDiscard();
        return discarded;
    }

    private void PlayDiscardSfx()
    {
        if (discardSfx == null || audioSource == null)
            return;

        audioSource.PlayOneShot(discardSfx);
    }
}
