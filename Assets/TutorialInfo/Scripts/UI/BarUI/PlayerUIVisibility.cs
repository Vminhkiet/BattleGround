using Photon.Pun;
using UnityEngine;

public class PlayerUIVisibility : MonoBehaviour
{
    [SerializeField] private GameObject attackBarToHide;

    private void Awake()
    {
        PhotonView view = GetComponentInParent<PhotonView>();
        if (view != null && !view.IsMine)
        {
            attackBarToHide.SetActive(false);
        }
    }
}
