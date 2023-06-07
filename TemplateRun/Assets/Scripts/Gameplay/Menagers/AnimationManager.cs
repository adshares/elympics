using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private JumpManager jumpManager;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;

    [Header("DeathAnimation")]
    [SerializeField] private float deathFreezeDuration;
    [SerializeField] private float deathBounceInitialSpeed;
    [SerializeField] private float deathScreenActivateDelay;
    [SerializeField] private Vector2 deathBounceNoise;
    [SerializeField] private GameObject deathSprite;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private float deathSpriteGravity;
    private Vector2 currentSpriteVelocity;
    private bool dead = false;
    private bool bounceStarted = false;
    private Transform deathSpriteTransform;

    private void Start()
    {
        gameStateSynchronizer.SubscribeToGameStateChange(PlayDeathAnimation);
        deathSpriteTransform = deathSprite.GetComponent<Transform>();
    }
    void Update()
    {
        if (dead) SimulateDeathBounce(Time.deltaTime);
        else
        {
            playerAnimator.SetFloat("yVelocity", jumpManager.PlayerRigidbody.velocity.y);
            playerAnimator.SetBool("Grounded", jumpManager.IsGrounded());
        }
    }

    private void PlayDeathAnimation(int oldState, int newState)
    {
        if ((GameState)newState != GameState.GameEnded) return;
        playerSpriteRenderer.enabled = false;
        deathSprite.SetActive(true);
        dead = true;
        Invoke("StartDeathBounce", deathFreezeDuration);
        Invoke("ActivateEndGameScreen", deathScreenActivateDelay);
    }

    private void StartDeathBounce()
    {
        currentSpriteVelocity = Vector2.up * deathBounceInitialSpeed + new Vector2(Random.Range(-deathBounceNoise.x / 2, deathBounceNoise.x / 2), Random.Range(-deathBounceNoise.y / 2, deathBounceNoise.y / 2));
        bounceStarted = true;
    }

    private void SimulateDeathBounce(float deltaTime)
    {
        if (!bounceStarted) return;
        currentSpriteVelocity += Vector2.down * deathSpriteGravity * deltaTime;
        deathSpriteTransform.position += new Vector3(currentSpriteVelocity.x, currentSpriteVelocity.y, 0) * deltaTime;
    }

    private void ActivateEndGameScreen()
    {
        endGameScreen.SetActive(true);
    }
}
