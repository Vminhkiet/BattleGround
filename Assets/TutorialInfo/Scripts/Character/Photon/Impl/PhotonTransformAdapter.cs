using UnityEngine;
using Photon.Pun;

public class PhotonTransformAdapter : MonoBehaviourPun, INetworkTransform, IPunObservable
{
    private Quaternion _networkRotation;
    [SerializeField] private float _smoothingFactor = 10f;

    void Awake()
    {
        if (photonView == null)
        {
            Debug.LogError("PhotonTransformAdapter: PhotonView component not found on this GameObject. This adapter will not work.", this);
            this.enabled = false;
        }
        _networkRotation = transform.rotation;
    }


    void Update()
    {
        if (!photonView.IsMine)
        {
            if (Quaternion.Angle(transform.rotation, _networkRotation) < 0.1f)
            {
                transform.rotation = _networkRotation;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, _networkRotation, _smoothingFactor * Time.deltaTime);
        }
    }

    public void SendRotation(Quaternion rotation) { }

    public void ReceiveRotation(Quaternion rotation)
    {
        _networkRotation = rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else
        {
            Quaternion received = (Quaternion)stream.ReceiveNext();
            ReceiveRotation(received);
        }
    }
}