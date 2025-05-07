using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public RectTransform shopImage;
   // private float boundStrength = 10f;
   // private float boundDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        StartBounceEffect();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartBounceEffect()
    {
        shopImage.DOLocalRotate(new Vector3(0, 0, 10), 0.1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
