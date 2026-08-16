using UnityEngine;

namespace Chaos.Gameplay.Characters
{
    internal sealed class CharacterMovementJuice : MonoBehaviour
    {
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Transform visorTransform;
        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private float runSpeedThreshold = 6f;
        [SerializeField] private float moveSpeedThreshold = 0.1f;
        [SerializeField] private float groundedVerticalSpeedThreshold = 0.5f;
        [SerializeField] private float walkSquashAmount = 0.12f;
        [SerializeField] private float walkSquashFrequency = 10f;
        [SerializeField] private float idleBreathAmount = 0.02f;
        [SerializeField] private float idleBreathFrequency = 1.5f;
        [SerializeField] private float squashSmoothing = 12f;

        private const float CapsuleLocalHalfHeight = 1f;

        private Vector3 _bodyBaseScale;
        private Vector3 _bodyBasePosition;
        private Vector3 _visorBaseScale;
        private Vector3 _lastPosition;
        private float _squashPhase;
        private bool _isDustPlaying;

        private void Awake()
        {
            _bodyBaseScale = bodyTransform.localScale;
            _bodyBasePosition = bodyTransform.localPosition;
            _visorBaseScale = visorTransform.localScale;
            _lastPosition = transform.position;
        }

        private void Update()
        {
            Vector3 delta = transform.position - _lastPosition;
            _lastPosition = transform.position;

            float verticalSpeed = delta.y / Time.deltaTime;
            delta.y = 0f;
            float horizontalSpeed = delta.magnitude / Time.deltaTime;

            // Derived from observed transform motion (not CharacterController.isGrounded), since Move() only
            // runs on the owner and this component must react correctly on observers too.
            bool isGrounded = Mathf.Abs(verticalSpeed) < groundedVerticalSpeedThreshold;
            bool isMoving = horizontalSpeed > moveSpeedThreshold;
            bool isRunning = horizontalSpeed > runSpeedThreshold;

            UpdateSquash(isMoving);
            UpdateDust(isRunning && isGrounded);
        }

        private void UpdateSquash(bool isMoving)
        {
            float amount = isMoving ? walkSquashAmount : idleBreathAmount;
            float frequency = isMoving ? walkSquashFrequency : idleBreathFrequency;

            _squashPhase += Time.deltaTime * frequency;
            float squash = Mathf.Abs(Mathf.Sin(_squashPhase)) * amount;

            ApplySquash(squash);
        }

        private void ApplySquash(float squash)
        {
            float targetBodyScaleY = _bodyBaseScale.y * (1f - squash);
            Vector3 targetBodyScale = new Vector3(_bodyBaseScale.x, targetBodyScaleY, _bodyBaseScale.z);
            bodyTransform.localScale = Vector3.Lerp(bodyTransform.localScale, targetBodyScale, squashSmoothing * Time.deltaTime);

            // Compensates the pivot-centered scale so the feet stay planted instead of sinking into the floor.
            float targetBodyOffsetY = (_bodyBaseScale.y - targetBodyScaleY) * CapsuleLocalHalfHeight;
            Vector3 targetBodyPosition = _bodyBasePosition - new Vector3(0f, targetBodyOffsetY, 0f);
            bodyTransform.localPosition = Vector3.Lerp(bodyTransform.localPosition, targetBodyPosition, squashSmoothing * Time.deltaTime);

            Vector3 targetVisorScale = new Vector3(_visorBaseScale.x, _visorBaseScale.y * (1f - squash), _visorBaseScale.z);
            visorTransform.localScale = Vector3.Lerp(visorTransform.localScale, targetVisorScale, squashSmoothing * Time.deltaTime);
        }

        private void UpdateDust(bool shouldPlay)
        {
            if (dustParticles == null)
            {
                return;
            }

            if (shouldPlay == _isDustPlaying)
            {
                return;
            }

            if (shouldPlay)
            {
                dustParticles.Play();
            }
            else
            {
                dustParticles.Stop();
            }

            _isDustPlaying = shouldPlay;
        }
    }
}
