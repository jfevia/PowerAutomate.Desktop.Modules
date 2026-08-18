// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// A projectile effect travelling between tiles, matching TFS's ShootEffect_t wire values.
/// </summary>
public enum ShootEffect : byte
{
    Spear = 0,
    Bolt = 1,
    Arrow = 2,
    Fire = 3,
    Energy = 4,
    Poisonarrow = 5,
    Burstarrow = 6,
    Throwingstar = 7,
    Throwingknife = 8,
    Smallstone = 9,
    Death = 10,
    Largerock = 11,
    Snowball = 12,
    Powerbolt = 13,
    Poisonfield = 14,
    Infernalbolt = 15,
    Huntingspear = 16,
    Enchantedspear = 17,
    Redstar = 18,
    Greenstar = 19,
    Royalspear = 20,
    Sniperarrow = 21,
    Onyxarrow = 22,
    Piercingbolt = 23,
    Whirlwindsword = 24,
    Whirlwindaxe = 25,
    Whirlwindclub = 26,
    Etherealspear = 27,
    Ice = 28,
    Earth = 29,
    Holy = 30,
    Suddendeath = 31,
    Flasharrow = 32,
    Flammingarrow = 33,
    Shiverarrow = 34,
    Energyball = 35,
    Smallice = 36,
    Smallholy = 37,
    Smallearth = 38,
    Eartharrow = 39,
    Explosion = 40,
    Cake = 41,

    /// <summary>
    /// TFS SHOOT_EFFECT_WEAPONTYPE; resolved to the attacker's weapon type at render time.
    /// </summary>
    Weapontype = 254,

    /// <summary>
    /// TFS SHOOT_EFFECT_NONE; wire value 0 decodes to this sentinel.
    /// </summary>
    None = 255
}
