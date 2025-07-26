using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyAssistant
{
    public static Vector3 detectPlayer(bool facingLeft,GameObject gameObject)
    {
        for (int i = 0; i < 180; i++) //probably overkill
        {
            float angle = (gameObject.transform.eulerAngles.z + 90 - i + (System.Convert.ToSingle(facingLeft) * 180)) % 360;
            Vector2 temp = new Vector2(Mathf.Cos(angle * Mathf.PI / 180), Mathf.Sin(angle * Mathf.PI / 180));
            RaycastHit2D[] lookForPlayer = Physics2D.RaycastAll(gameObject.transform.position, temp, 30f);
            foreach (RaycastHit2D hit in lookForPlayer)
            {
                if (hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 15)
                    break;
                if (hit.collider.gameObject != gameObject && hit.collider.gameObject.layer == 9)
                {
                    return new Vector3(temp.x, temp.y, 0f);
                }
            }
        }
        return new Vector3(0f,0f,1f);
    }
}
