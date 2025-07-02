using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class ExplosionEffect : MonoBehaviour
{
    public float lifetime = 2f;

    private void OnEnable()
    {
        Invoke(nameof(ReturnToPool), lifetime);
    }

    void ReturnToPool()
    {
        ExplosionPooler.Instance.ReturnToPool(gameObject);
    }

}