using UnityEngine;
namespace EntityForTest
{
    public class EntityForTest : Enemy
    {
        protected AudioPlayer player;
        [SerializeField]private Barrage basketball;
        float timer;
        protected override void Start()
        {
            base.Start();
            player = GameObject.FindWithTag("Audio").GetComponent<AudioPlayer>();
            player.PlayCreatureSound(CreatureType.Test, SoundType.Idle);
        }
        protected override void Update()
        {
            timer += Time.deltaTime;
            if (timer > 2)
                { Shoot(); timer =0; player.PlayCreatureSound(CreatureType.Test, SoundType.Attack); }
            base.Update();
            if(currentHealth<=0)
            {
                entityAnimator.SetBool("Death", true);
            }
        }
        protected override void HitEvent()
        {
            base.HitEvent();
            player.PlayCreatureSound(CreatureType.Test, SoundType.Death);
        }
        void Shoot()
        {
            GameObject.Instantiate(basketball, transform.position + new Vector3(1f,0, 0), Quaternion.identity);
        }
    }
}