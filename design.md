# DunnoAlternative

## Pre (what should this be called?)

In this file some statements will be prefixed with "stretch goal: " 
these MIGHT be implemented once the main functionality (everything without the prefix) has been implemented.

At this point a lot of this document is still open for change, feel free to suggest alternative implementations

## In short:

A single player game in which the player assembles a cast of heroes and monsters and uses those to conquer a randomly generated world, 
filled with AI who try to do the same, with equal tools.

The game will be split between a world map and a battle map. Conquering the world should take about a few hours for an experienced player (and not longer! Important!).
Battles should take 5 minutes at most, and a fastforward should be present to speed up battles. 

Most of the important decisions will be made in the world mode, in the form of deciding what heroes and monsters to recruit and where to deploy them.
Battles will be mostly automatic under AI control, with minimal user input.

### Stretch goal: 

online multiplayer

## Target platforms:

Windows, Linux

### Stretch goal: 

Android, web

## Technical:

It will either be made in C# and SFML or in Rust (probably with Bevy?)
Both battles and the world will be 2d and grid based. Troops in battles will use static (unanimated) 2d sprites. 
Instead of animations particle effects, and sprite movements will be used to make the battlefield look somewhat alive.

There will be scrolling, zooming, resolution options.

It will be possible to save in the world mode. And the game will autosave (unless disabled) at the start and end of every player turn.

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
- A limited amount of actions/attacks. (maybe 2), might also get used for construction. Alternatively a player may only attack the same province once per turn.
- Every hero (and their attached monsters) may fight once per turn (and only defend during other players turns if they didn't attack!).

With these resources they have to each turn, if so desired:
- Invade other factions territories.
- Build province upgrades: either economic upgrades, or (better) castles 
- Hire new (wandering?) heroes
- Buy new monster cards
- Levelup/upgrade monsters/heroes? (if leveling gets added)

Each province will need to be succesfully attacked twice to be defeated. The second spot might contain a castle.

## World Generation

At the start of a new game. A small random world will be generated with various tiles. Some like water and mountains etc, will not be conquerable. All other tiles will be connected to eachother (no player alone on an unreachable island :D). After this a tile will be picked for the player depending on difficulty and the rest will be filled by AI players. (or maybe some will be special 'rebel' factions which won't partake in offensive actions themselves.)

## Monster (recruitment)

Despite the name these may also just be human troops. They are organized in cards with a select amount of a certain monster.

Buying monster cards costs money depending on the card. Only a limited amount of the same card may be bought, sometimes only 1 sometimes a couple (maybe 3 or 5).

Cards are found in thematic decks (example: human deck, slime deck, undead deck). 
Monster cards will be assigned different ranks (typically stronger units at higher ranks). To be allowed to recruit monster cards from a certain rank of a deck you must first buy X cards of the previous rank.

# Battle:

Realtime, grid based. Mostly AI controlled. Player controls limited to either giving some limited orders to heroes or army (advance, hold, retreat etc, instead of move to x, attack y etc).
Heroes and monsters might gain levels from battles. (not sure whether this is a good idea)
