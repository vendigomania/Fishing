using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatchPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider catchProgress;

    [SerializeField] private Slider catchFish;
    [SerializeField] private Slider catchPlayer;
    [SerializeField] private RectTransform catchPlayerZone;
    [SerializeField] private RectTransform fishHandle;

    public UnityAction<bool> OnSuccess;

    bool isDown = false;
    float velocity;
    float weight;

    private float PlayerZoneHeight => (GameData.Instance.CatchZoneEasy + 1) * 0.05f;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
    }

    float fishTarget;

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Catch " + isDown);

        velocity = Mathf.Clamp(velocity + 6 * (isDown ? Time.deltaTime : -Time.deltaTime), -4f, 4f);
        catchPlayer.value = Mathf.Clamp(catchPlayer.value + velocity * Time.deltaTime / 10f, PlayerZoneHeight / 2, 1f - PlayerZoneHeight /2);

        if (Mathf.Abs(fishTarget - catchFish.value) > 0.05f)
        {
            catchFish.value = Mathf.Lerp(catchFish.value, fishTarget, Time.deltaTime * 0.3f * weight);
        }
        else
        {
            fishTarget = Random.Range(0.06f, 1f - 0.06f);
        }
        
        float progressVelocity = (Mathf.Abs(catchPlayer.value - catchFish.value) < PlayerZoneHeight) ?
            Time.deltaTime :
            -Time.deltaTime * (1f / GameData.Instance.DecreaseBreakSpeed);

        catchProgress.value += progressVelocity / 14f;
            

        if (catchProgress.value == 0)
        {
            OnSuccess?.Invoke(false);
            gameObject.SetActive(false);
        }
        else if (catchProgress.value == 1)
        {
            OnSuccess?.Invoke(true);
            gameObject.SetActive(false);
        }
    }

    public void Launch(float _weight)
    {
        weight = _weight;
        catchProgress.value = 0.5f;
        isDown = false;
        velocity = 0f;
        fishTarget = Random.Range(0.06f, 1f - 0.06f);

        catchPlayerZone.sizeDelta = new Vector2(catchPlayerZone.sizeDelta.x, PlayerZoneHeight * 700f);
        fishHandle.localScale = Vector2.one * Mathf.Lerp(0.6f, 1.1f, _weight / 5f);

        gameObject.SetActive(true);
    }
}
