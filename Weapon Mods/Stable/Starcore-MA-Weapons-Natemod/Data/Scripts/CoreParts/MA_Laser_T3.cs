﻿using static Scripts.Structure;
using static Scripts.Structure.WeaponDefinition;
using static Scripts.Structure.WeaponDefinition.ModelAssignmentsDef;
using static Scripts.Structure.WeaponDefinition.HardPointDef;
using static Scripts.Structure.WeaponDefinition.HardPointDef.Prediction;
using static Scripts.Structure.WeaponDefinition.TargetingDef.BlockTypes;
using static Scripts.Structure.WeaponDefinition.TargetingDef.Threat;
using static Scripts.Structure.WeaponDefinition.TargetingDef;
using static Scripts.Structure.WeaponDefinition.TargetingDef.CommunicationDef.Comms;
using static Scripts.Structure.WeaponDefinition.TargetingDef.CommunicationDef.SecurityMode;
using static Scripts.Structure.WeaponDefinition.HardPointDef.HardwareDef;
using static Scripts.Structure.WeaponDefinition.HardPointDef.HardwareDef.HardwareType;

namespace Scripts {   
    partial class Parts {
        // Don't edit above this line

        WeaponDefinition MA_Fixed_T3 => new WeaponDefinition
        {
            Assignments = new ModelAssignmentsDef
            {
                MountPoints = new[] {
                    new MountPointDef {
                        SubtypeId = "MA_Fixed_T3",
                        SpinPartId = "", // For weapons with a spinning barrel such as Gatling Guns.
                        MuzzlePartId = "T3_EL", // The subpart where your muzzle empties are located.
                        AzimuthPartId = "T3_AZ",
                        ElevationPartId = "T3_EL",
                        DurabilityMod = 0.2f, // GeneralDamageMultiplier, 0.25f = 400% resistance.
                        IconName = "filter_energy.dds"
                    },
                     new MountPointDef {
                        SubtypeId = "MA_Fixed_T3_sb",
                        SpinPartId = "", // For weapons with a spinning barrel such as Gatling Guns.
                        MuzzlePartId = "T3_EL", // The subpart where your muzzle empties are located.
                        AzimuthPartId = "T3_AZ",
                        ElevationPartId = "T3_EL",
                        DurabilityMod = 0.2f, // GeneralDamageMultiplier, 0.25f = 400% resistance.
                        IconName = "filter_energy.dds"
                    },

                },
                Muzzles = new[] {
                   "T3_muzzle",
                },
                Ejector = "", // Optional; empty from which to eject "shells" if specified.
                Scope = "T3scope", // Where line of sight checks are performed from. Must be clear of block collision.
            },
            Targeting = new TargetingDef
            {
                Threats = new[] {
                    Projectiles, Grids, // Types of threat to engage: Grids, Projectiles, Characters, Meteors, Neutrals
                },
                SubSystems = new[] {
                    Offense, Power, Utility, Production, Thrust, Jumping, Steering, Any // Subsystem targeting priority: Offense, Utility, Power, Production, Thrust, Jumping, Steering, Any
                },
                ClosestFirst = false, // Tries to pick closest targets first (blocks on grids, projectiles, etc...).
                IgnoreDumbProjectiles = false, // Don't fire at non-smart projectiles.
                LockedSmartOnly = false, // Only fire at smart projectiles that are locked on to parent grid.
                MinimumDiameter = 0, // Minimum radius of threat to engage.
                MaximumDiameter = 0, // Maximum radius of threat to engage; 0 = unlimited.
                MaxTargetDistance = 3000, // Maximum distance at which targets will be automatically shot at; 0 = unlimited.
                MinTargetDistance = 0, // Minimum distance at which targets will be automatically shot at.
                TopTargets = 14, // Maximum number of targets to randomize between; 0 = unlimited.
                CycleTargets = 2, // Number of targets to "cycle" per acquire attempt.
                TopBlocks = 18, // Maximum number of blocks to randomize between; 0 = unlimited.
                CycleBlocks = 8, // Number of blocks to "cycle" per acquire attempt.
                StopTrackingSpeed = 0, // do not track target threats traveling faster than this speed
                MaxTrackingTime = 35, // After this time has been reached the weapon will stop tracking existing target and scan for a new one, only applies to turreted weapons
            },
            HardPoint = new HardPointDef
            {
                PartName = "Fixed Laser T3 Diode", // Name of the weapon in terminal, should be unique for each weapon definition that shares a SubtypeId (i.e. multiweapons).
                DeviateShotAngle = 0f, // Projectile inaccuracy in degrees.
                AimingTolerance = 9f, // How many degrees off target a turret can fire at. 0 - 180 firing angle.
                AimLeadingPrediction = Off, // Level of turret aim prediction; Off, Basic, Accurate, Advanced
                DelayCeaseFire = 10, // Measured in game ticks (6 = 100ms, 60 = 1 second, etc..). Length of time the weapon continues firing after trigger is released.
                AddToleranceToTracking = false, // Allows turret to only track to the edge of the AimingTolerance cone instead of dead centre.
                CanShootSubmerged = true, // Whether the weapon can be fired underwater when using WaterMod.

                Ui = new UiDef
                {
                    RateOfFire = false, // Enables terminal slider for changing rate of fire.
                    DamageModifier = false, // Enables terminal slider for changing damage per shot.
                    ToggleGuidance = false, // Enables terminal option to disable smart projectile guidance.
                    EnableOverload = false, // Enables terminal option to turn on Overload; this allows energy weapons to double damage per shot, at the cost of quadrupled power draw and heat gain, and 2% self damage on overheat.
                },
                Ai = new AiDef
                {
                    TrackTargets = true, // Whether this weapon tracks its own targets, or (for multiweapons) relies on the weapon with PrimaryTracking enabled for target designation.
                    TurretAttached = true, // Whether this weapon is a turret and should have the UI and API options for such.
                    TurretController = true, // Whether this weapon can physically control the turret's movement.
                    PrimaryTracking = true, // For multiweapons: whether this weapon should designate targets for other weapons on the platform without their own tracking.
                    LockOnFocus = false, // Whether this weapon should automatically fire at a target that has been locked onto via HUD.
                    SuppressFire = false, // If enabled, weapon can only be fired manually.
                    OverrideLeads = false, // Disable target leading on fixed weapons, or allow it for turrets.
                },
                HardWare = new HardwareDef
                {
                    RotateRate = 0.2f, // Max traversal speed of azimuth subpart in radians per tick (0.1 is approximately 360 degrees per second).
                    ElevateRate = 0.2f, // Max traversal speed of elevation subpart in radians per tick.
                    MinAzimuth = -5,
                    MaxAzimuth = 5,
                    MinElevation = -5,
                    MaxElevation = 5,
                    HomeAzimuth = 0, // Default resting rotation angle
                    HomeElevation = 0, // Default resting elevation
                    InventorySize = 0.658f, // Inventory capacity in kL.
                    IdlePower = 0.5f, // Power draw in MW while not charging, or for non-energy weapons. Defaults to 0.001.
                    FixedOffset = false, // Deprecated.
                    Offset = Vector(x: 0, y: 0, z: 0), // Offsets the aiming/firing line of the weapon, in metres.
                    Type = BlockWeapon, // What type of weapon this is; BlockWeapon, HandWeapon, Phantom 
                    CriticalReaction = new CriticalDef
                    {
                        Enable = false, // Enables Warhead behaviour.
                        DefaultArmedTimer = 120, // Sets default countdown duration.
                        PreArmed = false, // Whether the warhead is armed by default when placed. Best left as false.
                        TerminalControls = true, // Whether the warhead should have terminal controls for arming and detonation.
                        AmmoRound = "40m", // Optional. If specified, the warhead will always use this ammo on detonation rather than the currently selected ammo.
                    },
                },
                Other = new OtherDef
                {
                    ConstructPartCap = 0, // Maximum number of blocks with this weapon on a grid; 0 = unlimited.
                    RotateBarrelAxis = 0, // For spinning barrels, which axis to spin the barrel around; 0 = none.
                    EnergyPriority = 0, // Deprecated.
                    MuzzleCheck = false, // Whether the weapon should check LOS from each individual muzzle in addition to the scope.
                    Debug = false, // Force enables debug mode.
                    RestrictionRadius = 0, // Prevents other blocks of this type from being placed within this distance of the centre of the block.
                    CheckInflatedBox = false, // If true, the above distance check is performed from the edge of the block instead of the centre.
                    CheckForAnyWeapon = false, // If true, the check will fail if ANY weapon is present, not just weapons of the same subtype.
                },
                Loading = new LoadingDef
                {
                    RateOfFire = 3600, // Set this to 3600 for beam weapons.
                    BarrelsPerShot = 1, // How many muzzles will fire a projectile per fire event.
                    TrajectilesPerBarrel = 1, // Number of projectiles per muzzle per fire event.
                    SkipBarrels = 0, // Number of muzzles to skip after each fire event.
                    ReloadTime = 240, // Measured in game ticks (6 = 100ms, 60 = 1 seconds, etc..).
                    MagsToLoad = 1, // Number of physical magazines to consume on reload.
                    DelayUntilFire = 55, // How long the weapon waits before shooting after being told to fire. Measured in game ticks (6 = 100ms, 60 = 1 seconds, etc..).
                    HeatPerShot = 1, // Heat generated per shot.
                    MaxHeat = 300, // Max heat before weapon enters cooldown (70% of max heat).
                    Cooldown = .1f, // Percentage of max heat to be under to start firing again after overheat; accepts 0 - 0.95
                    HeatSinkRate = 30, // Amount of heat lost per second.
                    DegradeRof = false, // Progressively lower rate of fire when over 80% heat threshold (80% of max heat).
                    ShotsInBurst = 0, //480 Use this if you don't want the weapon to fire an entire physical magazine before stopping to reload. Should not be more than your magazine capacity.
                    DelayAfterBurst = 0, // How long to spend "reloading" after each burst. Measured in game ticks (6 = 100ms, 60 = 1 seconds, etc..).
                    FireFull = false, // Whether the weapon should fire the full burst, even if the target is lost or player stops firing prematurely.
                    GiveUpAfter = false, // Whether the weapon should drop its current target and reacquire a new target after finishing its burst.
                    BarrelSpinRate = 0, // Visual only, 0 disables and uses RateOfFire.
                    DeterministicSpin = false, // Spin barrel position will always be relative to initial / starting positions (spin will not be as smooth).
                    SpinFree = true, // Spin barrel while not firing.
                    StayCharged = false, // Will start recharging whenever power cap is not full.
                },
                Audio = new HardPointAudioDef
                {
                    PreFiringSound = "GladiusPreFiringSound", // Audio for warmup effect.
                    FiringSound = "GladiusFiringSound", // Audio for firing.
                    FiringSoundPerShot = false, // Whether to replay the sound for each shot, or just loop over the entire track while firing.
                    ReloadSound = "",
                    NoAmmoSound = "",
                    HardPointRotationSound = "", // Audio played when turret is moving.
                    BarrelRotationSound = "",
                    FireSoundEndDelay = 10, // How long the firing audio should keep playing after firing stops. Measured in game ticks(6 = 100ms, 60 = 1 seconds, etc..).
                },
                Graphics = new HardPointParticleDef
                {
                    Effect1 = new ParticleDef
                    {
                        Name = "", // SubtypeId of muzzle particle effect.
                        Color = Color(red: 0, green: 0, blue: 0, alpha: 1), // Deprecated, set color in particle sbc.
                        Offset = Vector(x: 0, y: -1, z: 0), // Offsets the effect from the muzzle empty.

                        Extras = new ParticleOptionDef
                        {
                            Loop = false, // Deprecated, set this in particle sbc.
                            Restart = false, // Whether to end the previous effect early and spawn a new one.
                            MaxDistance = 50, // Max distance at which this effect should be visible. NOTE: This will use whichever MaxDistance value is higher across Effect1 and Effect2!
                            MaxDuration = 5, // How many ticks the effect should be ended after, if it's still running.
                            Scale = 1f, // Scale of effect.
                        },
                    },
                    Effect2 = new ParticleDef
                    {
                        Name = "",
                        Color = Color(red: 0, green: 0, blue: 0, alpha: 1),
                        Offset = Vector(x: 0, y: 0, z: 0),

                        Extras = new ParticleOptionDef
                        {
                            Restart = false,
                            MaxDistance = 50,
                            MaxDuration = 0,
                            Scale = 1f,
                        },
                    },
                },
            },
            Ammos = new[] {
                MA_Laser_T3, // Must list all primary, shrapnel, and pattern ammos.
                MA_Laser_T3_Ammo_Decal, // Must list all primary, shrapnel, and pattern ammos.
            },
            Animations = Fixed_T3_Animations,
            //Upgrades = UpgradeModules,
        };

