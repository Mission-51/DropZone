using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public enum BulletType
    {
        Normal,
        Ice,        // 슬로우
        Gunpowder   // 폭발탄
    }

    public bool isPiercing; // 관통탄 여부
    public BulletType bulletType; // 발사체의 속성
    public float explosionRadius = 3f; // 폭발탄의 반경
    public int damage; // 총알의 데미지 값
        
    public float lifeTime; // 총알의 생명 시간
    public GameObject shooter; // 발사체를 발사한 주체 (자기 자신을 식별하기 위한 변수)

    public GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹
    public float effectOffset = 1.5f; // 이펙트 위치 조정을 위한 오프셋

    public Color normalColor = Color.yellow;
    public Color iceColor = Color.blue;
    public Color gunpowderColor = Color.red;

    private Renderer bulletRenderer;
    public int shooterViewID;

    void Awake()
    {
        bulletRenderer = GetComponent<Renderer>(); // 발사체의 렌더러를 가져옴
    }

    void Start()
    {       

        // 발사체의 속성에 따른 초기 설정
        ApplyBulletProperties();       

        // 일정 시간이 지나면 총알 파괴
        Destroy(gameObject, lifeTime);

    }

    private void ApplyBulletProperties()
    {
        // 발사체의 속성에 따라 색상 변경
        switch (bulletType)
        {
            case BulletType.Normal:
                bulletRenderer.material.color = normalColor;
                break;
            case BulletType.Ice:
                bulletRenderer.material.color = iceColor;
                break;
            case BulletType.Gunpowder:
                bulletRenderer.material.color = gunpowderColor;
                break;
        }
    }
    private bool hasHit = false; // 이미 충돌했는지 확인하는 플래그

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;  // 이미 충돌했으면 더 이상 처리하지 않음

        PlayerStatus enemyStatus = other.gameObject.GetComponent<PlayerStatus>();

        if (enemyStatus != null && other.gameObject != shooter)
        {
            // shooterViewID를 사용하여 데미지를 적용
            //enemyStatus.photonView.RPC("TakeDamage", RpcTarget.All, damage, shooterViewID);
            enemyStatus.TakeDamage(damage, shooterViewID); 
            Debug.Log($"총알 데미지 {damage} 적용됨");

            hasHit = true; // 충돌 발생 플래그 설정
        }

        // 특수 효과 적용
        if (bulletType == BulletType.Ice && other.gameObject.CompareTag("Player"))
        {
            // 슬로우 효과 적용
            PlayerStatus targetStatus = other.gameObject.GetComponent<PlayerStatus>();
            if (targetStatus != null)
            {
                targetStatus.ApplyStatusEffect(PlayerStatus.StatusEffect.Slow2);
            }
        }
        else if (bulletType == BulletType.Gunpowder)
        {
            // 폭발탄 효과 적용
            Explode();
        }
                
        // 관통탄이 아닌 경우에만 파괴
        if (isPiercing)
        {
            return;
        }

        if (other.gameObject.CompareTag("Untagged") || bulletType != BulletType.Gunpowder || other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

    }

    private void Explode()
    {
        Vector3 effectPosition = transform.position + transform.forward * effectOffset;

        // 폭발 이펙트 생성
        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, effectPosition, Quaternion.identity);

            // 이펙트가 일정 시간 후에 파괴되도록 설정 
            Destroy(explosionEffect, 1f); // 폭발 이펙트가 1초 후 파괴되도록 설정
        }

        // 폭발탄의 효과를 적용하는 로직
        Collider[] hitColliders = Physics.OverlapSphere(effectPosition, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            PlayerStatus targetStatus = hitCollider.GetComponent<PlayerStatus>();
            if (targetStatus != null && hitCollider.gameObject != shooter) // 자기 자신을 제외
            {
                targetStatus.TakeDamage(10, shooterViewID); // 폭발탄의 데미지
            }
        }

        // 폭발 후 발사체 파괴
        if (!isPiercing)
        {
            Destroy(gameObject);
        }
    }
        
}