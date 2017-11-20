//
// See https://www.youtube.com/watch?v=wGI2e3Dzk_w&list=PLX2vGYjWbI0SUWwVPCERK88Qw8hpjEGd8
//
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {
    public float gravityModifier = 1f;
    public float minGroundNomalY = 0.65f;
    public Vector2 velocity;
    public bool frozen;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNomal;
    protected Rigidbody2D rigibody2d;
    protected ContactFilter2D contactFilter2d;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMovementDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    protected virtual void ComputeVelocity() {
    }

    void Start() {
        contactFilter2d.useTriggers = false;
        contactFilter2d.useLayerMask = true;
        contactFilter2d.SetLayerMask(BuildLayerMask());

        rigibody2d = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (frozen) {
            return;
        }

        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    void FixedUpdate() {
        if (frozen) {
            return;
        }

        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;

        var deltaPosition = velocity * Time.deltaTime;

        // オブジェクトが地面についていればスロープを考慮する
        // 空中にいるときなどは地面の形状を考慮しない
        var count = rigibody2d.Cast(Vector2.down, contactFilter2d, hitBuffer, shellRadius);
        var currentGrounded = (count >= 1);

        var movementAlongGround = new Vector2(groundNomal.y, -groundNomal.x);
        var movementHorizontal = currentGrounded ? movementAlongGround * deltaPosition.x : new Vector2(deltaPosition.x, 0);
        Move(movementHorizontal);

        var movementVertical = Vector2.up * deltaPosition.y;
        Move(movementVertical);

        Debug.DrawRay(transform.position, groundNomal, Color.green);
        Debug.DrawRay(transform.position, movementAlongGround, Color.yellow);
        Debug.DrawRay(transform.position, movementHorizontal * 10, Color.red);
        Debug.DrawRay(transform.position, movementVertical * 10, Color.blue);
    }

    void Move(Vector2 movement) {
        var distance = movement.magnitude;
        if (distance > minMovementDistance) {
            var count = rigibody2d.Cast(movement, contactFilter2d, hitBuffer, distance + shellRadius);

            hitBufferList.Clear();

            for (var i = 0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (var i = 0; i < hitBufferList.Count; i++) {
                var currentNomal = hitBufferList[i].normal;
                if (currentNomal.y > minGroundNomalY) {
                    grounded = true;
                    groundNomal = currentNomal;
                    currentNomal.x = 0;
                }

                var projection = Vector2.Dot(velocity, currentNomal);
                if (projection < 0) {
                    velocity = velocity - projection * currentNomal;
                }

                var modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = (minMovementDistance < distance) ? modifiedDistance : distance;
            }
        }

        rigibody2d.position = rigibody2d.position + movement.normalized * distance;
    }

    LayerMask BuildLayerMask() {
        var layer = LayerMask.NameToLayer("PhysicsObject");
        return Physics2D.GetLayerCollisionMask(layer);
    }
}