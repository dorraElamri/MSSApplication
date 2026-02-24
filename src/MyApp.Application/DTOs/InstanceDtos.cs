using System;
using MyApp.Domain.Entities;

namespace MyApp.Application.DTOs;

// CreateInstanceDto.cs
public class CreateInstanceDto
{
    public string Host { get; set; }
    public string? ApplicationName { get; set; }
    public string? Description { get; set; }
    public string? LogPath { get; set; }
    public EnvironmentType Environment { get; set; }

}

// UpdateInstanceDto.cs
public class UpdateInstanceDto
{
    public string? ApplicationName { get; set; }
    public string? Host { get; set; }
    public string? Description { get; set; }
    public string? LogPath { get; set; }
    public EnvironmentType? Environment { get; set; }
    public bool? IsActive { get; set; }
}


