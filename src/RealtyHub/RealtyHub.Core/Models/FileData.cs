﻿namespace RealtyHub.Core.Models;

public class FileData
{
    public string Id { get; set; } = string.Empty;
    public bool IsThumbnail { get; set; }
    public byte[] Content { get; set; } = [];
    public string ContentType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}