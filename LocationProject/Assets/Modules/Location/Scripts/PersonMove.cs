using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMove : MonoBehaviour {

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    public Transform targetTransform;
    CharacterController controller;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        //CharacterController controller = GetComponent<CharacterController>();
        //if (controller.isGrounded)
        //{
        //    moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //    moveDirection = transform.TransformDirection(moveDirection);
        //    moveDirection *= speed;
        //    if (Input.GetButton("Jump"))
        //        moveDirection.y = jumpSpeed;

        //}
        //moveDirection.y -= gravity * Time.deltaTime;
        //controller.Move(moveDirection * Time.deltaTime);


        //SetPosition(targetTransform.position);
        //UpdatePosition();
    }

    /// <summary>
    /// 设置位置
    /// </summary>
    public void SetPosition(Vector3 pos)
    {
        //transform.position = pos;
        transform.position = new Vector3(transform.position.x, pos.y, transform.position.z);
        moveDirection = pos - transform.position;
        UpdatePosition();
    }

    /// <summary>
    /// 刷新位置
    /// </summary>
    public void UpdatePosition()
    {

        //if (controller.isGrounded)
        //{
        //    moveDirection *= speed;

        //}
        moveDirection *= speed;
        //moveDirection.y -= gravity * Time.deltaTime;
        //controller.Move(moveDirection * Time.deltaTime);
        controller.Move(moveDirection);
    }

    public void SetCharacterController(bool isBool)
    {
        controller.enabled = isBool;
    }
}
