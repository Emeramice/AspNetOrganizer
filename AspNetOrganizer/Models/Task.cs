using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetOrganizer.Models
{
    public enum TaskPriority
    {
        Low,
        Normal,
        High
    }

    public class ToDoTask
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime DueDateTime { get; set; }
        public string Comment { get; set; }
        public bool IsCompleted { get; set; }
    }
}