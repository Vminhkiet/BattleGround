using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushTransparency : MonoBehaviour
{
    [Tooltip("Chọn layer của các bụi rậm.")]
    [SerializeField] private LayerMask bushLayer;

    private int overlappingBushCount = 0;

    private CharacterVisiblity _visiblity;

    private void Start()
    {
        _visiblity = GetComponent<CharacterVisiblity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((bushLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            overlappingBushCount++;
            UpdateCharacterAlpha();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((bushLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            overlappingBushCount--;
            if (overlappingBushCount < 0)
            {
                overlappingBushCount = 0;
            }
            UpdateCharacterAlpha();
        }
    }

    public void UpdateCharacterAlpha()
    {
        if (overlappingBushCount > 0)
        {
            _visiblity.SetInvisible();
        }
        else
        {
            _visiblity.SetVisible();
        }
    }
}