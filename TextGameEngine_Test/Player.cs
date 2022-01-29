using System.Collections.Generic;

namespace TextGameEngine_Test
{
    /// <summary>
    /// Holds relevant player information
    /// </summary>
    class Player : Entity
    {
        //-- properties
        public List<Item_Inventory> Inventory { get; set; }

        private Weapon _weapon;

        //-- constructor
        public Player()
        {
            Name = "";
            Health = 1;
            Inventory = new List<Item_Inventory>();
            Damage = 1;
        }

        //-- functions and methods
        internal void AddItem(Item item, int quantity)
        {
            bool temp = false;

            foreach (var foo in Inventory)
            {
                if (foo.Item.ID == item.ID)
                {
                    foo.Quantity += quantity;
                    temp = true;
                    break;
                }
            }

            if (!temp)
            {
                Inventory.Add(new Item_Inventory()
                {
                    Item = item,
                    Quantity = quantity
                });
            }
        }

        internal string UseItem(int itemPosition)
        {
            string ret = string.Empty;
            if (itemPosition > Inventory.Count)
                return "";

            //find item.
            //return name
            ret = Inventory[itemPosition].Item.Name;

            //decrease quantity
            Inventory[itemPosition].Quantity -= 1;

            if (Inventory[itemPosition].Quantity <= 0)
            {
                Inventory.RemoveAt(itemPosition);
            }



            return ret;
        }

        public override int GetAttack()
        {
            int temp = Damage;

            if (_weapon != null)
                temp += _weapon.AttackBonus;

            return temp;
        }

        public void SetWeapon(Weapon weapon)
        {
            _weapon = weapon;
        }
    }
}
