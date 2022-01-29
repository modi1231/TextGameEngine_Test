using System;
using System.Collections.Generic;

namespace TextGameEngine_Test
{

    /* How to add an action: 
     * 1.  plan it out
     * 2.  update state enum
     * 3.  update PrintCurrentCommands
     * 4.  update ProcessInput
     * 5.  update HandleGameState
     * 6.  update PrintBodyText if needed
     * */
    internal class TextGameEngine
    {
        //various game states in response from the user.
        enum STATE
        {
            NONE,
            INVENTORY,
            EXITING,
            PLAYER_HIT,
            USE_ITEM,
            PLAYER_GET_ITEM,
            PLAYER_GET_WEAPON,
            MAKE_ENEMY,
            ATTACK_ENEMY
        }

        private List<Item> _items;//all available items
        Random _r = new Random();//for testing.

        internal void Run()
        {
            // ---- setup ---- 
            STATE currentState = STATE.NONE;
            string input = string.Empty;
            Player player = SetupPlayer("Test Name", 10);

            Enemy enemy = null;

            // ---- load relevant data ---- 
            _items = LoadAllITems();

            //TODO testing - remove
            player.AddItem(_items[0], 1);
            player.AddItem(_items[2], 10);


            // ---- Display header
            PrintHeader(player.Name, player.Health, player.GetAttack());
            PrintCurrentCommands(currentState);

            string bodyText = "";// for any flavor text

            // ---- engine ---- 
            /* The engine continuously runs until it is told to exit.
             * The main three parts is to get the input, process the input, and update the dispaly.
             * */
            while (currentState != STATE.EXITING)
            {
                bodyText = string.Empty;

                //1.  Get input
                PrintColor("> ", ConsoleColor.White);
                input = Console.ReadLine();

                //2.  Process input
                //2.1  Get the new state based on user input.
                currentState = ProcessInput(input, currentState);

                //2.2  Handle special cases.
                HandleGameState(currentState, player, ref bodyText, input, ref enemy);


                //3.  Update UI
                Console.Clear();
                PrintHeader(player.Name, player.Health, player.GetAttack());
                PrintEnemy(enemy);
                PrintCurrentCommands(currentState);
                PrintBodyText(currentState, bodyText, player.Inventory);

                //3.1 Reset certain states when done.
                currentState = TweakState(currentState);
            }
        }


        /// <summary>
        /// Conditional input by state allows reuse of keyboard input but for different things.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="currentState"></param>
        /// <returns></returns>
        private STATE ProcessInput(string input, STATE currentState)
        {
            STATE ret = STATE.NONE;

            if (string.IsNullOrEmpty(input.Trim()))
                return currentState;

            if (currentState == STATE.NONE)
            {
                if (input.ToLower() == "x")
                {
                    ret = STATE.EXITING;
                }
                else if (input.ToLower() == "i")
                {
                    ret = STATE.INVENTORY;
                }
                else if (input.ToLower() == "c")
                {
                    ret = STATE.PLAYER_HIT;
                }
                else if (input.ToLower() == "g")
                {
                    ret = STATE.PLAYER_GET_ITEM;
                }
                else if (input.ToLower() == "w")
                {
                    ret = STATE.PLAYER_GET_WEAPON;
                }
                else if (input.ToLower() == "m")
                {
                    ret = STATE.MAKE_ENEMY;
                }
                else if (input.ToLower() == "a")
                {
                    ret = STATE.ATTACK_ENEMY;
                }
                else // if no valid input keep the current state
                {
                    ret = currentState;
                }
            }
            else if (currentState == STATE.INVENTORY)
            {
                if (input.ToLower() == "x")
                {
                    ret = STATE.EXITING;
                }
                else if (input.ToLower() == "c")
                {
                    ret = STATE.NONE;
                }
                else if (input.ToLower()[0].ToString() == "u")
                {
                    return STATE.USE_ITEM;
                }
                else // if no valid input keep the current state
                {
                    ret = currentState;
                }

            }


            return ret;
        }

