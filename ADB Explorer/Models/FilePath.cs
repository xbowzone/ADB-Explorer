﻿using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using static ADB_Explorer.Converters.FileTypeClass;
using static ADB_Explorer.Services.ADBService;

namespace ADB_Explorer.Models
{
    public enum PathType
    {
        Android,
        Windows,
    }

    public class FilePath : INotifyPropertyChanged
    {
        public PathType PathType { get; protected set; }

        protected bool IsRegularFile { private get; set; }
        public bool IsDirectory { get; protected set; }

        public string FullPath { get; protected set; }
        public string ParentPath => FullPath[..(FullPath.LastIndexOf(PathSeparator()) is int index && index > 0 ? index : ^0)];
        public string FullName { get; protected set; }
        public string NoExtName
        {
            get
            {
                if (IsDirectory || !IsRegularFile || HiddenOrWithoutExt(FullName))
                    return FullName;
                else
                    return FullName[..FullName.LastIndexOf('.')];
            }
        }

        public readonly AdbDevice Device; // will be left null for PC

        public FilePath(ShellObject windowsPath)
        {
            PathType = PathType.Windows;
            
            FullPath = windowsPath.ParsingName;
            FullName = windowsPath.Name;
            IsDirectory = windowsPath is ShellFolder;
            IsRegularFile = !IsDirectory;
        }

        public FilePath(string androidPath,
                        string fullName = "",
                        FileType fileType = FileType.File,
                        AdbDevice device = null)
        {
            PathType = PathType.Android;

            FullPath = androidPath;
            FullName = string.IsNullOrEmpty(fullName) ? GetFullName(androidPath) : fullName;
            IsDirectory = fileType == FileType.Folder;
            IsRegularFile = fileType == FileType.File;

            Device = device;
        }

        private string GetFullName(string fullPath) =>
            fullPath[(fullPath.LastIndexOf(PathSeparator()) + 1)..];

        private static bool HiddenOrWithoutExt(string fullName) => fullName.Count(c => c == '.') switch
        {
            0 => true,
            1 when fullName.StartsWith('.') => true,
            _ => false,
        };

        private char PathSeparator() => PathSeparator(PathType);

        private static char PathSeparator(PathType pathType) => pathType switch
        {
            PathType.Windows => '\\',
            PathType.Android => '/',
            _ => throw new NotSupportedException(),
        };

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString()
        {
            return FullName;
        }
    }
}