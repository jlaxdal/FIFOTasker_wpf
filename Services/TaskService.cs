using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class TaskService
{
    private readonly AppDbContext _context = new();

    public TaskItem? GetActiveTask()
    {
        return _context.Tasks
            .Where(t => t.Finished == null)
            .OrderBy(t => t.QueueOrder)
            .FirstOrDefault();
    }

    public int GetUnfinishedCount()
    {
        return _context.Tasks.Count(t => t.Finished == null);
    }

    public List<TaskItem> GetUnfinishedTasks(int limit = 10)
    {
        return _context.Tasks
            .Where(t => t.Finished == null)
            .OrderBy(t => t.QueueOrder)
            .Take(limit)
            .ToList();
    }

    public List<TaskItem> GetAllTasks()
    {
        return _context.Tasks
            .OrderBy(t => t.Finished.HasValue ? 1 : 0)
            .ThenBy(t => t.QueueOrder)
            .ThenByDescending(t => t.Finished)
            .ToList();
    }

    public void AddTask(string summary, string? detailedDescription)
    {
        var maxOrder = _context.Tasks.Any()
            ? _context.Tasks.Max(t => t.QueueOrder) + 1
            : 0;

        var task = new TaskItem
        {
            Summary = summary,
            DetailedDescription = detailedDescription,
            QueueOrder = maxOrder,
            Added = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        _context.SaveChanges();
    }

    public void FinishCurrentTask()
    {
        var active = GetActiveTask();
        if (active == null) return;

        active.Finished = DateTime.UtcNow;
        _context.SaveChanges();
    }

    public void EscalateTask(int taskId)
    {
        var taskToEscalate = _context.Tasks.Find(taskId);
        if (taskToEscalate == null || taskToEscalate.Finished.HasValue) return;

        var currentActive = GetActiveTask();

        if (currentActive != null && currentActive.Id != taskId)
        {
            // Simple re-order: give it the lowest queue order
            var minOrder = _context.Tasks
                .Where(t => t.Finished == null)
                .Min(t => t.QueueOrder);

            taskToEscalate.QueueOrder = minOrder - 1;
            _context.SaveChanges();
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}