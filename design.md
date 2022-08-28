# DunnoAlternative

## Pre (what should this be called?)

In this file some statements will be prefixed with "stretch goal: " 
these MIGHT be implemented once the main functionality (everything without the prefix) has been implemented.

At this point a lot of this document is still open for change, feel free to suggest alternative implementations

## In short:

A single player game in which the player assembles a cast of heroes and monsters and uses those to conquer a randomly generated world, 
filled with AI who try to do the same, with equal tools.

The game will be split between a world map, en a battle map. Conquering the world should take about a few hours for an experienced player (and not longer! Important!).
Battles should take 5 minutes at most, and a fastforward should be present to massively speed up battles. 

Most of the important decisions will be made in the world mode, in the form of deciding what heroes and monsters to recruit and where to deploy them.
Battles will be mostly automatic under AI control, with minimal user input.

### Stretch goal: 

online multiplayer

## Target platforms:

Windows, Linux

### Stretch goal: 

Android

## Technical:

It will either be made in C# and SFML or in Rust (probably with Bevy?)
Both battles and the world will be 2d and grid based. Troops in battles will use static (unanimated) 2d sprites. 
Instead of animations particle effects, and sprite movements will be used to make the battlefield look somewhat alive.

A decision will not be made whether the resolution will be static, and the whole world/battle will be visible at times. 
Or to add scrolling, zooming, resolution options.

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

# World:

Turn based, turns (and battles) will take place sequentially. 
At the start of a turn the player gets a limited amount of resources to spend during their turn: 
- Money from their provinces, this is the only resource that carries over to following turns.
- A limited amount of actions/attacks. (maybe 2), might also get used for construction.
- Every hero (and their attached monsters) may fight once per turn (and only defend during other players turns if they didn't attack!).

With these resources they have to each turn, if so desired:
- Invade other factions territories.
- Build province upgrades: either economic upgrades, or (better) castles 
- Hire new (wandering?) heroes
- Buy new monster cards
- Levelup/upgrade monsters/heroes? (if leveling gets added)

## Monster (recruitment)

Despite the name these may also just be human troops. They are organized in cards with a select amount of a certain monster.

Buying monster cards costs money depending on the card. Only a limited amount of the same card may be bought, sometimes only 1 sometimes a couple (maybe 3 or 5).

Cards are found in thematic decks (example: human deck, slime deck, undead deck). 
Monster cards will be assigned different ranks (typically stronger units at higher ranks). To be allowed to recruit monster cards from a certain rank of a deck you must first buy X cards of the previous rank.

# Battle:

Realtime, grid based. Mostly AI controlled. Player controls limited to either giving some limited orders to heroes or army (advance, hold, retreat etc, instead of move to x, attack y etc).
Heroes and monsters might gain levels from battles. (not sure whether this is a good idea)
