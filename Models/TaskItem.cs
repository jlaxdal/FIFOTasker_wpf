using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TaskItem
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Summary { get; set; } = string.Empty;

    public string? DetailedDescription { get; set; }

    public DateTime Added { get; set; } = DateTime.UtcNow;

    public DateTime? Finished { get; set; }

    // For ordering: lower number = earlier in queue
    public int QueueOrder { get; set; }

    // Runtime-only property for UI (NOT saved to DB)
    [NotMapped]
    public bool IsNotActive { get; set; } = true;
}