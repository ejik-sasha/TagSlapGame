using UnityEngine;
using Photon.Pun;

public class PlayerTag : MonoBehaviourPun
{
    public bool isTagger;

    [Header("Hand")]
    public GameObject handRoot;

    [Header("Visual")]
    public Renderer bodyRenderer;
    public Color taggerColor = Color.red;
    public Color runnerColor = Color.white;

    void Start()
    {
        UpdateState();
    }

    [PunRPC]
    public void SetTagger(bool value)
    {
        isTagger = value;
        UpdateState();
    }

    void UpdateState()
    {
        UpdateColor();
        UpdateHand();
    }

    void UpdateColor()
    {
        if (!bodyRenderer) return;
        bodyRenderer.material.color = isTagger ? taggerColor : runnerColor;
    }

    void UpdateHand()
    {
        if (!handRoot) return;
        handRoot.SetActive(isTagger);
    }
}