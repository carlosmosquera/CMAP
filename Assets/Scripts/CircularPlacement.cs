using UnityEngine;

[ExecuteInEditMode]
public class CircularPlacement : MonoBehaviour
{
    public GameObject circlePrefab; // The prefab for the circular objects
    public float radius = 5f; // The radius of the circle
    public int numberOfCircles = 8; // Number of circles to place
    public float startAngle = 0f; // Starting angle in degrees

    void Start()
    {
        PlaceCircles();
    }

    void PlaceCircles()
    {
        float angleStep = 360f / numberOfCircles; // Calculate the angle between each circle

        for (int i = 0; i < numberOfCircles; i++)
        {
            float angle = startAngle + i * angleStep;
            float angleRad = angle * Mathf.Deg2Rad;

            // Calculate the position of the circle
            Vector2 circlePosition = new Vector2(
                transform.position.x + Mathf.Cos(angleRad) * radius,
                transform.position.y + Mathf.Sin(angleRad) * radius
            );

            // Instantiate the circle at the calculated position
            Instantiate(circlePrefab, circlePosition, Quaternion.identity);
        }
    }
}
