using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class HM
{
    public static void RotateTransformToAngle(Transform t, Vector3 vec)
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = vec;
        t.rotation = q;
    }
    public static void RotateLocalTransformToAngle(Transform t, Vector3 vec)
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = vec;
        t.localRotation = q;
    }
    public static float GetAngle2DBetween(Vector3 from, Vector3 to)
    {
        return Mathf.Rad2Deg * Mathf.Atan2(from.y - to.y, from.x - to.x);
    }

    public static RaycastHit2D Raycast2DAtPosition(Vector3 pos, int layerMask = 0)
    {
        //make sure we arent on the same plane as our object we are trying to hit
        pos.z = 1000;
        return Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, layerMask);
    }
    public static RaycastHit RaycastAtPosition(Vector3 startingPos, Vector3 dir, float length = Mathf.Infinity, int layerMask = -1)
    {
        RaycastHit hit;
        if (layerMask == -1) Physics.Raycast(startingPos, dir, out hit, length);
        else Physics.Raycast(startingPos, dir, out hit, length, layerMask);
        return hit;
    }
    public static RaycastHit2D RaycastToMouseCursor()
    {
        return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity);
    }
    /// <summary>
    /// Find the LayerMask by using LayerMask.GetMask("layermask name here")
    /// </summary>
    /// <param name="layerMask"> </param>
    /// <returns></returns>
    public static RaycastHit2D RaycastToMouseCursor(int layerMask)
    {
        return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);
    }
}
