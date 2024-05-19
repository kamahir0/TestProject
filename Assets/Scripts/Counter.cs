using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class Counter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI counterTMP;
    [SerializeField] RapidButton plusButton;
    [SerializeField] RapidButton minusButton;

    private int counter = 0;

    private void Start()
    {
        plusButton.OnRapidFire
        .Subscribe(_ => IncrementCounter())
        .AddTo(this);

        minusButton.OnRapidFire
        .Subscribe(_ => DecrementCounter())
        .AddTo(this);
    }

    public void IncrementCounter()
    {
        counter++;
        counterTMP.text = counter.ToString();
    }

    public void DecrementCounter()
    {
        counter--;
        counterTMP.text = counter.ToString();
    }
}