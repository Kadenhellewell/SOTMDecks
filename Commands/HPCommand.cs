using System;
using System.Collections.Generic;

namespace SOTMDecks.Commands
{
    /// <summary>
    /// Applies damage or healing to the player, a single target, several
    /// targets, or everything at once. The prior HP of every affected entity is
    /// captured before the change, so Undo restores the exact previous state —
    /// unlike manual reversal, this is immune to the HP clamp at 0 / MaxHP.
    /// </summary>
    internal class HPCommand : Command
    {
        internal enum Scope { Player, Card, Cards, All }

        private readonly Scope scope_;
        private readonly bool isDamage_;

        private int? playerOldHP_;
        private readonly List<(HeroCard card, int oldHP)> cardSnapshots_ = new();

        public HPCommand(Player player, Scope scope, bool isDamage) : base(player)
        {
            scope_ = scope;
            isDamage_ = isDamage;
        }

        public override bool Execute()
        {
            switch (scope_)
            {
                case Scope.Player: return ExecutePlayer();
                case Scope.Card:   return ExecuteCard();
                case Scope.Cards:  return ExecuteCards();
                case Scope.All:    return ExecuteAll();
                default:           return false;
            }
        }

        public override void Undo()
        {
            if (playerOldHP_.HasValue) player_.SetHP(playerOldHP_.Value);

            foreach (var (card, oldHP) in cardSnapshots_)
            {
                card.HP = oldHP;
            }
        }

        private int? GetAmount()
        {
            if (MiscHelpers.GetIntFromPlayer("How much?") is not int amount) return null;

            if (amount <= 0)
            {
                Console.WriteLine("Amount must be positive");
                return null;
            }

            return amount;
        }

        private bool ExecutePlayer()
        {
            if (GetAmount() is not int amount) return false;

            playerOldHP_ = player_.GetHP();
            ApplyToPlayer(amount);
            return true;
        }

        private bool ExecuteCard()
        {
            HeroCard? card = MiscHelpers.GetCardFromIndex(player_.PlayArea());
            if (card is null) return false;

            if (card.MaxHP == 0)
            {
                Console.WriteLine("That card is not a target");
                return false;
            }

            if (GetAmount() is not int amount) return false;

            ApplyToCard(card, amount);
            return true;
        }

        private bool ExecuteCards()
        {
            List<HeroCard>? cards = MiscHelpers.GetCardsFromInput(player_.PlayArea());
            if (cards is null) return false;

            if (GetAmount() is not int amount) return false;

            foreach (HeroCard card in cards)
            {
                if (card.MaxHP == 0)
                {
                    Console.WriteLine($"{card.Name} is not a target (skipping)");
                    continue;
                }

                ApplyToCard(card, amount);
            }

            // Nothing applied (every selected card was a non-target) — don't push a no-op onto the undo stack.
            return cardSnapshots_.Count > 0;
        }

        private bool ExecuteAll()
        {
            if (GetAmount() is not int amount) return false;

            playerOldHP_ = player_.GetHP();
            ApplyToPlayer(amount);

            foreach (HeroCard card in player_.PlayArea().GetCards())
            {
                if (card.MaxHP == 0) continue;
                ApplyToCard(card, amount);
            }

            return true;
        }

        private void ApplyToPlayer(int amount)
        {
            if (isDamage_) player_.DealDamage(amount);
            else           player_.Heal(amount);
        }

        private void ApplyToCard(HeroCard card, int amount)
        {
            cardSnapshots_.Add((card, card.HP));

            if (isDamage_)
            {
                card.HP -= amount;
                if (card.HP <= 0)
                {
                    Console.WriteLine($"{card.Name} has died. If applicable, destroy it.");
                }
            }
            else
            {
                card.HP += amount;
            }
        }
    }
}
