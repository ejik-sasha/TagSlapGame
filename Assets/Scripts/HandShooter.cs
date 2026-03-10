using UnityEngine;
using Photon.Pun;

public class HandShooter : MonoBehaviour
{
    public float shootDistance = 6f;
    public float shootSpeed = 15f;

    Vector3 startLocalPos;
    bool shooting;
    bool returning;

    PlayerTag ownerTag;
    PhotonView ownerPV;

    void Start()
    {
        ownerTag = GetComponentInParent<PlayerTag>();
        ownerPV  = GetComponentInParent<PhotonView>();

        startLocalPos = transform.localPosition;

        if (!ownerPV || !ownerPV.IsMine)
            enabled = false;
    }

    void Update()
    {
        if (!ownerPV.IsMine) return;
        if (!ownerTag.isTagger) return;

        if (Input.GetMouseButtonDown(0) && !shooting && !returning)
        {
            shooting = true;
        }

        if (shooting)
        {
            transform.localPosition += Vector3.forward * shootSpeed * Time.deltaTime;

            if (Vector3.Distance(startLocalPos, transform.localPosition) >= shootDistance)
            {
                shooting = false;
                returning = true;
            }
        }
        else if (returning)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                startLocalPos,
                shootSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.localPosition, startLocalPos) < 0.05f)
            {
                returning = false;
                transform.localPosition = startLocalPos;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!ownerPV.IsMine) return;
        if (!ownerTag.isTagger) return;

        PlayerTag otherTag = other.GetComponent<PlayerTag>();
        if (!otherTag) return;
        if (otherTag.isTagger) return;

        Debug.Log("TAG HIT: " + otherTag.photonView.Owner.NickName);

        GameManager.Instance.photonView.RPC(
            "RequestTagSwap",
            RpcTarget.MasterClient,
            ownerTag.photonView.ViewID,
            otherTag.photonView.ViewID
        );

        shooting = false;
        returning = true;
    }
}