        /// <summary>
        /// Sets any information text, updates objects, or properties
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="player"></param>
        /// <param name="bodyText"></param>
        /// <param name="input"></param>
        /// <param name="enemy"></param>
        private void HandleGameState(STATE currentState, Player player, ref string bodyText, string input, ref Enemy enemy)
        {
            if (currentState == STATE.EXITING)
            {
                bodyText = "Exiting...";
            }
            else if (currentState == STATE.PLAYER_HIT)
            {
                int damage = GetDamage();
                player.DecreaseHealth(damage);
                bodyText = $"Player takes {damage} damage.";

            }
            else if (currentState == STATE.USE_ITEM)
            {
                var temp = input.Split(' ');
                if (temp != null && temp.Length == 2)
                {
                    int itemNumber = int.Parse(temp[1]);

                    bodyText = $"Player uses {player.UseItem(itemNumber - 1)}.";//arrays start at 0, but printed inventory starts at 1.. so decrement
                }
                else
                {
                    bodyText = "Invalid Selection";
                }

            }
            else if (currentState == STATE.PLAYER_GET_ITEM)
            {
                Item temp = GetRandomItem(_items);

                player.AddItem(temp, 1);
                bodyText = $"Player recieves {temp.Name}.";
            }
            else if (currentState == STATE.PLAYER_GET_WEAPON)
            {
                Weapon temp = GenerateWeapon();
                player.SetWeapon(temp);

                bodyText = $"Player equips {temp.Name}.";
            }
            else if (currentState == STATE.MAKE_ENEMY)
            {
                enemy = GenerateEnemy();
            }
            else if (currentState == STATE.ATTACK_ENEMY)
            {
                if (enemy == null)
                {
                    bodyText = "There is no enemy to attack.";
                }
                else
                {
                    if (!enemy.IsDead())
                    {
                        bodyText = $"Player attacks for {player.GetAttack()}.\n";
                        enemy.DecreaseHealth(player.GetAttack());
                        if (!enemy.IsDead())
                        {
                            bodyText += $"{enemy.Name} attacks for {enemy.GetAttack()}.\n";
                            player.DecreaseHealth(enemy.GetAttack());
                        }
                    }
                }
            }
        }

        private Weapon GenerateWeapon()
        {
            //TODO testing - remove

            Weapon temp = new Weapon();
            temp.AttackBonus = 5;
            temp.ID = 100;
            temp.Name = "Sword";
            return temp;
        }

        private STATE TweakState(STATE currentState)
        {
            STATE ret = currentState;

            if (currentState == STATE.PLAYER_HIT ||
                currentState == STATE.PLAYER_GET_ITEM ||
                currentState == STATE.PLAYER_GET_WEAPON ||
                currentState == STATE.MAKE_ENEMY ||
                currentState == STATE.ATTACK_ENEMY)
                ret = STATE.NONE;

            if (currentState == STATE.USE_ITEM)
                ret = STATE.INVENTORY;

            return ret;
        }
        private Enemy GenerateEnemy()
        {
            //TODO testing - remove
            Enemy temp = new Enemy();
            temp = new Enemy();
            temp.ID = 1;
            temp.Name = "Goblin";
            temp.Health = _r.Next(5);


            return temp;
        }
        private Item GetRandomItem(List<Item> items)
        {
            int foo = _r.Next(items.Count);

            return items[foo];
        }
        private int GetDamage()
        {
            return _r.Next(1, 4);
        }
        private Player SetupPlayer(string name, int health)
        {
            Player ret = new Player();
            ret.Name = name;
            ret.Health = health;
            return ret;
        }

        /// <summary>
        /// Information could be loaded from a file.
        /// </summary>
        /// <returns></returns>
        private List<Item> LoadAllITems()
        {
            //TODO testing - remove

            List<Item> ret = new List<Item>();

            ret.Add(new Item()
            {
                ID = 1,
                Name = "Test Item 1"
            });
            ret.Add(new Item()
            {
                ID = 2,
                Name = "Test Item 2"
            });
            ret.Add(new Item()
            {
                ID = 3,
                Name = "Test Item 3"
            });

            return ret;
        }


