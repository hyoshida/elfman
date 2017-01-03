using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utils {
    enum HitType {
        NONE,
        GROUND,
        WALL
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

            if (hitGround) {
                return HitType.GROUND;
            }
            if (hitWall) {
                return HitType.WALL;
            }
            return HitType.NONE;
        }
    }
}
