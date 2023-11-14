using System;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using System.IO;
using GTA.Native;

public class VehicleJump : Script
{
    private bool jumpKeyPressedRecently = false;
    private float jumpHeight = 20.0f; // Default jump height
    private Keys jumpKey = Keys.L; // Default jump key
    private bool allowJumpInCars = true; // Default setting
    private bool allowJumpInPlanes = true; // Default setting
    private bool allowJumpInHelicopters = true; // Default setting
    private bool allowJumpInBikes = true; // Default setting
    private bool allowJumpInBoats = true; // Default setting

    public VehicleJump()
    {
        LoadSettings(); // Load settings from the INI file
        Tick += OnTick;
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
    }

    private void LoadSettings()
    {
        string iniPath = "scripts\\VehicleJump.ini";

        if (File.Exists(iniPath))
        {
            ScriptSettings settings = ScriptSettings.Load(iniPath);
            jumpHeight = settings.GetValue("Settings", "JumpHeight", 20.0f);
            jumpKey = settings.GetValue("Settings", "JumpKey", Keys.L);
            allowJumpInCars = settings.GetValue("Settings", "AllowJumpInCars", true);
            allowJumpInPlanes = settings.GetValue("Settings", "AllowJumpInPlanes", true);
            allowJumpInHelicopters = settings.GetValue("Settings", "AllowJumpInHelicopters", true);
            allowJumpInBikes = settings.GetValue("Settings", "AllowJumpInBikes", true);
            allowJumpInBoats = settings.GetValue("Settings", "AllowJumpInBoats", true);
        }
        else
        {
            // Create a default INI file with default values
            ScriptSettings settings = ScriptSettings.Load(iniPath);
            settings.SetValue("Settings", "JumpHeight", jumpHeight);
            settings.SetValue("Settings", "JumpKey", jumpKey);
            settings.SetValue("Settings", "AllowJumpInCars", allowJumpInCars);
            settings.SetValue("Settings", "AllowJumpInPlanes", allowJumpInPlanes);
            settings.SetValue("Settings", "AllowJumpInHelicopters", allowJumpInHelicopters);
            settings.SetValue("Settings", "AllowJumpInBikes", allowJumpInBikes);
            settings.SetValue("Settings", "AllowJumpInBoats", allowJumpInBoats);
            settings.Save();
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        if (jumpKeyPressedRecently)
        {
            jumpKeyPressedRecently = false;
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == jumpKey && !jumpKeyPressedRecently)
        {
            Jump();
            jumpKeyPressedRecently = true;
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        // Additional key release logic if needed
    }

    private void Jump()
    {
        Ped playerPed = Game.Player.Character;
        if (playerPed.IsInVehicle())
        {
            Vehicle currentVehicle = playerPed.CurrentVehicle;
            if (currentVehicle != null && currentVehicle.IsOnAllWheels)
            {
                if (CanJumpInVehicleType(currentVehicle))
                {
                    currentVehicle.ApplyForce(Vector3.WorldUp * jumpHeight);
                }
            }
        }
    }

    private bool CanJumpInVehicleType(Vehicle vehicle)
    {
        VehicleHash vehicleHash = (VehicleHash)vehicle.Model.Hash;
        VehicleClass vehicleClass = Function.Call<VehicleClass>(Hash.GET_VEHICLE_CLASS, vehicle);

        switch (vehicleClass)
        {
            case VehicleClass.Planes:
                return allowJumpInPlanes;

            case VehicleClass.Helicopters:
                return allowJumpInHelicopters;

            case VehicleClass.Motorcycles:
                return allowJumpInBikes;

            case VehicleClass.Boats:
                return allowJumpInBoats;

            default:
                return allowJumpInCars;
        }
    }
}
