using UnityEngine;

public interface ILeft
{
   public bool facingLeft   {
        get {
            if (this is MonoBehaviour behaviour)
                return behaviour.transform.lossyScale.x < 0f;
            else
                return false;
        }
        private set { } }
   public int facingLeftInt {
        get
        {
            if (this is MonoBehaviour behaviour)
                return behaviour.transform.lossyScale.x < 0f?-1:1;
            else
                return 1;
        }
        private set { } }
}
