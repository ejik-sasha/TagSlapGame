using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviourPun
{
    public Camera cam;

    void Awake()
    {
        cam.gameObject.SetActive(photonView.IsMine);
    }
}
