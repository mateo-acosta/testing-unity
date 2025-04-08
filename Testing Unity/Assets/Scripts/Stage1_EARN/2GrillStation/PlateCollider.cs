using UnityEngine;

public class PlateCollider : MonoBehaviour
{
    public enum PlateType
    {
        LeftPlate,  // Raw patty source
        RightPlate  // Cooked patty destination
    }
    
    public PlateType plateType;
    public float detectionRadius = 1.0f;
    
    // Check if a point is within this plate's area
    public bool IsPointOnPlate(Vector2 point)
    {
        Vector2 platePosition = new Vector2(transform.position.x, transform.position.y);
        return Vector2.Distance(platePosition, point) < detectionRadius;
    }
    
    // Visual representation of the detection radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = plateType == PlateType.LeftPlate ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
} 