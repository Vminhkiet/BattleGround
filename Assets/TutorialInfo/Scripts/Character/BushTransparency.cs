using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushTransparency : MonoBehaviourPunCallbacks
{
    [Tooltip("Chọn layer của các bụi rậm.")]
    [SerializeField] private LayerMask bushLayer;

    private int overlappingBushCount = 0;

    private CharacterVisiblity _visiblity;
    private PhotonView photonView;

    private void Start()
    {
        _visiblity = GetComponent<CharacterVisiblity>();
        photonView =GetComponent<PhotonView>();
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
            _visiblity.SetAlpha(0);

            if (photonView.IsMine)
            {
                photonView.RPC(nameof(RPC_SetAlpha), RpcTarget.Others,0f);
            }
        }
        else
        {
            _visiblity.SetVisible();

            if (photonView.IsMine)
            {
                photonView.RPC(nameof(RPC_SetAlpha), RpcTarget.Others, 1.0f);
            }
        }
    }


    [PunRPC]
    void RPC_SetAlpha(float alpha)
    {
        if(photonView==null)
            return;
        if (!photonView.IsMine)
        {
            Debug.Log("aaa");
            _visiblity.SetAlpha(alpha);
        }
    }

}