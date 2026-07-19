# Chaos Mode — Raw Ideas

We already have a decent little game going. Now it's time to add spice: the **CHAOTIC mode**, which will be the game's differentiator.

## Chaotic items

Some items can spawn "chaotic," meaning they can trigger events and interactions with players. There's a random (not too rare) chance that a pickup item spawns with a different behavior. Example behaviors below:

- **Fleeing item** — When it sees a player approaching, it tries to flee by hopping. It's not too fast, but it should add a fun challenge for the player: it keeps hopping away and dodging until it's finally caught. If the player holds the item too long without delivering it, the item auto-drops from the player's hand and starts fleeing by hopping again.

- **Funny robot item** — When you get close to it, it transforms into a little robot with a shovel. If the shovel hits the player, they get launched far away (Heave Ho / Mario-style). No visuals need to be implemented for now.

- **Teleporting item** — If you pick up the item and hold it for a few seconds, there's a chance it checks the game to see if another player is holding another item. If so, it darkens both players' screens and a big magic eye appears in the center of the screen, with a dark purple, mystical/witchcraft-themed background. After a few seconds of this, the item swaps the holding player's position with the other player who was also holding an item — a full position swap. Now the chaotic item is in the other player's hands, who's disoriented and doesn't quite know where to go, while the item that was in player 2's hand is now in player 1's hand (the one who originally picked up the teleport item). From the first swap onward, the item stays active, and every so many seconds it re-checks for another player and swaps again. The idea is that this item makes it *harder* for players to drop it into the collection container — without being frustrating. It must not teleport too far from the container: if players are already close to delivering it and it teleports them next to a player way across the map, that's extremely annoying. So it never teleports too far, and it's somewhat random whether it triggers multiple swaps or not.

- **Heavy item** — Reduces the carrying player's movespeed by 20%. Can be carried solo, but two players carrying it together removes the debuff entirely (much faster).

- **Shy item** — Like Boo from Mario: becomes intangible and non-interactable if a player approaches too fast. You have to approach slowly.

- **Toaster item** — Heats up over time even while unplugged, and can become too hot to hold (auto-drops the holder). Needs a bucket of water (or similar pickup) to cool it back down.

- **Light but fragile item** — Cannot be dropped at all.

- **Tall and fragile item** — Sways when the carrying player makes aggressive turns (sways to the *opposite* side of the turn direction). If the turn is aggressive enough, the item topples, falls to the ground, and breaks permanently.

## Player mechanics

- **Slap button** — Lets a player slap teammates, triggering their ragdoll. Can also be used on items (they're cute, with different expressions when slapped). Similar to the slap mechanic in *Moving Out*, or the co-op house-painting game similar to *Overcooked*.
  - If a player is carrying an item and gets slapped, they drop it (and it may even break, depending on the item).
  - Fleeing items get stunned when slapped and stop fleeing for a while.
  - Possible future addition: carts.

- **Score = money** — Scores are actually money, and players need to hit a target quota. Money is earned via commission, so it's intentionally fine to slap teammates to make them drop their item — you can then pick it up and sell it yourself to collect the commission instead of them.
  - Outside of matches, money can be spent on cosmetics or boosts (TBD).