        WeaponDefinition MA_Gladius => new WeaponDefinition
        {
            Assignments = new ModelAssignmentsDef
            {
                MountPoints = new[] {
                    new MountPointDef {
                        SubtypeId = "MA_Gladius",
                        SpinPartId = "", // For weapons with a spinning barrel such as Gatling Guns.
                        MuzzlePartId = "MA_Gladius_Barrel", // The subpart where your muzzle empties are located.
                        AzimuthPartId = "MA_Gladius_Base",
                        ElevationPartId = "MA_Gladius_Barrel",
                        DurabilityMod = 0.25f, // GeneralDamageMultiplier, 0.25f = 400% resistance.
                        IconName = "filter_energy.dds"
                    },


                },
                Muzzles = new[] {
                   "muzzle_missile_001", "muzzle_missile_002"
                },
                Ejector = "", // Optional; empty from which to eject "shells" if specified.
                Scope = "Scope", // Where line of sight checks are performed from. Must be clear of block collision.
            },
            Targeting = new TargetingDef
            {
                Threats = new[] {
                    Grids, // Types of threat to engage: Grids, Projectiles, Characters, Meteors, Neutrals
                },
                SubSystems = new[] {
                    Offense, Thrust, Utility, Power, Production, Any, // Subsystem targeting priority: Offense, Utility, Power, Production, Thrust, Jumping, Steering, Any
                },
                ClosestFirst = false, // Tries to pick closest targets first (blocks on grids, projectiles, etc...).
                IgnoreDumbProjectiles = false, // Don't fire at non-smart projectiles.
                LockedSmartOnly = false, // Only fire at smart projectiles that are locked on to parent grid.
                MinimumDiameter = 0, // Minimum radius of threat to engage.
                MaximumDiameter = 0, // Maximum radius of threat to engage; 0 = unlimited.
                MaxTargetDistance = 4000, // Maximum distance at which targets will be automatically shot at; 0 = unlimited.
                MinTargetDistance = 0, // Minimum distance at which targets will be automatically shot at.
                TopTargets = 20, // Maximum number of targets to randomize between; 0 = unlimited.
                TopBlocks = 5, // Maximum number of blocks to randomize between; 0 = unlimited.
                StopTrackingSpeed = 0, // Do not track threats traveling faster than this speed; 0 = unlimited.
            },
            HardPoint = new HardPointDef
            {
                PartName = "Gladius Laser", // Name of the weapon in terminal, should be unique for each weapon definition that shares a SubtypeId (i.e. multiweapons).
                DeviateShotAngle = 0f, // Projectile inaccuracy in degrees.
                AimingTolerance = .1f, // How many degrees off target a turret can fire at. 0 - 180 firing angle.
                AimLeadingPrediction = Off, // Level of turret aim prediction; Off, Basic, Accurate, Advanced
                DelayCeaseFire = 0, // Measured in game ticks (6 = 100ms, 60 = 1 second, etc..). Length of time the weapon continues firing after trigger is released.
                AddToleranceToTracking = false, // Allows turret to only track to the edge of the AimingTolerance cone instead of dead centre.
                CanShootSubmerged = true, // Whether the weapon can be fired underwater when using WaterMod.

                Ui = new UiDef
                {
                    RateOfFire = false, // Enables terminal slider for changing rate of fire.
                    DamageModifier = true, // Enables terminal slider for changing damage per shot.
                    ToggleGuidance = false, // Enables terminal option to disable smart projectile guidance.
                    EnableOverload = false, // Enables terminal option to turn on Overload; this allows energy weapons to double damage per shot, at the cost of quadrupled power draw and heat gain, and 2% self damage on overheat.
                },
                Ai = new AiDef
                {
                    TrackTargets = true, // Whether this weapon tracks its own targets, or (for multiweapons) relies on the weapon with PrimaryTracking enabled for target designation.
                    TurretAttached = true, // Whether this weapon is a turret and should have the UI and API options for such.
                    TurretController = true, // Whether this weapon can physically control the turret's movement.
                    PrimaryTracking = true, // For multiweapons: whether this weapon should designate targets for other weapons on the platform without their own tracking.
                    LockOnFocus = false, // Whether this weapon should automatically fire at a target that has been locked onto via HUD.
                    SuppressFire = false, // If enabled, weapon can only be fired manually.
                    OverrideLeads = false, // Disable target leading on fixed weapons, or allow it for turrets.
                },
                HardWare = new HardwareDef
                {
                    RotateRate = 0.008f, // Max traversal speed of azimuth subpart in radians per tick (0.1 is approximately 360 degrees per second).
                    ElevateRate = 0.008f, // Max traversal speed of elevation subpart in radians per tick.
                    MinAzimuth = -180,
                    MaxAzimuth = 180,
                    MinElevation = -15,
                    MaxElevation = 90,
                    HomeAzimuth = 0, // Default resting rotation angle
                    HomeElevation = 0, // Default resting elevation
                    InventorySize = 0.658f, // Inventory capacity in kL.
                    IdlePower = 25f, // Power draw in MW while not charging, or for non-energy weapons. Defaults to 0.001.
                    FixedOffset = false, // Deprecated.
                    Offset = Vector(x: 0, y: 0, z: 0), // Offsets the aiming/firing line of the weapon, in metres.
                    Type = BlockWeapon, // What type of weapon this is; BlockWeapon, HandWeapon, Phantom 
                    CriticalReaction = new CriticalDef
                    {
                        Enable = false, // Enables Warhead behaviour.
                        DefaultArmedTimer = 120, // Sets default countdown duration.
                        PreArmed = false, // Whether the warhead is armed by default when placed. Best left as false.
                        TerminalControls = true, // Whether the warhead should have terminal controls for arming and detonation.
                        AmmoRound = "40m", // Optional. If specified, the warhead will always use this ammo on detonation rather than the currently selected ammo.
                    },
                },
                Other = new OtherDef
                {
                    ConstructPartCap = 0, // Maximum number of blocks with this weapon on a grid; 0 = unlimited.
                    RotateBarrelAxis = 0, // For spinning barrels, which axis to spin the barrel around; 0 = none.
                    EnergyPriority = 0, // Deprecated.
                    MuzzleCheck = false, // Whether the weapon should check LOS from each individual muzzle in addition to the scope.
                    Debug = false, // Force enables debug mode.
                    RestrictionRadius = 0, // Prevents other blocks of this type from being placed within this distance of the centre of the block.
                    CheckInflatedBox = false, // If true, the above distance check is performed from the edge of the block instead of the centre.
                    CheckForAnyWeapon = false, // If true, the check will fail if ANY weapon is present, not just weapons of the same subtype.
                },
                Loading = new LoadingDef
                {
                    RateOfFire = 3600, // Set this to 3600 for beam weapons.
                    BarrelsPerShot = 2, // How many muzzles will fire a projectile per fire event.
                    TrajectilesPerBarrel = 1, // Number of projectiles per muzzle per fire event.
                    SkipBarrels = 0, // Number of muzzles to skip after each fire event.
                    ReloadTime = 240, // Measured in game ticks (6 = 100ms, 60 = 1 seconds, etc..).
                    MagsToLoad = 1, // Number of physical magazines to consume on reload.
                    DelayUntilFire = 55, // How long the weapon waits before shooting after being told to fire. Measured in game ticks (6 = 100ms, 60 = 1 seconds, etc..).
                    HeatPerShot = 1, // Heat generated per shot.
                    MaxHeat = 1200, // Max heat before weapon enters cooldown (70% of max heat).
                    Cooldown = .1f, // Percentage of max heat to be under to start firing again after overheat; accepts 0 - 0.95
                    HeatSinkRate = 60, // Amount of heat lost per second.
                    DegradeRof = false, // Progressively lower rate of fire when over 80% heat threshold (80% of max heat).
                    ShotsInBurst = 480, // Use this if you don't want the weapon to fire an entire physical magazine before stopping to reload. Should not be more than your magazine capacity.
                    DelayAfterBurst = 0, // How long to spend "reloading" after each burst. Measured in game ticks (6 = 100ms, 60 = 1 seconds, etc..).
                    FireFull = true, // Whether the weapon should fire the full burst, even if the target is lost or player stops firing prematurely.
                    GiveUpAfter = false, // Whether the weapon should drop its current target and reacquire a new target after finishing its burst.
                    BarrelSpinRate = 0, // Visual only, 0 disables and uses RateOfFire.
                    DeterministicSpin = false, // Spin barrel position will always be relative to initial / starting positions (spin will not be as smooth).
                    SpinFree = true, // Spin barrel while not firing.
                    StayCharged = false, // Will start recharging whenever power cap is not full.
                },
                Audio = new HardPointAudioDef
                {
                    PreFiringSound = "GladiusPreFiringSound", // Audio for warmup effect.
                    FiringSound = "GladiusFiringSound", // Audio for firing.
                    FiringSoundPerShot = false, // Whether to replay the sound for each shot, or just loop over the entire track while firing.
                    ReloadSound = "",
                    NoAmmoSound = "",
                    HardPointRotationSound = "WepTurretGatlingRotate", // Audio played when turret is moving.
                    BarrelRotationSound = "WepShipGatlingRotation",
                    FireSoundEndDelay = 0, // How long the firing audio should keep playing after firing stops. Measured in game ticks(6 = 100ms, 60 = 1 seconds, etc..).
                },
                Graphics = new HardPointParticleDef
                {
                    Effect1 = new ParticleDef
                    {
                        Name = "", // SubtypeId of muzzle particle effect.
                        Color = Color(red: 0, green: 0, blue: 0, alpha: 1), // Deprecated, set color in particle sbc.
                        Offset = Vector(x: 0, y: -1, z: 0), // Offsets the effect from the muzzle empty.

                        Extras = new ParticleOptionDef
                        {
                            Loop = false, // Deprecated, set this in particle sbc.
                            Restart = false, // Whether to end the previous effect early and spawn a new one.
                            MaxDistance = 50, // Max distance at which this effect should be visible. NOTE: This will use whichever MaxDistance value is higher across Effect1 and Effect2!
                            MaxDuration = 5, // How many ticks the effect should be ended after, if it's still running.
                            Scale = 1f, // Scale of effect.
                        },
                    },
                    Effect2 = new ParticleDef
                    {
                        Name = "",
                        Color = Color(red: 0, green: 0, blue: 0, alpha: 1),
                        Offset = Vector(x: 0, y: 0, z: 0),

                        Extras = new ParticleOptionDef
                        {
                            Restart = false,
                            MaxDistance = 50,
                            MaxDuration = 0,
                            Scale = 1f,
                        },
                    },
                },
            },
            Ammos = new[] {
                MA_Laser_Gladius_Ammo, // Must list all primary, shrapnel, and pattern ammos.
                MA_Laser_T3_Ammo_Decal, // Must list all primary, shrapnel, and pattern ammos.
            },
            Animations = MA_Gladius_Laser_Animations,
            //Upgrades = UpgradeModules,
        };


