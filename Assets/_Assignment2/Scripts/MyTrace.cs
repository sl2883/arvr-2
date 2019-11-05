using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Usage
//MyTrace.trace(new List<string>{ "place function executed"}, true);
namespace Helpers
{
    public static class MyTrace
    {
        public static void trace(List<string> str, bool same)
        {
            Debug.Log("**********SUNNY***************");
            if (same)
            {

                string strOut = string.Join(", ", str);
                Debug.Log("---SUNNY---" + strOut);
            }
            else
            {
                for (int i = 0; i < str.Count; i++)
                {
                    Debug.Log("---SUNNY--- " + str);
                }
            }

        }

        public static void tracePoint(string pointName, Vector3 vector3)
        {
            Debug.Log("**********SUNNY***************");
            Debug.Log("---SUNNY---" + pointName + " " + vector3.ToString());
        }
    }
    
}


