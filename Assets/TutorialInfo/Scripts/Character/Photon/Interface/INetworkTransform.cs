using UnityEngine;
public interface INetworkTransform
{
    void SendRotation(Quaternion rotation);
    void ReceiveRotation(Quaternion rotation);
}