using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Rage;
using Rage.Attributes;
using Rage.Native;
using DotNet.Globbing;
using System.Drawing;

[assembly: Plugin("RAGEplus", Description = "", Author = "Bluscream")]
namespace RAGEplus
{
    public class EntryPoint
    {
        #region Commands
        [ConsoleCommand]
        private static void Command_Log(string[] text)
        {
            Game.Console.Print(text.Join(" "));
        }
        [ConsoleCommand]
        private static void Command_Notification(string text)
        {
            Game.DisplayNotification(text);
        }
        [ConsoleCommand]
        private static void Command_HelpNotification(string text)
        {
            Game.DisplayHelp(text);
        }
        [ConsoleCommand]
        private static void Command_Subtitle(string text)
        {
            Game.DisplaySubtitle(text);
        }
        [ConsoleCommand]
        private static void Command_RevealMap(bool reveal = true)
        {
            Game.IsFullMapRevealForced = reveal;
        }
#if debug
        [ConsoleCommand]
            public static void Command_RemoveAllPedsAndVehicles()
            {
                foreach(Ped ped in World.GetAllPeds())
                {
                    if (!ped.IsPlayer)
                    {
                        ped.Delete();
                    }
                }
                Vehicle playersVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                foreach(Vehicle vehicle in World.GetAllVehicles())
                {
                    if (!playersVehicle.Exists() || vehicle != playersVehicle)
                    {
                        vehicle.Delete();
                    }
                }
            }
#endif
        [ConsoleCommand]
        private static void Command_ExplodeAll(string pattern = "*") {
            var globOptions = new GlobOptions(); globOptions.Evaluation.CaseInsensitive = true;
            var playersVehicle = Game.LocalPlayer.Character.CurrentVehicle;
                foreach(var vehicle in World.GetAllVehicles())
                {
                    if (vehicle.Exists() && (!playersVehicle.Exists() || vehicle != playersVehicle) && !vehicle.IsExplosionProof)
                    {
                        if (Glob.Parse(pattern, globOptions).IsMatch(vehicle.Model.Name)) vehicle.Explode();
                    }
                }
        }
        [ConsoleCommand]
        private static void Command_KillAll(bool makePersistent = false, string pattern = "*") {
            var globOptions = new GlobOptions(); globOptions.Evaluation.CaseInsensitive = true;
                foreach(var ped in World.GetAllPeds())
                {
                if (!ped.Exists() || ped.IsExplosionProof || ped.IsPlayer) continue;
                if (!Glob.Parse(pattern, globOptions).IsMatch(ped.Model.Name)) continue;
                    ped.Kill();
                    if (makePersistent) ped.MakePersistent();
                }
        }
        private class FPSDisplay
        {
            public bool Enabled = false;
            public string Text = $"GTA V v{Game.ProductVersion} (Build {Game.BuildNumber})\n\nRes: {Game.Resolution}";
            public PointF Position = new PointF(2, 2);
            // public SizeF Size = new SizeF(50, 50);
            // public RectangleF Rectangle = new RectangleF(fpsDisplay.Position, fpsDisplay.Size);
            public Color Color = Color.White;
            public int Peds, Vehicles, Objects = 0;

        }
        private static FPSDisplay fpsDisplay;
        [ConsoleCommand]
        private static void ToggleFPS() {
            if (fpsDisplay is null) fpsDisplay = new FPSDisplay();
            if (fpsDisplay.Enabled) {
                Game.RawFrameRender -= OnRawFrameRender;
                // Game.FrameRender -= OnFrameRender;
            } else {
                Game.RawFrameRender += OnRawFrameRender;
                // Game.FrameRender += OnFrameRender;
            }
            fpsDisplay.Enabled ^= true;
        }

        private static void OnFrameRender(object sender, GraphicsEventArgs e) {
        }
       
