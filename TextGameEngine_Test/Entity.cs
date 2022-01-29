namespace TextGameEngine_Test
{
    class Entity
    {
        public string Name { get; set; }
        public int Health { get; set; }
        protected int Damage { get; set; }

        internal void DecreaseHealth(int damage)
        {
            Health -= damage;

            //don't go below zero
            if (Health < 0)
                Health = 0;
        }

        public virtual int GetAttack()
        {
            return Damage;
        }

        public bool IsDead()
        {
            return Health <= 0;
        }
    }
}