        /// <summary>
        /// displays any flavor text or conditionally colors said text
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="bodyText"></param>
        /// <param name="inventory"></param>
        private void PrintBodyText(STATE currentState, string bodyText, List<Item_Inventory> inventory)
        {
            if (currentState == STATE.EXITING)
                PrintLineColor(bodyText, ConsoleColor.Yellow);
            else if (currentState == STATE.INVENTORY)
                PrintInventory(inventory);
            else if (currentState == STATE.PLAYER_HIT)
                PrintLineColor(bodyText, ConsoleColor.Red);
            else if (currentState == STATE.USE_ITEM)
            {
                PrintInventory(inventory);
                PrintLineColor(bodyText, ConsoleColor.White);
            }
            else if (currentState == STATE.PLAYER_GET_ITEM)
            {
                PrintLineColor(bodyText, ConsoleColor.White);
            }
            else if (currentState == STATE.PLAYER_GET_WEAPON)
            {
                PrintLineColor(bodyText, ConsoleColor.White);
            }
            else
            {
                if (bodyText.Trim() != string.Empty)
                    PrintLineColor(bodyText, ConsoleColor.White);
            }


        }
        private void PrintCurrentCommands(STATE currentState)
        {
            PrintLineColor("Commands:", ConsoleColor.DarkGray);

            //in what state does this command list show up
            if (currentState == STATE.NONE ||
                currentState == STATE.PLAYER_HIT ||
                currentState == STATE.PLAYER_GET_ITEM ||
                currentState == STATE.PLAYER_GET_WEAPON ||
                currentState == STATE.MAKE_ENEMY ||
                currentState == STATE.ATTACK_ENEMY
                )
            {
                PrintLineColor("(x) to Exit\n" +
                    "(i) for Inventory\n" +
                    "(c) player takes damage\n" +
                    "(g) to get random item\n" +
                    "(w) to get Weapon\n" +
                    "(m) to make enemy\n" +
                    "(a) to attack enemy", ConsoleColor.DarkGray);
            }
            else if (currentState == STATE.INVENTORY || currentState == STATE.USE_ITEM)
            {
                PrintLineColor("(x) to Exit\n(c) for Close Inventory\n(u #) to Use Item", ConsoleColor.DarkGray);
            }
        }
        private void PrintInventory(List<Item_Inventory> inventory)
        {
            PrintLineColor("=====================", ConsoleColor.DarkYellow);
            PrintLineColor("Inventory", ConsoleColor.DarkYellow);
            if (inventory == null || inventory.Count == 0)
            {
                PrintColor("No items in inventory", ConsoleColor.White);
            }
            else
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    PrintColor($"{i + 1}. {inventory[i].Item.Name}", ConsoleColor.White);
                    PrintColor($" ({inventory[i].Quantity})", ConsoleColor.DarkRed);
                    Console.WriteLine();
                }
            }


            //  Console.WriteLine();
            PrintLineColor("=====================", ConsoleColor.DarkYellow);
        }
        private void PrintEnemy(Enemy enemy)
        {
            if (enemy != null)
            {
                if (enemy.Health == 0)
                {
                    PrintColor("| ", ConsoleColor.Green);
                    PrintColor($"Enemy: ", ConsoleColor.DarkRed);
                    PrintColor(enemy.Name, ConsoleColor.White);
                    PrintColor(" is dead.", ConsoleColor.DarkRed);
                    PrintLineColor(" | ", ConsoleColor.Green);

                }
                else
                {
                    PrintColor("| ", ConsoleColor.Green);
                    PrintColor($"Enemy: ", ConsoleColor.DarkRed);
                    PrintColor(enemy.Name, ConsoleColor.White);
                    PrintColor(" | ", ConsoleColor.Green);
                    PrintColor($"Health: ", ConsoleColor.DarkRed);
                    PrintColor(enemy.Health.ToString(), ConsoleColor.White);
                    PrintColor(" | ", ConsoleColor.Green);
                    PrintColor($"Attack: ", ConsoleColor.DarkRed);
                    PrintColor(enemy.GetAttack().ToString(), ConsoleColor.White);
                    PrintLineColor(" | ", ConsoleColor.Green);
                }
            }
        }
        private void PrintHeader(string name, int health, int attack)
        {
            PrintLineColor("-----------------------------------------------", ConsoleColor.Green);
            PrintColor("| ", ConsoleColor.Green);

            PrintColor("Name: ", ConsoleColor.Blue);
            PrintColor(name, ConsoleColor.White);
            PrintColor("  | ", ConsoleColor.Green);

            PrintColor("Health: ", ConsoleColor.Red);
            PrintColor(health.ToString(), ConsoleColor.White);
            PrintColor("  | ", ConsoleColor.Green);

            PrintColor("Attack: ", ConsoleColor.Magenta);
            PrintColor(attack.ToString(), ConsoleColor.White);

            PrintLineColor("  | ", ConsoleColor.Green);
            PrintLineColor("-----------------------------------------------", ConsoleColor.Green);
            //Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
        }
        private void PrintColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }
        private void PrintLineColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }
    }
}