        WeaponDefinition MA_Gladius_Ion => new WeaponDefinition
        {
            Assignments = new ModelAssignmentsDef
            {
                MountPoints = new[] {
                    new MountPointDef {
                        SubtypeId = "MA_Gladius",
                        SpinPartId = "", // For weapons with a spinning barrel such as Gatling Guns.
                        MuzzlePartId = "blankmuzzle", // The subpart where your muzzle empties are located.
                        AzimuthPartId = "emp_base",
                        ElevationPartId = "blankmuzzle",
                        DurabilityMod = 0.25f, // GeneralDamageMultiplier, 0.25f = 400% resistance.
                        IconName = "filter_energy.dds"
                    },
                

                },
                Muzzles = new[] {
                   "muzzle_emp",
                },
                Ejector = "", // Optional; empty from which to eject "shells" if specified.
                Scope = "Scope_emp", // Where line of sight checks are performed from. Must be clear of block collision.
            },
            Targeting = new TargetingDef
            {
                Threats = new[] {
                    Grids, Projectiles,  // Types of threat to engage: Grids, Projectiles, Characters, Meteors, Neutrals
                },
                SubSystems = new[] {
                    Offense, Thrust, Utility, Power, Production, Any, // Subsystem targeting priority: Offense, Utility, Power, Production, Thrust, Jumping, Steering, Any
                },
                ClosestFirst = true, // Tries to pick closest targets first (blocks on grids, projectiles, etc...).
                IgnoreDumbProjectiles = false, // Don't fire at non-smart projectiles.
                LockedSmartOnly = false, // Only fire at smart projectiles that are locked on to parent grid.
                MinimumDiameter = 0, // Minimum radius of threat to engage.
                MaximumDiameter = 0, // Maximum radius of threat to engage; 0 = unlimited.
                MaxTargetDistance = 750, // Maximum distance at which targets will be automatically shot at; 0 = unlimited.
                MinTargetDistance = 0, // Minimum distance at which targets will be automatically shot at.
                TopTargets = 0, // Maximum number of targets to randomize between; 0 = unlimited.
                TopBlocks = 0, // Maximum number of blocks to randomize between; 0 = unlimited.
                StopTrackingSpeed = 0, // Do not track threats traveling faster than this speed; 0 = unlimited.
            },
            HardPoint = new HardPointDef
            {
                PartName = "Gladius Ion", // Name of the weapon in terminal, should be unique for each weapon definition that shares a SubtypeId (i.e. multiweapons).
                DeviateShotAngle = 0f, // Projectile inaccuracy in degrees.
                AimingTolerance = 10f, // How many degrees off target a turret can fire at. 0 - 180 firing angle.
                AimLeadingPrediction = Off, // Level of turret aim prediction; Off, Basic, Accurate, Advanced
                DelayCeaseFire = 0, // Measured in game ticks (6 = 100ms, 60 = 1 second, etc..). Length of time the weapon continues firing after trigger is released.
                AddToleranceToTracking = false, // Allows turret to only track to the edge of the AimingTolerance cone instead of dead centre.
                CanShootSubmerged = false, // Whether the weapon can be fired underwater when using WaterMod.

                Ui = new UiDef
                {
                    RateOfFire = false, // Enables terminal slider for changing rate of fire.
                    DamageModifier = false, // Enables terminal slider for changing damage per shot.
                    ToggleGuidance = false, // Enables terminal option to disable smart projectile guidance.
                    EnableOverload = false, // Enables terminal option to turn on Overload; this allows energy weapons to double damage per shot, at the cost of quadrupled power draw and heat gain, and 2% self damage on overheat.
                },
                Ai = new AiDef
                {
                    TrackTargets = true, // Whether this weapon tracks its own targets, or (for multiweapons) relies on the weapon with PrimaryTracking enabled for target designation.
                    TurretAttached = true, // Whether this weapon is a turret and should have the UI and API options for such.
                    TurretController = true, // Whether this weapon can physically control the turret's movement.
                    PrimaryTracking = false, // For multiweapons: whether this weapon should designate targets for other weapons on the platform without their own tracking.
                    LockOnFocus = false, // Whether this weapon should automatically fire at a target that has been locked onto via HUD.
                    SuppressFire = false, // If enabled, weapon can only be fired manually.
                    OverrideLeads = false, // Disable target leading on fixed weapons, or allow it for turrets.
                },
                HardWare = new HardwareDef
                {
                    RotateRate = 0.015f, // Max traversal speed of azimuth subpart in radians per tick (0.1 is approximately 360 degrees per second).
                    ElevateRate = 0.015f, // Max traversal speed of elevation subpart in radians per tick.
                    MinAzimuth = -180,
                    MaxAzimuth = 180,
                    MinElevation = 0,
                    MaxElevation = 1,
                    HomeAzimuth = 0, // Default resting rotation angle
                    HomeElevation = 0, // Default resting elevation
                    InventorySize = 0.658f, // Inventory capacity in kL.
                    IdlePower = 0.0005f, // Power draw in MW while not charging, or for non-energy weapons. Defaults to 0.001.
                    FixedOffset = false, // Deprecated.
                    Offset = Vector(x: 0, y: 0, z: 0), // Offsets the aiming/firing line of the weapon, in metres.
                    Type = BlockWeapon, // What type of weapon this is; BlockWeapon, HandWeapon, Phantom 
                    CriticalReaction = new CriticalDef
                    {
                        Enable = false, // Enables Warhead behaviour.
                        DefaultArmedTimer = 120, // Sets default countdown duration.
                        PreArmed = false, // Whether the warhead is armed by default when placed. Best left as false.
                        TerminalControls = true, // Whether the warhead should have terminal controls for arming and detonation.
                        AmmoRound = "40m", // Optional. If specified, the warhead will always use this ammo on detonation rather than the currently selected ammo.
                    },
                },
                Other = new OtherDef
                {
                    ConstructPartCap = 0, // Maximum number of blocks with this weapon on a grid; 0 = unlimited.
                    RotateBarrelAxis = 0, // For spinning barrels, which axis to spin the barrel around; 0 = none.
                    EnergyPriority = 0, // Deprecated.
                    MuzzleCheck = false, // Whether the weapon should check LOS from each individual muzzle in addition to the scope.
                    Debug = false, // Force enables debug mode.
                    RestrictionRadius = 0, // Prevents other blocks of this type from being placed within this distance of the centre of the block.
                    CheckInflatedBox = false, // If true, the above distance check is performed from the edge of the block instead of the centre.
                    CheckForAnyWeapon = false, // If true, the check will fail if ANY weapon is present, not just weapons of the same subtype.
                },
                Loading = new LoadingDef
                {
                    RateOfFire = 3600, // Set this to 3600 for beam weapons.
                    BarrelsPerShot = 1, // How many muzzles will fire a projectile per fire event.
                    TrajectilesPerBarrel = 1, // Number of projectiles per muzzle per fire event.
                    SkipBarrels = 0, // Number of muzzles to skip after each fire event.
                    ReloadTime = 420, // Measured in game ticks (6 = 100ms, 60 = 1 seconds, etc..).
                    MagsToLoad = 1, // Number of physical magazines to consume on reload.
                    DelayUntilFire = 55, // How long the weapon waits before shooting after being told to fire. Measured in game ticks (6 = 100ms, 60 = 1 seconds, etc..).
                    HeatPerShot = 0, // Heat generated per shot.
                    MaxHeat = 60, // Max heat before weapon enters cooldown (70% of max heat).
                    Cooldown = .25f, // Percentage of max heat to be under to start firing again after overheat; accepts 0 - 0.95
                    HeatSinkRate = 0, // Amount of heat lost per second.
                    DegradeRof = false, // Progressively lower rate of fire when over 80% heat threshold (80% of max heat).
                    ShotsInBurst = 60, // Use this if you don't want the weapon to fire an entire physical magazine before stopping to reload. Should not be more than your magazine capacity.
                    DelayAfterBurst = 0, // How long to spend "reloading" after each burst. Measured in game ticks (6 = 100ms, 60 = 1 seconds, etc..).
                    FireFull = true, // Whether the weapon should fire the full burst, even if the target is lost or player stops firing prematurely.
                    GiveUpAfter = false, // Whether the weapon should drop its current target and reacquire a new target after finishing its burst.
                    BarrelSpinRate = 0, // Visual only, 0 disables and uses RateOfFire.
                    DeterministicSpin = false, // Spin barrel position will always be relative to initial / starting positions (spin will not be as smooth).
                    SpinFree = true, // Spin barrel while not firing.
                    StayCharged = false, // Will start recharging whenever power cap is not full.
                },
                Audio = new HardPointAudioDef
                {
                    PreFiringSound = "", // Audio for warmup effect.
                    FiringSound = "EMPFiringSound", // Audio for firing.
                    FiringSoundPerShot = false, // Whether to replay the sound for each shot, or just loop over the entire track while firing.
                    ReloadSound = "",
                    NoAmmoSound = "",
                    HardPointRotationSound = "", // Audio played when turret is moving.
                    BarrelRotationSound = "",
                    FireSoundEndDelay = 0, // How long the firing audio should keep playing after firing stops. Measured in game ticks(6 = 100ms, 60 = 1 seconds, etc..).
                },
                Graphics = new HardPointParticleDef
                {
                    Effect1 = new ParticleDef
                    {
                        Name = "MA_laserhit_reversed", // SubtypeId of muzzle particle effect.
                        Color = Color(red: 10, green: 15, blue: 20, alpha: 1), // Deprecated, set color in particle sbc.
                        Offset = Vector(x: 0, y: 0, z: -0.5f), // Offsets the effect from the muzzle empty.

                        Extras = new ParticleOptionDef
                        {
                            Loop = true, // Deprecated, set this in particle sbc.
                            Restart = true, // Whether to end the previous effect early and spawn a new one.
                            MaxDistance = 400, // Max distance at which this effect should be visible. NOTE: This will use whichever MaxDistance value is higher across Effect1 and Effect2!
                            MaxDuration = 30, // How many ticks the effect should be ended after, if it's still running.
                            Scale = 1f, // Scale of effect.
                        },
                    },
                    Effect2 = new ParticleDef
                    {
                        Name = "",
                        Color = Color(red: 0, green: 0, blue: 0, alpha: 1),
                        Offset = Vector(x: 0, y: 0, z: 0),

                        Extras = new ParticleOptionDef
                        {
                            Restart = false,
                            MaxDistance = 50,
                            MaxDuration = 0,
                            Scale = 1f,
                        },
                    },
                },
            },
            Ammos = new[] {
                MA_Ion_1, // Must list all primary, shrapnel, and pattern ammos.
            },
            //Animations = PDX_Animations,
            //Upgrades = UpgradeModules,
        };


        // Don't edit below this line.
    }
}
