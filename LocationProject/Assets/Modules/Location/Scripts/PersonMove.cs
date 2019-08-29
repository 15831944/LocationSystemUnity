using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对于人员，我们在PhysicsManager设置人员所在layer不能与地板所在layer和自身碰撞
/// </summary>
public class PersonMove : MonoBehaviour {

    public float speed = 0.1F;
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
        transform.position = new Vector3(transform.position.x, pos.y, transform.position.z);
        moveDirection = pos - transform.position;
        UpdatePosition();

        //moveDirection = pos - transform.position;
        //UpdatePosition();
    }

    /// <summary>
    /// 设置位置
    /// </summary>
    public void SetPosition_History(Vector3 pos)
    {
        //transform.position = new Vector3(transform.position.x, pos.y, transform.position.z);
        moveDirection = pos - transform.position;
        UpdatePosition();

        //moveDirection = pos - transform.position;
        //UpdatePosition();
    }

    /// <summary>
    /// 刷新位置
    /// 注意：对于人员，我们在PhysicsManager设置人员所在layer不能与地板所在layer和自身碰撞
    /// </summary>
    public void UpdatePosition()
    {
        //moveDirection *= speed;
        //controller.Move(moveDirection);

        //controller.Move(moveDirection * Time.deltaTime * speed);
        Vector3 back = transform.position;

        if (controller.enabled == false)
        {
            controller.enabled = true;
        }

        CollisionFlags flags= controller.Move(moveDirection);//注意：对于人员，我们在PhysicsManager设置人员所在layer不能与地板所在layer和自身碰撞

        if ((flags & CollisionFlags.Sides)!=0)
        {
            transform.position = back;
        }
    }

    public void SetCharacterController(bool isBool)
    {
        controller.enabled = isBool;
    }
}
