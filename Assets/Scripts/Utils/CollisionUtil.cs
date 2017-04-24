using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utils {
    enum HitType {
        NONE    = 0x00,
        GROUND  = 0x01,
        WALL    = 0x02,
        LEFT    = 0x04,
        RIGHT   = 0x08,
        TOP     = 0x10,
        BOTTOM  = 0x20,
    }

    class CollisionUtil {
        public readonly float GROUND_ANGLE_TOLERANCE = Mathf.Cos(30.0f * Mathf.Deg2Rad);

        Collision2D _collision;

        public CollisionUtil(Collision2D collision) {
            _collision = collision;
        }

        public bool IsLayer(String layerName) {
            int groundLayer = LayerMask.NameToLayer(layerName);
            return (_collision.gameObject.layer == groundLayer);
        }

        public HitType HitTest() {
            bool hitGround = false;
            bool hitWall = false;

            // from http://www.gamedev.net/topic/673693-how-to-check-if-grounded-in-2d-unity-game/#entry5265297
            foreach (ContactPoint2D contact in _collision.contacts) {
                if (Vector3.Dot(contact.normal, Vector3.up) > GROUND_ANGLE_TOLERANCE) {
                    // this collider is touching "ground"
                    hitGround = true;
                } else {
                    // this collider is touching "wall"
                    hitWall = true;
                }
            }

            var hitType = HitType.NONE;

            if (_collision.contacts.Length > 0) {
                // from http://answers.unity3d.com/questions/783377/detect-side-of-collision-in-box-collider-2d.html#answer-783413
                Vector3 contactPoint = _collision.contacts[0].point;
                Vector3 center = _collision.collider.bounds.center;

                bool right = contactPoint.x > center.x;
                bool top = contactPoint.y > center.y;

                hitType |= (right ? HitType.RIGHT : HitType.LEFT);
                hitType |= (top ? HitType.TOP : HitType.BOTTOM);
            }

            if (hitGround) {
                return HitType.GROUND | hitType;
            }
            if (hitWall) {
                return HitType.WALL | hitType;
            }
            return HitType.NONE | hitType;
        }
    }
}