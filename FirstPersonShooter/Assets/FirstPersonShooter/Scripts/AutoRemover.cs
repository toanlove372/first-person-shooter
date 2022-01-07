using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonShooter
{
    public class AutoRemover : MonoBehaviour
    {
        public float delay;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(this.delay);
            Destroy(this.gameObject);
        }
    }
}