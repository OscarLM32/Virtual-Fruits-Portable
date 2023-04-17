using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Enemy patrol script using DOTween meant to be used for simple patrolling shapes: no rotation, linear or simple shapes,
/// no wait time, etc. 
/// </summary>
/// <remarks>
/// This class heavily depends on DOTween methods and parameters. Check the DOTween documentation for more information.
/// </remarks>
/// <see href="http://dotween.demigiant.com/documentation.php">DOTween documentation</see>
public class EnemyBasicPatrolling : MonoBehaviour
{
    /// <summary>
    /// The points that are going to construct the shape of the patrolling
    /// </summary>
    /// <remarks>
    /// The initial position of the object does not need to be included in the list
    /// </remarks>>
    public Vector3[] patrolPoints;
    public float totalPatrolTime;
    public PathType pathType = PathType.CubicBezier;
    
    /// <summary>
    /// Id that sets apart this tween animation so it can be manipulated after initialization
    /// </summary>
    public string patrolId;
    public Ease easeType = Ease.Linear;
    public LoopType loopType = LoopType.Yoyo;

    /// <summary>
    /// Starts the patrol animation based on the parameters set in the inspector menu in Unity
    /// </summary>
    private void Start()
    {
        transform.DOLocalPath(
            patrolPoints, 
            totalPatrolTime, 
            pathType, 
            PathMode.Sidescroller2D, 
            5)
            .SetId(patrolId)
            .SetEase(easeType)
            .SetLoops(-1, loopType);
    }
}
