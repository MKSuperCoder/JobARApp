using System.Collections;
using UnityEngine;

public class CubeAnimator : MonoBehaviour
{
    public IEnumerator MoveCubeToTrashCan(GameObject cube, Vector3 liftPosition, Vector3 trashPosition, float duration)
    {
        Vector3 startPosition = cube.transform.position;
        float elapsedTime = 0f;

        // Lift the cube
        while (elapsedTime < duration)
        {
            cube.transform.position = Vector3.Lerp(startPosition, liftPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cube.transform.position = liftPosition;

        elapsedTime = 0f;

        // Move the cube to the trash can
        while (elapsedTime < duration)
        {
            cube.transform.position = Vector3.Lerp(liftPosition, trashPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cube.transform.position = trashPosition;

        Destroy(cube);
    }
}
