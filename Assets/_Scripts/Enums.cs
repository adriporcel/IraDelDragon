public enum Players
{
    main,
    secondary
}
public enum BoardPosition
{
    handMain,
    playAreaMain,
    bortherhoodAreaMain,
    handSecond,
    playAreaSecond,
    bortherhoodAreaSecond,
}
public enum GameState
{
    start,
    mainPlayerTurn,
    secondPlayerTurn,
    endGame
}
public enum Deck
{
    red,
    green,
    blue,
    grey,
    magical
}
public enum CardType
{
    none,
    brotherhood,
    creature,
    enchantCreature,
    enchantment,
    instant,
    magicalObject,
    player,
    enchantBrotherhood,
    flyCreature
}
public enum ObjectType
{
    none,
    attack,
    defense,
    distanceAttack,
    charm
}