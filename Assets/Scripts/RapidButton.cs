using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UniRx;
using UnityEditor;

public class RapidButton : Button, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    /// <summary>
    /// 連射処理を扱う
    /// </summary>
    private class RapidFire
    {
        private float counter;
        readonly private float interval;
        readonly private UnityAction onRapidFire;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RapidFire(float interval, UnityAction onRapidFire)
        {
            this.interval = interval;
            this.onRapidFire = onRapidFire;
        }

        /// <summary>
        /// カウントし、インターバルだけ数えていたらイベント通知
        /// </summary>
        public void CountAndFire(float deltaTime)
        {
            counter += deltaTime;
            if (counter >= interval) onRapidFire?.Invoke();

            // インターバルを超えたらカウントをリセット
            counter = counter % interval;
        }
    }
    
    [SerializeField] private float firstInterval = 0.5f;
    [SerializeField] private float rapidInterval = 0.1f;

    /// <summary> 連射でOnNextを発行 </summary>
    public IObservable<Unit> OnRapidFire => onRapidFire;
    private readonly Subject<Unit> onRapidFire = new Subject<Unit>();

    private bool isPressed;
    private RapidFire currentRapidFire;
    
    private void Update()
    {
        if (!isPressed) return;
        
        // 一定秒数以上カウントしたら発火
        currentRapidFire?.CountAndFire(Time.deltaTime);
    }

    public override void OnPointerDown(PointerEventData _)
    {
        base.OnPointerDown(_);
        isPressed = true;
        onRapidFire.OnNext(Unit.Default); // 押し下げた瞬間にも1回OnNext
        currentRapidFire = new RapidFire(firstInterval, OnIntroHold);

        // 押下から連射開始まで適用
        void OnIntroHold()
        {
            onRapidFire.OnNext(Unit.Default);
            currentRapidFire = new RapidFire(rapidInterval, OnSubsequentHold);
        }

        // 連射開始から押上まで適用
        void OnSubsequentHold() => onRapidFire.OnNext(Unit.Default);
    }

    // ボタンを離したり退出したりしたらリセット
    public override void OnPointerUp(PointerEventData _)
    {
        base.OnPointerUp(_);
        isPressed = false;
        currentRapidFire = null;
    }
    public override void OnPointerExit(PointerEventData _)
    {
        base.OnPointerExit(_);
        isPressed = false;
        currentRapidFire = null;
    }

    // 一応、継承元のonClickにAddListenerしても通知するようにしてある
    protected override void Awake()
    {
        base.Awake();
        onRapidFire.Subscribe(_ => onClick?.Invoke()).AddTo(this);
    }

    // 継承元の通常OnPointerClickは処理が被るので握り潰す
    public override void OnPointerClick(PointerEventData _){}
}

#if UNITY_EDITOR
[CustomEditor(typeof(RapidButton))]
public class RapidButtonEditor : UnityEditor.UI.ButtonEditor
{
    SerializedProperty firstIntervalProp;
    SerializedProperty rapidIntervalProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        firstIntervalProp = serializedObject.FindProperty("firstInterval");
        rapidIntervalProp = serializedObject.FindProperty("rapidInterval");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(firstIntervalProp, new GUIContent("First Interval"));
        EditorGUILayout.PropertyField(rapidIntervalProp, new GUIContent("Rapid Interval"));
        serializedObject.ApplyModifiedProperties();
    }
}
#endif