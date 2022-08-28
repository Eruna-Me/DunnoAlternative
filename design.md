# DunnoAlternative

## Pre (what should this be called?)

In this file some statements will be prefixed with "stretch goal: " 
these MIGHT be implemented once the main functionality (everything without the prefix) has been implemented.

At this point a lot of this document is still open for change, feel free to suggest alternative implementations

## In short:

A single player game in which the player assembles a cast of heroes and monsters and uses those to conquer a randomly generated world, 
filled with AI who try to do the same, with equal tools.
```The AI should not be "provided" with more (relevant) information than the player.```

The game will be split between a world map, en a battle map. Conquering the world should take about a few hours for an experienced player (and not longer! Important!).
```Important to note that there should probably be a proper saving / auto-saving system and probably some handling of abrupt shutdowns. Unless you expect players to finish the whole game in one sitting```
Battles should take 5 minutes at most, and a fastforward should be present to massively speed up battles. 

Most of the important decisions will be made in the world mode, in the form of deciding what heroes and monsters to recruit and where to deploy them.
Battles will be mostly automatic under AI control, with minimal user input.

### Stretch goal: 

online multiplayer

## Target platforms:

Windows, Linux

### Stretch goal: 

Android ```, Web```

## Technical:

It will either be made in C# and SFML or in Rust (probably with Bevy?)
Both battles and the world will be 2d ```Top down view``` grid based. Troops in battles will use static (unanimated) 2d sprites. ```At least basic animation maybe, but rotation will probably do```
Instead of animations particle effects, and sprite movements will be used to make the battlefield look somewhat alive.

A decision will not be made whether the resolution will be static, and the whole world/battle will be visible at times. 
Or to add scrolling, zooming, resolution options.
```We should probably do something that can handle scaling to a certrain degree. making it static will not age well```

## Game loop:

New game -> select starting Hero, etc

Turn ->
- Invade -> both choose heroes + monsters
- Build stuff  
- Recruit 
- etc
    
Next players turn, repeat

# Shared Data:

Both players can send at most 3 heroes (1 for each flank; left, right, center). And associate 2 formations of monsters to each hero.
```How will deze formations look and how will they "spawn" in the battle area etc, plz include pics```

# World:

```There is nothing about the world generation here, (there was a bit in the "In Short" section, but not enough). I assume the world is not infinite, because it would be very complex and probably requires giving the AI unfair advantages. I also wonder if the player gets to see purely AI involved battles. I would assume the AI's also battle each other, but as far as I'm aware, that is not mentioned explicitly.```

Turn based, turns (and battles) will take place sequentially. 
At the start of a turn the player gets a limited amount of resources to spend during their turn: 
- Money from their provinces, this is the only resource that carries over to following turns.
- A limited amount of actions/attacks. (maybe 2), might also get used for construction. ```Can we build on specific world tiles!? Or is that a stretch goal?```
- Every hero (and their attached monsters) may fight once per turn (and only defend during other players turns if they didn't attack!).

With these resources they have to each turn, if so desired:
- Invade other factions territories.
- Build province upgrades: either economic upgrades, or (better) castles 
- Hire new (wandering?) heroes
```Maybe building housing at a cost will provide you with x heroes or something. I dunno```
- Buy new monster cards
- Levelup/upgrade monsters/heroes? (if leveling gets added)

## Monster (recruitment)
```Aren't they like mobs or pets.. or slaves? Ok maybe not use that last one that will get us cancelled```

Despite the name these may also just be human troops. They are organized in cards with a select amount of a certain monster.

Buying monster cards costs money depending on the card. Only a limited amount of the same card may be bought, sometimes only 1 sometimes a couple (maybe 3 or 5).

Cards are found in thematic decks (example: human deck, slime deck, undead deck). 
Monster cards will be assigned different ranks (typically stronger units at higher ranks). To be allowed to recruit monster cards from a certain rank of a deck you must first buy X cards of the previous rank.

# Battle:

Realtime, grid based. Mostly AI controlled. Player controls limited to either giving some limited orders to heroes or army (advance, hold, retreat etc, instead of move to x, attack y etc).
Heroes and monsters might gain levels from battles. (not sure whether this is a good idea)
