using UnityEngine;
using System.Collections;

public class SpikeyAttack : MonoBehaviour
{
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackSpeed = 15f;
    [SerializeField] private float returnSpeed = 10f;

    public IEnumerator DoAttack(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        Vector3 dir = (targetPos - startPos).normalized;
        Vector3 endPos = startPos + dir * attackDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * attackSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * returnSpeed;
            transform.position = Vector3.Lerp(endPos, startPos, t);
            yield return null;
        }
    }
}
