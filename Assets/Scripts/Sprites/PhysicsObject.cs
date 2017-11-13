//
// See https://www.youtube.com/watch?v=wGI2e3Dzk_w&list=PLX2vGYjWbI0SUWwVPCERK88Qw8hpjEGd8
//
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {
    public float gravityModifier = 1f;
    public float minGroundNomalY = 0.65f;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNomal;
    protected Rigidbody2D rigibody2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter2d;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMovementDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    protected virtual void ComputeVelocity() {
    }

    void Start() {
        contactFilter2d.useTriggers = false;
        contactFilter2d.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter2d.useLayerMask = true;

        rigibody2d = GetComponent<Rigidbody2D>();
    }

    void Update() {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    void FixedUpdate() {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;

        var deltaPosition = velocity * Time.deltaTime;

        var movementAlongGround = new Vector2(groundNomal.y, -groundNomal.x);
        var movementHorizontal = movementAlongGround * deltaPosition.x;
        Move(movementHorizontal, vertical: false);

        var movementVertical = Vector2.up * deltaPosition.y;
        Move(movementVertical, vertical: true);
    }

    void Move(Vector2 movement, bool vertical) {
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
                    if (vertical) {
                        groundNomal = currentNomal;
                        currentNomal.x = 0;
                    }
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
}