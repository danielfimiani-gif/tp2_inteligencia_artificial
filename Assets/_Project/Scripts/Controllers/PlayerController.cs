using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
class PlayerController : MonoBehaviour, IDamageable {
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float maxHealth = 250f;

    [Header("Fire")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int maxAmmo = 12;
    [SerializeField] private float reloadTime = 1.5f;

    [SerializeField] private HealthBar healthBar;

    public static event Action OnPlayerDied;
    public static event Action<int, int> OnAmmoChanged;

    private Rigidbody _rb;
    private Animator _animator;
    private InputSystem_Actions _inputActions;
    private Camera _camera;
    private Plane _floor;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private int _currentAmmo;
    private bool _isReloading;
    private float _nextFireTime;

    public int CurrentAmmo {
        get => _currentAmmo;
        set {
            _currentAmmo = value;
            OnAmmoChanged?.Invoke(_currentAmmo, maxAmmo);
        }
    }

    public float CurrentHealth { get; private set; }

    void Awake() {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _camera = Camera.main;
        _inputActions = new InputSystem_Actions();

        if (_rb) {
            _rb.useGravity = false;
            _rb.constraints = RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        _floor = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        CurrentAmmo = maxAmmo;
        CurrentHealth = maxHealth;
        if (healthBar != null) healthBar.SetValue(CurrentHealth, maxHealth);
    }

    void OnEnable() {
        _inputActions.Player.Enable();
        _inputActions.Player.Fire.started += FireStarted;
        _inputActions.Player.Reload.performed += ReloadPerformed;
    }

    void Update() {
        _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
        _lookInput = _inputActions.Player.Look.ReadValue<Vector2>();


        if (_inputActions.Player.Fire.IsPressed()) {
            TryFire();
        }

        HandleMouseRotation();
        UpdateAnimator();
    }

    void FixedUpdate() {
        HandleMovement();
    }

    void OnDisable() {
        _inputActions.Player.Fire.performed -= FireStarted;
        _inputActions.Player.Reload.performed -= ReloadPerformed;
        _inputActions.Player.Disable();
    }

    void HandleMovement() {
        var direction = new Vector3(_moveInput.x, 0, _moveInput.y);
        if (direction.magnitude > 1) direction = direction.normalized;

        var newPos = _rb.position + Time.fixedDeltaTime * walkSpeed * direction;
        _rb.MovePosition(newPos);
    }

    void HandleMouseRotation() {
        var ray = _camera.ScreenPointToRay(_lookInput);
        if (_floor.Raycast(ray, out float distance)) {
            var point = ray.GetPoint(distance);
            var lookPoint = point - transform.position;
            lookPoint.y = 0;
            if (lookPoint.magnitude > 0.01) {
                var desiredRotation = Quaternion.LookRotation(lookPoint);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    desiredRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    void UpdateAnimator() {
        var worldDirection = new Vector3(_moveInput.x, 0, _moveInput.y);
        var localMovement = transform.InverseTransformDirection(worldDirection);
        _animator.SetFloat("MoveX", localMovement.x, 0.1f, Time.deltaTime);
        _animator.SetFloat("MoveZ", localMovement.z, 0.1f, Time.deltaTime);
        _animator.SetFloat("Speed", worldDirection.magnitude);
    }

    private void FireStarted(InputAction.CallbackContext context) {
        if (_isReloading) return;

        if (CurrentAmmo <= 0) {
            AudioManager.Instance.PlaySFX("FireClick");
        }
    }

    private void TryFire() {
        if (_isReloading) return;
        if (CurrentAmmo <= 0) return;
        if (Time.time < _nextFireTime) return;

        _nextFireTime = Time.time + fireRate;
        CurrentAmmo--;
        ProjectilePool.Instance.GetBullet(firePoint.position, firePoint.rotation);
        _animator.SetTrigger("Fire");
    }

    private void ReloadPerformed(InputAction.CallbackContext context) {
        if (_isReloading) return;

        if (CurrentAmmo == maxAmmo) return;

        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine() {
        _isReloading = true;
        _animator.SetTrigger("Reload");
        yield return new WaitForSeconds(reloadTime);
        CurrentAmmo = maxAmmo;
        _isReloading = false;
    }

    public void ReceiveDamage(float amount) {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0) OnPlayerDied?.Invoke();
        if (healthBar != null) healthBar.SetValue(CurrentHealth, maxHealth);
    }
}