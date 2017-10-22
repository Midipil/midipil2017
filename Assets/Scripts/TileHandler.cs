using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileHandler : MonoBehaviour {
    [SerializeField] private float _weight;
    public float Weight { get { return _weight; } }
    public bool IsOkayWithSharks = false;

    private GameObject _tile;
    private TileHandler _prevTile;

    [SerializeField] private GameObject _planctonPrefab;
    private float _planctonDensity = 0.5f;
    private float _planctonOffset = -0.2f;

    public Vector3[] Balises
    {
        get;
        private set;
    }

    public void Init(TileHandler prevTile = null)
    {
        _tile = gameObject;
        _prevTile = prevTile;

        Balises = _tile.GetComponentsInChildren<Transform>().Where(t => t.gameObject.CompareTag("balise")).Select(t => t.position).OrderBy(p => p.z).ToArray();

        // generate the plancton, interpolating the path from the previous tile
        var controlPoints = new List<Vector3>();
        if (_prevTile != null) controlPoints.Add(_prevTile.Balises.Last());
        controlPoints.AddRange(Balises);

        var path = controlPoints;// MakeSmoothCurve(controlPoints, 10f);
        float totalZDistance = path.Last().z  - path.First().z;
        float step = _planctonPrefab.transform.localScale.z / _planctonDensity;

        int currentPointAfter = 1;
        for(float z = path.First().z; z <= path.Last().z && currentPointAfter < path.Count; z+= step)
        {
            if (path[currentPointAfter].z <= z) currentPointAfter++;
            if (currentPointAfter >= path.Count) break;

            var before = path[currentPointAfter - 1];
            var after = path[currentPointAfter];

            var interpolatedPos = Vector3.Lerp(before, after, Mathf.Abs(z - before.z) / Mathf.Abs(after.z - before.z));

            var plancton = Instantiate(_planctonPrefab, transform, true);
            // Set height
            interpolatedPos = new Vector3(interpolatedPos.x, GlobalVars.Instance.playerHeight + _planctonOffset, interpolatedPos.z);
            plancton.transform.position = interpolatedPos;

        }
    }

    // arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
    public static Vector3[] MakeSmoothCurve(IList<Vector3> arrayToCurve, float smoothness)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (smoothness < 1.0f) smoothness = 1.0f;

        pointsLength = arrayToCurve.Count;

        curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3>(arrayToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints.ToArray());
    }
}
