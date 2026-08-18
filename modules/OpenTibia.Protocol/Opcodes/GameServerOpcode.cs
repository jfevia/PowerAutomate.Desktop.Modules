// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

/// <summary>
/// Server-to-client opcodes for protocol 8.60 game connections, matching tfs-old-svn r3884.
/// </summary>
public enum GameServerOpcode : byte
{
    None = 0x00,
    LoginOrPendingState = 0x0A,
    LoginError = 0x14,
    LoginAdvice = 0x15,
    LoginWait = 0x16,
    Ping = 0x1E,
    Challenge = 0x1F,

    /// <summary>
    /// Relogin window in 8.60; modern clients reuse 0x28 for a death dialog.
    /// </summary>
    ReLoginWindow = 0x28,
    FullMap = 0x64,
    MapTopRow = 0x65,
    MapRightRow = 0x66,
    MapBottomRow = 0x67,
    MapLeftRow = 0x68,
    UpdateTile = 0x69,
    CreateOnMap = 0x6A,
    ChangeOnMap = 0x6B,
    DeleteOnMap = 0x6C,
    MoveCreature = 0x6D,
    OpenContainer = 0x6E,
    CloseContainer = 0x6F,
    CreateContainer = 0x70,
    ChangeInContainer = 0x71,
    DeleteInContainer = 0x72,
    SetInventory = 0x78,
    DeleteInventory = 0x79,
    OpenNpcTrade = 0x7A,
    PlayerGoods = 0x7B,
    CloseNpcTrade = 0x7C,
    OwnTrade = 0x7D,
    CounterTrade = 0x7E,
    CloseTrade = 0x7F,
    Ambient = 0x82,
    GraphicalEffect = 0x83,
    TextEffect = 0x84,
    MissleEffect = 0x85,

    /// <summary>
    /// Creature target-square marker in 8.60; modern clients reuse 0x86 for item classes.
    /// </summary>
    CreatureSquare = 0x86,
    CreatureHealth = 0x8C,
    CreatureLight = 0x8D,
    CreatureOutfit = 0x8E,
    CreatureSpeed = 0x8F,
    CreatureSkull = 0x90,
    CreatureParty = 0x91,
    CreatureUnpass = 0x92,
    EditText = 0x96,
    EditList = 0x97,
    PlayerData = 0xA0,
    PlayerSkills = 0xA1,
    PlayerState = 0xA2,
    ClearTarget = 0xA3,
    Talk = 0xAA,
    Channels = 0xAB,
    OpenChannel = 0xAC,
    OpenPrivateChannel = 0xAD,
    RuleViolationChannel = 0xAE,
    RuleViolationRemove = 0xAF,
    RuleViolationCancel = 0xB0,
    RuleViolationLock = 0xB1,
    OpenOwnChannel = 0xB2,
    CloseChannel = 0xB3,
    TextMessage = 0xB4,
    CancelWalk = 0xB5,
    FloorChangeUp = 0xBE,
    FloorChangeDown = 0xBF,
    ChooseOutfit = 0xC8,
    VipAdd = 0xD2,
    VipState = 0xD3,
    VipLogout = 0xD4,
    TutorialHint = 0xDC,
    AutomapFlag = 0xDD,
    QuestLog = 0xF0,
    QuestLine = 0xF1
}
