// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// A graphical effect shown on a tile, matching TFS's MagicEffect_t wire values.
/// </summary>
public enum MagicEffect : byte
{
    DrawBlood = 0,
    LoseEnergy = 1,
    Poff = 2,
    Blockhit = 3,
    ExplosionArea = 4,
    ExplosionDamage = 5,
    FireArea = 6,
    YellowRings = 7,
    PoisonRings = 8,
    HitArea = 9,
    Teleport = 10,
    EnergyDamage = 11,
    WrapsBlue = 12,
    WrapsRed = 13,
    WrapsGreen = 14,
    HitbyFire = 15,
    Poison = 16,
    MortArea = 17,
    SoundGreen = 18,
    SoundRed = 19,
    PoisonArea = 20,
    SoundYellow = 21,
    SoundPurple = 22,
    SoundBlue = 23,
    SoundWhite = 24,
    Bubbles = 25,
    Craps = 26,
    GiftWraps = 27,
    FireworkYellow = 28,
    FireworkRed = 29,
    FireworkBlue = 30,
    Stun = 31,
    Sleep = 32,
    Watercreature = 33,
    Groundshaker = 34,
    Hearts = 35,
    Fireattack = 36,
    EnergyArea = 37,
    Smallclouds = 38,
    Holydamage = 39,
    Bigclouds = 40,
    Icearea = 41,
    Icetornado = 42,
    Iceattack = 43,
    Stones = 44,
    Smallplants = 45,
    Carniphila = 46,
    Purpleenergy = 47,
    Yellowenergy = 48,
    Holyarea = 49,
    Bigplants = 50,
    Cake = 51,
    Giantice = 52,
    Watersplash = 53,
    Plantattack = 54,
    Tutorialarrow = 55,
    Tutorialsquare = 56,
    Mirrorhorizontal = 57,
    Mirrorvertical = 58,
    Skullhorizontal = 59,
    Skullvertical = 60,
    Assassin = 61,
    Stepshorizontal = 62,
    Bloodysteps = 63,
    Stepsvertical = 64,
    Yalaharighost = 65,
    Bats = 66,
    Smoke = 67,
    Insects = 68,
    Dragonhead = 69,

    /// <summary>
    /// TFS MAGIC_EFFECT_NONE; wire value 0 decodes to this sentinel.
    /// </summary>
    None = 255
}
