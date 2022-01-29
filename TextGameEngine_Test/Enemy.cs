namespace TextGameEngine_Test
{
    class Enemy : Entity
    {
        public int ID { get; set; }
        //-- constructor
        public Enemy()
        {
            Name = "Enemy";
            Health = 1;
            Damage = 1;
        }
    }

}
