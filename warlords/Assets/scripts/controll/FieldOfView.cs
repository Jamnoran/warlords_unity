using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

public class FieldOfView : MonoBehaviour
{
    public int TYPE_OF_FIELD_OF_VIEW = 0;   // 0 == undeclared, 1 == minion, 2 == hero, 3 == for spells.
    public static int MINION = 1;
    public static int HERO = 2;
    private bool sentInitialAggro = false;
    
    public Vector3 desiredPosition;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    
    public List<Transform> visibleTargets = new List<Transform>();
    private List<Minion> minions = new List<Minion>();
    private List<Hero> heroes = new List<Hero>();

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindTargetsWithDelay", .2f);
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    public bool FindVisibleTargets()
    {
        //visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
       // Debug.Log("Size of target in view radiusu: " + targetsInViewRadius.Length + " And i am : " + gameObject.name);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    if (!visibleTargets.Contains(target))
                    {
                        Debug.Log("Adding to visable size now: " + visibleTargets.Count + " Of type: " + target.name);
                        visibleTargets.Add(target);
                    }
                    //else {
                        //Debug.Log("Visable already contains this target");
                    //}
                    //if we found target in range add it to the list of targets found.
                    
                    
                    // This is what happens if this class is on a minion
                    if (TYPE_OF_FIELD_OF_VIEW == MINION && !sentInitialAggro && (target.name == "Warrior" || target.name == "Priest")) {
                        Debug.Log("This target is in range : " + target.name + " from vision of a : " + gameObject.name);
                        Debug.Log("Hero, found initiating aggro!");
                        Debug.Log(gameObject.name);
                        Transform currentMinion = gameObject.transform;
                        Debug.Log("Minions position is: " + currentMinion.position);
                        Debug.Log("Hero position is: " + target.position);

                        //make mob look at target before moving it.
                        Vector3 targetPostition = new Vector3(target.position.x, target.transform.position.y, target.position.z);
                        currentMinion.transform.LookAt(targetPostition);

                        // Send this information to server
                        Hero hero = getGameLogic().getClosestHeroByPosition(target.position);
                        Minion minion = getGameLogic().getClosestMinionByPosition(target.position);
                        if (hero != null && minion != null)
                        {
                            sentInitialAggro = true;
                            Debug.Log("Got minion aggro on this id: " + minion.id + " And this heroId: " + hero.id);
                            getCommunication().sendMinionAggro(minion.id, hero.id);
                        }
                    }
                }
                return true;
            }
        }
        return false;
    }

    public bool isPortalInRange()
    {
        foreach (var target in visibleTargets)
        {
            if (target.name.Contains("Stairs"))
            {
                Debug.Log("User had portal in range");
                return true;
            }
        }
        Debug.Log("User was out of reach");
        return false;
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }


    MinionAnimations getAnimation()
    {
        return (MinionAnimations)transform.GetComponent(typeof(MinionAnimations));
    }

    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    /// <summary>
    /// Draw a visual representation of the fov
    /// </summary>
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }


            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}