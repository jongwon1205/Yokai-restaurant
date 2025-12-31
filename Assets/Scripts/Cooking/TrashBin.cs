using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour, IInteractable
{
    [Header("Prompt / Highlight")]
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private GameObject highlightObject;

    [Header("SFX")]
    [SerializeField] private AudioClip discardSfx;
    [SerializeField] private AudioSource audioSource;

    public Transform PromptAnchor => promptAnchor != null ? promptAnchor : transform;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void SetHighlighted(bool isOn)
    {
        if (highlightObject != null)
            highlightObject.SetActive(isOn);
    }

    public bool Interact(PlayerCarry carry)
    {
        return TryInteract(carry);
    }

    public bool TryInteract(PlayerCarry carry)
    {
        if (carry == null) return false;

        if (!carry.HasFood())
            return false;

        bool discarded = carry.TryDiscard();
        if (discarded)
            PlayDiscardSfx();

        return discarded;
    }

    private void PlayDiscardSfx()
    {
        if (discardSfx == null || audioSource == null)
            return;

        audioSource.PlayOneShot(discardSfx);
    }
}