        private static void OnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            if (fpsDisplay.Enabled) {
                if (Game.TickCount % 10 == 0) {
                    fpsDisplay.Peds = World.GetAllPeds().Length;
                    fpsDisplay.Vehicles = World.GetAllVehicles().Length;
                    fpsDisplay.Objects = World.GetAllObjects().Length;
                }
                 // Weather: {Enum.GetName(typeof(WeatherType), World.Weather)}
                var text =$@"
FPS: {Game.FrameRate}
FrameTime: {Game.FrameTime}
GameTime: {Game.GameTime}
Frame: {Game.FrameCount}
Tick: {Game.TickCount}

Time: {World.DateTime} {(World.IsTimeOfDayFrozen ? "(Frozen)" : "")} ({Game.TimeScale}x)

Max Stars: {Game.MaxWantedLevel}

Peds: {fpsDisplay.Peds} / {World.PedCapacity}
Vehicles: {fpsDisplay.Vehicles} / {World.VehicleCapacity}
Objects: {fpsDisplay.Objects} / {World.ObjectCapacity}
Fires: {World.NumberOfFires}
";
                e.Graphics.DrawText(fpsDisplay.Text + text, "", 10, fpsDisplay.Position, fpsDisplay.Color);
            }
        }

        [ConsoleCommand]
        private static void Persist() {
            var entities = new List<Entity>();
            entities.AddRange(World.GetAllPeds().Cast<Entity>().ToList());
            entities.AddRange(World.GetAllVehicles().Cast<Entity>().ToList());
            foreach(var entity in entities) {
                if (!entity.Exists()) continue;

            }
         }
        [ConsoleCommand]
        private static void Command_EMP() {
            foreach (var vehicle in World.GetAllVehicles()) {
                if (!vehicle.Exists()) continue;
                vehicle.IsTaxiLightOn = false;
                vehicle.IsInteriorLightOn = false;
                vehicle.IndicatorLightsStatus = VehicleIndicatorLightsStatus.Both;
                /*var elights = vehicle.EmergencyLighting.Clone();
                elights.Lights.ToList().ForEach(l => l.Light = false);
                vehicle.EmergencyLightingOverride = elights;*/
                vehicle.IsEngineOn = false;
                vehicle.IsEngineStarting = false;
                // vehicle.LockStatus = VehicleLockStatus.Locked;
                vehicle.BurstTires();
                vehicle.Health = 1;
                vehicle.FuelTankHealth = 699.0f;
                vehicle.EngineHealth = 0.1f;
            }
            var peds = World.GetAllPeds();
            foreach (var ped in peds) {
                if (!ped.Exists() || ped.IsPlayer) continue;
                ped.IsRagdoll = true;
            }
            GameFiber.Sleep(500);
            foreach (var ped in World.GetAllPeds()) {
                if (!ped.Exists()) continue;
                ped.IsRagdoll = false;
            }
        }
        [ConsoleCommand]
        private static void Command_BurnAll(string pattern = "*") {
            var globOptions = new GlobOptions(); globOptions.Evaluation.CaseInsensitive = true;
            var entities = new List<Entity>();
            entities.AddRange(World.GetAllPeds().Cast<Entity>().ToList());
            entities.AddRange(World.GetAllVehicles().Cast<Entity>().ToList());
            var set = 0;
            foreach(var entity in entities) {
                if (!entity.Exists()) continue;
                entity.IsFireProof = false;
                if (entity is Rage.Ped ped && ped.IsPlayer) continue;
                if (Glob.Parse(pattern, globOptions).IsMatch(entity.Model.Name)) {
                    entity.IsOnFire = true; set++;
                }
            }
            Game.DisplayNotification($"Set {set} / {entities.Count} things on fire");
        }
        // private static void Command_Repair() => Command_Fix();
        [ConsoleCommand]
        private static void Command_Fix() {
            Rage.Vehicle currentVehicle = Game.LocalPlayer.Character.CurrentVehicle;
			if (currentVehicle.Exists()) {
                currentVehicle.Repair();
                Game.DisplayNotification("Vehicle repaired!");
            } else {
                Game.DisplayNotification("Not in a vehicle!");
            }
        }
        [ConsoleCommand]
        private static void Command_CleanAll() {
            World.CleanWorld(true, true, true, true, true, false);
            Game.DisplayNotification($"Cleaned World!");
        }
        [ConsoleCommand]
        private static void Command_TeleportToFirstBlip() {
            var blip = TeleportToBlip();
            if (blip is null) Game.DisplayNotification("No blip found!");
            else Game.DisplayNotification("Teleported player to nearest blip.");
        }
        [ConsoleCommand]
        private static void Command_TeleportToFirstEnemy() {
            var blip = TeleportToBlip(new BlipSprite[] { BlipSprite.Enemy, BlipSprite.Enemy3, BlipSprite.Enemy3, BlipSprite.Enemy4 });
            if (blip is null) Game.DisplayNotification("No enemy found!");
            else Game.DisplayNotification("Teleported player to nearest enemy.");
        }
        [ConsoleCommand("Throw a exception")]
        private static void Command_Exception(string message = "", bool confirm = false) {
            if (!confirm) throw new ArgumentOutOfRangeException("confirm");
            throw new Exception(message);
        }
        [ConsoleCommand("Internal testing command")]
        private static void Command_Bluscream() {
            Game.LocalPlayer.Character.Armor = Game.LocalPlayer.Character.Health = 100;
			if (Game.LocalPlayer.Character.CurrentVehicle.Exists()) Command_Fix();
			Game.LocalPlayer.Character.ClearBlood(); Game.LocalPlayer.Character.ClearWetness();
            Game.MaxWantedLevel = Game.LocalPlayer.WantedLevel = 0;
            var now = DateTime.Now;
            World.TimeOfDay = now.TimeOfDay;
            World.DateTime = now;
            Command_RevealMap(true);
            Game.LocalPlayer.Model = "S_M_Y_COP_01"; // 1581098148
            Game.LocalPlayer.Character.setVariation(Ped.Component.Items_tasks, 1, Ped.TextureID.a);
            Game.LocalPlayer.Character.setVariation(Ped.Component.Accessories, 1, Ped.TextureID.a);
            Game.LocalPlayer.Character.setVariation(Ped.Component.Decals, 1, Ped.TextureID.b);
            // Game.LocalPlayer.Character.GiveHelmet(false, HelmetTypes.FiremanHelmet, 9);
            GiveWeapon("WEAPON_FLASHLIGHT", 0);
            GiveWeapon("WEAPON_NIGHTSTICK", 0);
            GiveWeapon("WEAPON_FIREEXTINGUISHER", 1000);
            GiveWeapon("WEAPON_DIGISCANNER", 0);

            GiveWeapon("WEAPON_FLARE", 10);
            GiveWeapon("WEAPON_BZGAS", 2);

            GiveWeapon("WEAPON_PISTOL", 312);
            GiveWeapon("WEAPON_PUMPSHOTGUN", 68);
            GiveWeapon("WEAPON_SNIPERRIFLE", 15);
        }
#endregion
#region Methods
        private static void GiveWeapon(string weapon, short ammo = 0, bool equip = false, PedInventory inventory = null) {
            if (inventory is null) inventory = Game.LocalPlayer.Character.Inventory;
			inventory.GiveNewWeapon(new WeaponAsset(weapon), ammo, equip);
        }
        private static Blip TeleportToBlip(BlipSprite[] blipTypes = null) {
        Blip foundBlip = new Blip(new Vector3());
        foreach (Blip blip in World.GetAllBlips()) {
            if (blipTypes is null || blipTypes.Contains(blip.Sprite)) {
                foundBlip = blip;
                break;
            }
        }
        if (foundBlip.Position != Vector3.Zero) {
            var newPos = foundBlip.Position;
            bool flag3 = false;
            for (int j = 0; j <= 800; j += 50)
            {
                Game.LocalPlayer.Character.Position = new Vector3(foundBlip.Position.X, foundBlip.Position.Y, (float)j);
                bool flag4 = NativeFunction.CallByName<bool>("GET_GROUND_Z_FOR_3D_COORD", new NativeArgument[] {
                        foundBlip.Position.X,
                        foundBlip.Position.Y,
                        (float)j,
                        foundBlip.Position.Z,
                        false
                });
                if (flag4) {
                    flag3 = true;
                    newPos.Z += 3f;
                    break;
                }
            }
            if (!flag3) {
                newPos.Z = 1000f;
            }
            Game.LocalPlayer.Character.Position = newPos;
            return foundBlip;
        } else {
            return null;
        }
    }
        private static void Log(params object[] messages)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var msgs = messages.Select(f => f.ToString()).ToList();
            Game.Console.Print($"[{DateTime.Now}] {assembly.FullName}: {string.Join(" ", msgs)}");
        }
#endregion
#region Events
        public static void Main()
        {
            Log("Plugin loaded.");
            GameFiber.Hibernate();
        }
        public static void Shutdown()
        {
            // Game.FrameRender -= OnFrameRender;
            if (fpsDisplay.Enabled) Game.RawFrameRender -= OnRawFrameRender;
            Log("Plugin unloaded.");
        }
#endregion
    }
}
