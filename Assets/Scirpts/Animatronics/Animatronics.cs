using System;
using System.Collections;
using UnityEngine;

public class Animatronics : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private string charName;
    [SerializeField] private int minNoiseForce;
    [SerializeField] private int maxNoiseForce;
    [SerializeField] private int spawnDistance;
    [SerializeField] private int minInitialPauseSecond;
    [SerializeField] private int maxInitialPauseSecond;
    [SerializeField] private int hp;
    [SerializeField] private int jumpScareTime;
    [SerializeField] private int minshockTime;
    [SerializeField] private int maxshockTime;
    [SerializeField] private int minCircleDegreesPerSecond;
    [SerializeField] private int maxCircleDegreesPerSecond;
    [SerializeField] private int circleMoveTime;
    [SerializeField] private int minPauseSecond;
    [SerializeField] private int maxPauseSecond;
    [SerializeField] private int chanceToCharge;
    [SerializeField] private int chanceToJumpScare;
    [SerializeField] private int chanceToFeint;
    [SerializeField] private int chargeTime;
    [SerializeField] private int invisibleTime;
    [SerializeField] private int cloackedTime;
    [SerializeField] private int deCloackedTime;
    [SerializeField] private int minRepositionAngleDegrees;
    [SerializeField] private int maxRepositionAngleDegrees;

    private Animator animator;
    private bool isFinishCircleMove;
    private bool alreadyinit;
    public AudioClip[] audioClips;
    public AudioSource audioSource;

    public Camera camera;

    public event Action OnSoundPlayFinished;
    public event Action OnVisibleFinished;

    public string[] visibleAnimationNames = { "FreddyGlimpse1", "FreddyGlimpse2", "FreddyGlimpse3"};

    public StateMachine StateMachine { get; private set; }

    [SerializeField] private Material bodyShader;
    [SerializeField] private Material eyeShader;
    private float bodyAlpha;
    private float eyeAlpha;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        AnimatronicsInit(id);

        bodyShader.SetFloat("_Alpha", 1);
        eyeShader.SetFloat("_Alpha", 0.5f);

        isFinishCircleMove = false;
        alreadyinit = false;
    }

    public void SetVisible()
    {
        bodyShader.SetFloat("_Alpha", 0);
        eyeShader.SetFloat("_Alpha", 0f);
        StartCoroutine(SetInvisible());
    }

    IEnumerator SetInvisible()
    {
        yield return new WaitForSeconds(invisibleTime);
        bodyShader.SetFloat("_Alpha", 1);
        eyeShader.SetFloat("_Alpha", 0.5f);
        OnVisibleFinished?.Invoke();
    }

    public bool ShouldCharge()
    {
        return UnityEngine.Random.Range(0, 100) < chanceToCharge;
    }

    public bool ShouldJumpScare()
    {
        return UnityEngine.Random.Range(0, 100) < chanceToJumpScare;
    }

    public bool HpCheck()
    {
        if (hp > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AnimatronicsInit(int _id)
    {
        var animatronicsTable = DataManager.Instance.AnimatronicsTable[_id];
        charName = animatronicsTable.charName;
        minNoiseForce = animatronicsTable.minNoiseForce;
        maxNoiseForce = animatronicsTable.maxNoiseForce;
        spawnDistance = animatronicsTable.spawnDistance;
        minInitialPauseSecond = animatronicsTable.minInitialPauseSecond;
        maxInitialPauseSecond = animatronicsTable.maxInitialPauseSecond;
        hp = animatronicsTable.hp;
        jumpScareTime = animatronicsTable.jumpScareTime;
        minshockTime = animatronicsTable.minShockTime;
        maxshockTime = animatronicsTable.maxShockTime;
        minCircleDegreesPerSecond = animatronicsTable.minCircleDegreesPerSecond;
        maxCircleDegreesPerSecond = animatronicsTable.maxCircleDegreesPerSecond;
        circleMoveTime = animatronicsTable.circleMoveTime;
        minPauseSecond = animatronicsTable.minPauseSecond;
        maxPauseSecond = animatronicsTable.maxPauseSecond;
        chanceToCharge = animatronicsTable.chanceToCharge;
        chanceToJumpScare = animatronicsTable.chanceToJumpScare;
        chanceToFeint = animatronicsTable.chanceToFeint;
        chargeTime = animatronicsTable.chargeTime;
        invisibleTime = animatronicsTable.invisibleTime;
        cloackedTime = animatronicsTable.cloackedTime;
        deCloackedTime = animatronicsTable.deCloackedTime;
        minRepositionAngleDegrees = animatronicsTable.minRepositionAngleDegrees;
        maxRepositionAngleDegrees = animatronicsTable.maxRepositionAngleDegrees;
    }

    public void PlayAnimation(string animationName)
    {
        if (animator != null)
        {
            animator.Play(animationName);
        }
    }

    public int WaitPauseSecond()
    {
        if (alreadyinit)
        {
            return UnityEngine.Random.Range(minPauseSecond, maxPauseSecond);
        }
        else
        {
            alreadyinit = true;
            return UnityEngine.Random.Range(minInitialPauseSecond, maxInitialPauseSecond);
        }
    }

    public string GoIdleToAnotherState()
    {
        int ran = UnityEngine.Random.Range(0, 100);
        string state = "";

        if (ran < chanceToCharge)
        {
            state = "chargeState";
        }
        else if (ran < chanceToCharge + chanceToFeint)
        {
            state = "feintState";
        }
        else if (ran >= chanceToCharge + chanceToFeint)
        {
            state = "circleMoveState";
        }
        return state;
    }

    public string GoFeintToAnotherState()
    {
        int ran = UnityEngine.Random.Range(0, 100);
        string state = "";

        if (ran <= 60)
        {
            state = "soundFeintState";
        }
        else if (ran > 60)
        {
            state = "invisibleFeintState";
        }

        return state;
    }

    public void ShaderSetAlphaValue()
    {
        bodyAlpha = bodyShader.GetFloat("_Alpha");
        eyeAlpha = eyeShader.GetFloat("_Alpha");

        if (bodyAlpha > 0 || eyeAlpha > 0)
        {
            bodyAlpha -= 0.03f;
            eyeAlpha -= 0.03f;
            bodyShader.SetFloat("_Alpha", bodyAlpha);
            eyeShader.SetFloat("_Alpha", eyeAlpha);
        }
    }

    public void MoveCircle()
    {
        isFinishCircleMove = false;
        StartCoroutine(MoveCircleOneSecond());
    }

    IEnumerator MoveCircleOneSecond()
    {
        float elapsedTime = 0;
        int degree = RotateDegree(minCircleDegreesPerSecond, maxCircleDegreesPerSecond);

        while (elapsedTime < circleMoveTime)
        {
            transform.RotateAround(Vector3.zero, Vector3.up, degree);
            yield return new WaitForSeconds(1f);
            elapsedTime++;
        }
        isFinishCircleMove = true;

    }

    public bool IsFinishCircleMove()
    {
        if (isFinishCircleMove)
        {
            isFinishCircleMove = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public int RotateDegree(int minDegrees, int maxDegrees)
    {
        return UnityEngine.Random.Range(minDegrees, maxDegrees);

    }

    public void RotateReposition()
    {
        int degree = RotateDegree(minRepositionAngleDegrees, maxRepositionAngleDegrees);
        transform.RotateAround(Vector3.zero, Vector3.up, degree);
    }

    public void PlaySoundFeint()
    {
        int ran = UnityEngine.Random.Range(0, audioClips.Length);
        AudioClip clip = audioClips[ran];
        StartCoroutine(CheckSoundPlayFinished(clip.length));
        GetComponent<AudioSource>().PlayOneShot(clip);
    }

    IEnumerator CheckSoundPlayFinished(float length)
    {
        yield return new WaitForSeconds(length);
        OnSoundPlayFinished?.Invoke();
    }

    public bool IsFindVisibleAnimatronics()
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);

        bool isVisible = viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;

        return isVisible;
    }

    public string selectVisibleAnimation()
    {
        int ran = UnityEngine.Random.Range(0, visibleAnimationNames.Length);
        string visibleanimName = visibleAnimationNames[ran];

        return visibleanimName;
    }


}
