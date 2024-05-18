using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;

public class Temp : MonoBehaviour
{
    UniTask uniTask;
    Tween tween;

    private void Start()
    {
        Debug.Log("test");
        Test().Forget();
    }

    private async UniTask Test()
    {
        await transform.DOMove(Vector3.zero, 1f);
        await UniTask.Delay(1000);
        Debug.Log("test2");
    }
}
