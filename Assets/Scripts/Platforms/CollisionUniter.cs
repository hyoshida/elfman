using System;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

// Based on http://gamedev.stackexchange.com/questions/125927/how-do-i-merge-colliders-in-a-tile-based-game
[RequireComponent(typeof(PolygonCollider2D))]
public class CollisionUniter : MonoBehaviour {
    // Use this for initialization
    void Start() {
        UnitColisions();
    }

    void UnitColisions() {
        List<List<Vector2>> polygons = new List<List<Vector2>>();

        foreach (Transform child in transform) {
            BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D>();
            if (!boxCollider2D) {
                continue;
            }

            List<Vector2> polygon = new List<Vector2>();
            // left-top
            polygon.Add(new Vector2(child.position.x, child.position.y));
            // left-bottom
            polygon.Add(new Vector2(child.position.x, child.position.y + boxCollider2D.size.y));
            // right-bottom
            polygon.Add(new Vector2(child.position.x + boxCollider2D.size.x, child.position.y + boxCollider2D.size.y));
            // right-top
            polygon.Add(new Vector2(child.position.x + boxCollider2D.size.x, child.position.y));
            // finish
            polygons.Add(polygon);

            boxCollider2D.enabled = false;
        }

        List<List<Vector2>> unitedPolygons = UniteCollisionPolygons(polygons);
        CreateLevelCollider(unitedPolygons);
    }

    // create the collider in unity from the list of polygons
    void CreateLevelCollider(List<List<Vector2>> polygons) {
        PolygonCollider2D collider = gameObject.GetComponent<PolygonCollider2D>();

        collider.enabled = true;
        collider.pathCount = polygons.Count;

        for (int i = 0; i < polygons.Count; i++) {
            Vector2[] points = polygons[i].ToArray();

            collider.SetPath(i, points);
        }
    }

    // this function takes a list of polygons as a parameter, this list of polygons represent all the polygons that constitute collision in your level.
    List<List<Vector2>> UniteCollisionPolygons(List<List<Vector2>> polygons) {
        // this is going to be the result of the method
        List<List<Vector2>> unitedPolygons = new List<List<Vector2>>();
        Clipper clipper = new Clipper();

        // clipper only works with ints, so if we're working with floats, we need to multiply all our floats by
        // a scaling factor, and when we're done, divide by the same scaling factor again
        int scalingFactor = 10000;

        // this loop will convert our List<List<Vector2>> to what Clipper works with, which is "Path" and "IntPoint"
        // and then add all the Paths to the clipper object so we can process them
        foreach (List<Vector2> polygon in polygons) {
            Path allPolygonsPath = new Path(polygon.Count);

            foreach (Vector2 vector in polygon) {
                allPolygonsPath.Add(new IntPoint(Mathf.Floor(vector.x * scalingFactor), Mathf.Floor(vector.y * scalingFactor)));
            }
            clipper.AddPath(allPolygonsPath, PolyType.ptSubject, true);
        }

        // this will be the result
        Paths solution = new Paths();

        // having added all the Paths added to the clipper object, we tell clipper to execute an union
        clipper.Execute(ClipType.ctUnion, solution);

        // the union may not end perfectly, so we're gonna do an offset in our polygons, that is, expand them outside a little bit
        ClipperOffset offset = new ClipperOffset();
        offset.AddPaths(solution, JoinType.jtMiter, EndType.etClosedPolygon);
        // 5 is the ammount of offset
        offset.Execute(ref solution, 5f);

        //now we just need to conver it into a List<List<Vector2>> while removing the scaling
        foreach (Path path in solution) {
            List<Vector2> unitedPolygon = new List<Vector2>();
            foreach (IntPoint point in path) {
                unitedPolygon.Add(new Vector2(point.X / (float)scalingFactor, point.Y / (float)scalingFactor));
            }
            unitedPolygons.Add(unitedPolygon);
        }

        // this removes some redundant vertices in the polygons when they are too close from each other
        // may be useful to clean things up a little if your initial collisions don't match perfectly from tile to tile
        //unitedPolygons = RemoveClosePointsInPolygons(unitedPolygons);

        // everything done
        return unitedPolygons;
    }

    List<List<Vector2>> RemoveClosePointsInPolygons(List<List<Vector2>> polygons) {
        float proximityLimit = 0.1f;

        List<List<Vector2>> resultPolygons = new List<List<Vector2>>();

        foreach (List<Vector2> polygon in polygons) {
            List<Vector2> pointsToTest = polygon;
            List<Vector2> pointsToRemove = new List<Vector2>();

            foreach (Vector2 pointToTest in pointsToTest) {
                foreach (Vector2 point in polygon) {
                    if (point == pointToTest || pointsToRemove.Contains(point))
                        continue;

                    bool closeInX = Math.Abs(point.x - pointToTest.x) < proximityLimit;
                    bool closeInY = Math.Abs(point.y - pointToTest.y) < proximityLimit;

                    if (closeInX && closeInY) {
                        pointsToRemove.Add(pointToTest);
                        break;
                    }
                }
            }
            polygon.RemoveAll(x => pointsToRemove.Contains(x));

            if (polygon.Count > 0) {
                resultPolygons.Add(polygon);
            }
        }

        return resultPolygons;
    }
}