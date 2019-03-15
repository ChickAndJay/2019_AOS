using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Shelly : CharacterAttack
{
    GameObject _firePosParent;
    public GameObject _shotIndicatorPrefab;
    GameObject _shotIndicator;
    Coroutine _drawIndicatorRoutine;
    public float viewRadius = 3;
    public float viewAngle = 30f;
    public float meshResolution = 1f;
    MeshFilter viewMeshFilter;
    Mesh viewMesh;
    public LayerMask obstacleMask;
    public int edgeResolveIterations;
    public float edgeDstThreshold;


	// Use this for initialization
	void Start () {
        base.Start();
        _shotIndicator = GameObject.Instantiate(_shotIndicatorPrefab, transform.position, transform.rotation);
        _shotIndicator.SetActive(false);
        viewMeshFilter = _shotIndicator.GetComponent<MeshFilter>();

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        _shotIndicator.SetActive(false);

        _firePosParent = PlayerController.instance._firePosParent;

    }

    // Update is called once per frame
    void Update () {
        _fireDir = PlayerController.instance._attackStickDir;
       // Debug.Log(_fireDir + " _ " + PlayerController.instance.name);
       // _shotIndicator.transform.position = transform.position;
	}

    public override void FireBaseAttack()
    {
        if (!_playerStats.onFire()) return;
        StartCoroutine(ActivateBaseAttack(false));
    }

    public override void FireSkillAttack()
    {
        base.FireSkillAttack();
        StartCoroutine(ActivateBaseAttack(true));

    }

    public override void StartDrawIndicator(bool isSkill)
    {
        if (!_shotIndicator.activeSelf)
        {
            _shotIndicator.SetActive(true);
            _drawIndicatorRoutine = StartCoroutine(DrawAttackIndicator(isSkill));
        }
    }

    public override void StopDrawIndicator()
    {
        if(_drawIndicatorRoutine != null)
        {
            StopCoroutine(_drawIndicatorRoutine);
            _shotIndicator.SetActive(false);
        }
        else
        {

        }
    }

    IEnumerator ActivateBaseAttack(bool isSkill)
    {
        PlayerController.instance.isActivatingSkill = true;

        Vector3 fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);
        GameObject muzzle;
        GameObject projectile;

        int stepCount = 3;
        float stepAngleSize = viewAngle / stepCount;

        _upperBodyDir = fireDir;
        rotate = true;

        if (isSkill)
        {
            viewRadius = PlayerController.instance._playerStats._skillRange;
            viewAngle = 45f;
            stepCount = 5;
        }
        else
        {
            viewRadius = PlayerController.instance._playerStats._bulletRange;
            viewAngle = 35f;
            stepCount = 3;
        }

        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < stepCount ; i++)
        {
            float y_stick = Vector3.Angle(Vector3.forward, fireDir);
            if (fireDir.x < 0)
                y_stick *= -1;
            float angle = y_stick - viewAngle / 2 + stepAngleSize * i;

            Vector3[] startPoints = new Vector3[4];
            for (int j=0; j<4; j++)
            {
                if(j != 2)
                    angle += stepAngleSize / 4;
                Vector3 dirFromAngle =
                          new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad),
                           0,
                           Mathf.Cos(angle * Mathf.Deg2Rad));

                if( j == 0 || j == 3)
                    startPoints[j] = transform.position + Vector3.up + dirFromAngle * 0.5f;
                else if( j== 1)
                    startPoints[j] = transform.position + Vector3.up + dirFromAngle * 0.4f;
                else if(j == 2)
                    startPoints[j] = transform.position + Vector3.up + dirFromAngle * 0.6f;

                projectile = GameObject.Instantiate(_projectile,
                    startPoints[j],
                    new Quaternion(0,0,0,0));
                projectile.transform.rotation = Quaternion.LookRotation(dirFromAngle);


                projectile.GetComponent<Projectile_Colt>().TheStart(
                    _playerStats._bulletVelocity,
                    _playerStats._damage
                    , _playerStats._bulletRange
                    , isSkill);

                
            }

        }

        PlayerController.instance.isActivatingSkill = false;
        rotate = false;

    }

    IEnumerator DrawAttackIndicator(bool isSkill)
    {
        if (isSkill)
        {
            viewRadius = PlayerController.instance._playerStats._skillRange;
            viewAngle = 45f;
        }
        else
        {
            viewRadius = PlayerController.instance._playerStats._bulletRange;
            viewAngle = 35f;
        }

        while (true)
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            List<Vector3> startPoints = new List<Vector3>();
            for (int i=0; i<stepCount; i++)
            {
                float y_stick = Vector3.Angle(Vector3.forward, new Vector3(_fireDir.x, 0, _fireDir.y));
                if (_fireDir.x < 0)
                    y_stick *= -1;
                float angle = y_stick - viewAngle / 2 + stepAngleSize * i;

                Vector3 dirFromAngle = 
                    new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad),
                    0,
                    Mathf.Cos(angle * Mathf.Deg2Rad));
                Vector3 fireParentPos = PlayerController.instance._firePosParent.transform.position;
                Vector3 point = fireParentPos + dirFromAngle.normalized * 0.5f + Vector3.up;
                viewPoints.Add(point);

                if (!isSkill)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(fireParentPos + Vector3.up, dirFromAngle, out hit, viewRadius, obstacleMask))
                        point = hit.point;
                    else
                        point = fireParentPos + dirFromAngle * viewRadius + Vector3.up + dirFromAngle * 0.5f;
                }
                else
                    point = fireParentPos + dirFromAngle * viewRadius + Vector3.up + dirFromAngle * 0.5f;

                viewPoints.Add(point);
            }

            int vertexCount = viewPoints.Count;


            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = (viewPoints[i]); 
                if (i * 6 + 5 < (vertexCount - 2) * 3)
                {
                    try
                    {
                        triangles[i * 6] = i*2;
                        triangles[i * 6 + 1] = i*2 + 1;
                        triangles[i * 6 + 2] = i*2 + 2;

                        triangles[i * 6 + 3] = i*2 + 2;
                        triangles[i * 6 + 4] = i*2 + 1;
                        triangles[i * 6 + 5] = i*2 + 3;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("i : " + i);
                    }
                }
            }

            viewMesh.Clear();

            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();

            yield return new WaitForEndOfFrame();
        }
    }
}
