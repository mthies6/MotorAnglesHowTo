using UnityEngine;
using System.Collections;

//each instance of this class represents a joint/motor and will be used to gather angle info
public class Motor : MonoBehaviour
{
    public enum Axis { unset = -1, x, y, z };
    public Axis importantAxis = Axis.unset;

    public float[] importantAngles;
    public float[] interpolated;
    public GameObject inGame;
    public float defaultAngle = 0;

    public void LogValue(int index)
    {
        //if unset, still discerning which is important
        if (importantAxis == Axis.unset)
        {
            float angle = this.inGame.transform.localEulerAngles.x;
            if (Mathf.Abs(angle) > 1) { importantAxis = Axis.x; }

            angle = this.inGame.transform.localEulerAngles.y;
            if (Mathf.Abs(angle) > 1) { importantAxis = Axis.y; }

            angle = this.inGame.transform.localEulerAngles.z;
            if (Mathf.Abs(angle) > 1) { importantAxis = Axis.z; }

            importantAngles[index] = 0;
        }

        //if set, can directly assign importantAngles value (axis may be set in prev if statement)
        if (importantAxis != Axis.unset)
        {

            switch (importantAxis)
            {
                case Axis.x:
                    importantAngles[index] = this.inGame.transform.localEulerAngles.x;
                    break;
                case Axis.y:
                    importantAngles[index] = this.inGame.transform.localEulerAngles.y;
                    break;
                case Axis.z:
                    importantAngles[index] = this.inGame.transform.localEulerAngles.z;
                    break;
            }

            if (index == 0) //if this is the first keyframe, and importantAxis has been set 
            {
                //by setting default value as the original position, you center the angle values around 0 = original position
                //comment the following line out if you want the angle value 0 to mean global 0
                defaultAngle = importantAngles[index]; //default value is this first value
                if (defaultAngle > 180) { defaultAngle -= 360; } //constrain angle between -180 and 180
            } //otherwise x, y, and z are all 0 and thus default = 0


            importantAngles[index] -= defaultAngle;

            if (importantAngles[index] > 180) { importantAngles[index] -= 360; } //constrain angles between -180 and 180
        }
    }
}
