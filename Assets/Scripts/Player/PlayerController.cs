using UnityEngine;

[ RequireComponent( typeof(PlayerDash) ) ]
[ RequireComponent( typeof(MeleeAttack) ) ]
[ RequireComponent( typeof(StaminaSystem) ) ]


public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Vector2Value startingPosition;

    public float speed = 4f;
    public enum state { Normal, Running, Dashing }
    public state currentState = state.Normal;
    public GameObject animatedObject;

    float currentSpeed = 0f;
    float acceleration = 200f;

    KeyboardStatus keyboardStatus = new KeyboardStatus();
    [HideInInspector]
    public Rigidbody2D rigidBody2D;
    [HideInInspector]
    public StatusEffects statusEffects;
    PlayerDash dash;
    MeleeAttack meleeAttack;
    [HideInInspector]
    public AILifeSystem lifeSystem;
    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public bool exhausted = false;

    public class KeyboardStatus
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;

        public bool keyPressed;

        public bool shift;

        public bool attackingKey;

        public void SetKeyPressed()
        {
            keyPressed = up || down || left || right;
        }
    }


    void SetCurrentSpeed()
    {
        keyboardStatus.SetKeyPressed();

        if ( keyboardStatus.keyPressed)
        {
            currentSpeed = Mathf.Min( speed, currentSpeed + acceleration * Time.deltaTime );
            animator.SetFloat( "Speed", 1 );
            AudioManager.instance.PlayOnLoop("FootstepsGrass", 1.5f);
        }
        else
        {
            currentSpeed = 0f;
            animator.SetFloat( "Speed", 0 );
            AudioManager.instance.Pause("FootstepsGrass");
        }
    }


    public Vector2 GetPlayerDirection()
    {
        Vector2 playerDirection = Vector2.zero;
        if (!CanvasController.instance.GamePaused())
        {
            if (keyboardStatus.up)
            {
                playerDirection += Vector2.up;
                animator.SetFloat("Vertical", 1);
                animator.SetFloat("Horizontal", 0);
            }
            else if (keyboardStatus.down)
            {
                playerDirection += Vector2.down;
                animator.SetFloat("Vertical", -1);
                animator.SetFloat("Horizontal", 0);
            }

            if (keyboardStatus.left)
            {
                playerDirection += Vector2.left;
                animator.SetFloat("Horizontal", -1);
                animator.SetFloat("Vertical", 0);
            }
            else if (keyboardStatus.right)
            {
                playerDirection += Vector2.right;
                animator.SetFloat("Horizontal", 1);
                animator.SetFloat("Vertical", 0);
            }

            playerDirection.Normalize();
        }

        return playerDirection;
    }


    public Vector2 FacingDirection()
    {
        float horizontal = animator.GetFloat( "Horizontal" );
        float vertical = animator.GetFloat( "Vertical" );

        return new Vector2( horizontal, vertical );
    }


    void HandleInput()
    {
        if ( Input.GetKeyDown( KeyCode.W ) ) keyboardStatus.up = true;
        else if ( Input.GetKeyUp( KeyCode.W ) ) keyboardStatus.up = false;

        if ( Input.GetKeyDown( KeyCode.S ) ) keyboardStatus.down = true;
        else if ( Input.GetKeyUp( KeyCode.S ) ) keyboardStatus.down = false;

        if ( Input.GetKeyDown( KeyCode.A ) ) keyboardStatus.left = true;
        else if ( Input.GetKeyUp( KeyCode.A ) ) keyboardStatus.left = false;

        if ( Input.GetKeyDown( KeyCode.D ) ) keyboardStatus.right = true;
        else if ( Input.GetKeyUp( KeyCode.D ) ) keyboardStatus.right = false;

        if ( Input.GetKeyDown( KeyCode.LeftShift ) && currentState != state.Dashing && !exhausted)
        {
            keyboardStatus.shift = true;
            dash.StartBehaviour();
            StaminaSystem.instance.loseStamina(10f);
            currentState = state.Dashing;
        }
        else if ( Input.GetKeyUp( KeyCode.LeftShift ) ) keyboardStatus.shift = false;

        if (Input.GetKeyDown(KeyCode.Space) && StaminaSystem.instance.currentStamina > 0f)
        {
            animator.SetBool("Attacking", true);
            StaminaSystem.instance.loseStamina(20f);
            meleeAttack.Attack();
        }
        else if (Input.GetKeyUp(KeyCode.Space)) animator.SetBool("Attacking", false);
    }


    private void OnCollisionEnter2D( Collision2D collision )
    {
        switch ( collision.gameObject.tag )
        {
            case "Obstacle":

                currentState = state.Normal;

                break;

            case "Enemy":

                Destroy( gameObject );

                break;
        }
    }


    void DashingUpdate()
    {
        if (!dash.dashing)
        {   if (keyboardStatus.shift)
            {
                currentState = state.Running;
            }
            else
                currentState = state.Normal;

        }
        
    }


    void RunningUpdate()
    {
        if (keyboardStatus.shift && StaminaSystem.instance.currentStamina > 0f)
        {
            SetCurrentSpeed();

            Vector2 playerDirection = GetPlayerDirection();

            rigidBody2D.velocity = playerDirection * currentSpeed * 1.5f;

            StaminaSystem.instance.loseStamina(10f * Time.deltaTime);
            AudioManager.instance.PlayOnLoop("FootstepsGrass", 1.8f);

        }
        else
        {
            AudioManager.instance.Pause("FootstepsGrass");
            currentState = state.Normal;
        }

    }


    void NormalUpdate()
    {
        SetCurrentSpeed();

        Vector2 playerDirection = GetPlayerDirection();

        Vector2 finalSpeed = playerDirection * currentSpeed;
        rigidBody2D.velocity = exhausted? finalSpeed * 0.5f : finalSpeed;
    }


    void Update()
    {
        HandleInput();

        switch ( currentState )
        {
            case state.Normal:

                NormalUpdate();

                break;

            case state.Running:

                RunningUpdate();

                break;

            case state.Dashing:

                DashingUpdate();

                break;
        }
    }


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        transform.position = startingPosition.startingPositionOnLoad;

        animator = animatedObject.GetComponent<Animator>();

        rigidBody2D = GetComponent<Rigidbody2D>();
        statusEffects = GetComponent<StatusEffects>();

        dash = GetComponent<PlayerDash>();

        meleeAttack = GetComponent<MeleeAttack>();

        lifeSystem = GetComponent<AILifeSystem>();

        dash.InitBehaviourData();
    }

    private void OnDestroy()
    {
        CanvasController.instance.EndGame();
    }
}