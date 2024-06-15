﻿using ADB_Explorer.ViewModels;
using static ADB_Explorer.Models.AdbExplorerConst;

namespace ADB_Explorer.Models;

public abstract class AbstractDrive : ViewModelBase
{
    public enum DriveType
    {
        Root,
        Internal,
        Expansion,
        External,
        Emulated,
        Unknown,
        Trash,
        Temp,
        Package,
    }

    private DriveType type = DriveType.Unknown;
    public DriveType Type
    {
        get => type;
        set => Set(ref type, value);
    }


    public static implicit operator bool(AbstractDrive obj)
    {
        return obj is not null;
    }
}

public class Drive : AbstractDrive
{
    public string Path { get; protected set; }

    /// <summary>
    /// Filesystem in USEr space. An emulated / virtual filesystem on Android.<br /><br />
    /// Does not support:<br />
    /// • Symbolic links<br />
    /// • Special chars in file name (like NTFS)<br />
    /// • Installing APK from it
    /// </summary>
    public virtual bool IsFUSE { get; }


    public Drive(string path = "")
    {
        Path = path;

        if (Type is DriveType.Unknown && DRIVE_TYPES.TryGetValue(path, out var type))
        {
            Type = type;
            if (Type is DriveType.Internal)
                Path = "/sdcard";
        }
    }
}
