using UnityEngine;

[ExecuteAlways]
public class ClippingPlane : MonoBehaviour
{
    public Material mat;
    public float planeScaleFactor = 1;

    Mesh clippingPlaneMesh = null;

    // Update is called once per frame
    void Update()
    {
        //planeDebugMesh = null;
        if (mat != null)
        {
            Plane plane = new Plane(transform.up, transform.position);

            Vector4 planeRepresentation =
                new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

            mat.SetVector("_Plane", planeRepresentation);
        }
    }

    private Mesh planeMesh()
    {
        Mesh _returnMesh = new Mesh();

        Vector3[] _verts = new Vector3[]
        {
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, 0.5f)
        };

        int[] _tris = new int[] 
        { 
            0,1,2,
            1,2,3
        };

        _returnMesh.vertices = _verts;
        _returnMesh.triangles = _tris;
        _returnMesh.RecalculateNormals();

        return _returnMesh;
    }

    private void OnDrawGizmos()
    {
        clippingPlaneMesh = planeMesh();
        /*
        if (clippingPlaneMesh == null)
        {
            
        }
        */
        //Quaternion _planeRotation = Quaternion.AngleAxis(90, transform.right);

        Vector3 _scale = new Vector3(transform.localScale.x * planeScaleFactor,
            transform.localScale.y * planeScaleFactor, 
            transform.localScale.z * planeScaleFactor);

        Gizmos.DrawWireMesh(clippingPlaneMesh, 
            transform.position, transform.rotation, _scale);
        //Gizmos.DrawMesh();
    }
}
