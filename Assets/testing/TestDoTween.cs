using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Threading.Tasks;
using UnityEngine;

public class TestDoTween : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TestTween();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void TestTween()
    {
        await Task.Delay(1000);

        transform.DOMoveX(3f, 1f).SetEase(Ease.InOutElastic);
        transform.DORestart();
        DOTween.Play(transform);
    }
}
