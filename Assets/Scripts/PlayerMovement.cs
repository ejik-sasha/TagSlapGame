using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviourPun
{
    public float normalSpeed = 8f;
    public float taggerSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    CharacterController controller;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        PlayerTag tag = GetComponent<PlayerTag>();
        float speed = tag.isTagger ? taggerSpeed : normalSpeed;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}
