using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    Transform PromptAnchor { get; }
    void SetHighlighted(bool isOn);
    bool Interact(PlayerCarry carry);
}
