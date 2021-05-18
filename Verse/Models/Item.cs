using System;

namespace Verse.Models
{
    public class Item
    {
        public int Id { get; init; }
        public string IconName { get; init; } = "";
        public string GroupName { get; init; } = "";
        public string Name { get; init; } = "";
        public string ImageName { get; init; } = "";
        public string[] GroupNameWords { get; init; } = Array.Empty<string>();
        public string[] MainWords { get; init; } = Array.Empty<string>();
        public string[] AdditionalWords { get; init; } = Array.Empty<string>();
    }
}