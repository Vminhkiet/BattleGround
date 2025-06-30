using Photon.Pun;
using UnityEngine;

public class PhotonOwnershipAdapter : INetworkOwnership
{
    private PhotonView _photonView;

    public PhotonOwnershipAdapter(PhotonView photonView)
    {
        _photonView = photonView;
    }

    public bool IsLocalPlayer
    {
        get { return _photonView.IsMine; }
    }
}