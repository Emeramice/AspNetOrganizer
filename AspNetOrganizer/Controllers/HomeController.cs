using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AspNetOrganizer.Models;
using System.IO;
using System.Data.SQLite;

namespace AspNetOrganizer.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (!System.IO.File.Exists(Server.MapPath("~/") + "TaskBase.db"))
            {
                System.IO.File.Create(Server.MapPath("~/") + "TaskBase.db");
            }
            using (SQLiteConnection taskBaseConnection = new SQLiteConnection("Data source =" + Server.MapPath("~/") + "TaskBase.db"))
            {
                taskBaseConnection.Open();
                using (SQLiteCommand createTableCommand = taskBaseConnection.CreateCommand())
                {
                    createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS ToDoTasks" +
                        "(task_id INTEGER PRIMARY KEY AUTOINCREMENT," +
                        "task_name TEXT NOT NULL," +
                        "priority INTEGER NOT NULL," +
                        "date_time_ticks INTEGER NOT NULL," +
                        "comment TEXT," +
                        "is_completed INTEGER NOT NULL);";
                    createTableCommand.ExecuteNonQuery();
                }
                ShowAllTasks(taskBaseConnection);
            }
            return View(ViewBag.Tasks);
        }

        private void ShowAllTasks(SQLiteConnection taskBaseConn)
        {
            using (SQLiteCommand viewTasksCommand = taskBaseConn.CreateCommand())
            {
                List<ToDoTask> tasksInDb = new List<ToDoTask>();
                viewTasksCommand.CommandText = "SELECT task_name, date_time_ticks, is_completed FROM ToDoTasks;";
                SQLiteDataReader tasksReader = viewTasksCommand.ExecuteReader();
                while (tasksReader.Read())
                {
                    ToDoTask currentTask = new ToDoTask();
                    currentTask.TaskName = tasksReader.GetString(0);
                    currentTask.DueDateTime = new DateTime(tasksReader.GetInt64(1));
                    currentTask.IsCompleted = (tasksReader.GetInt32(2) == 1);
                    tasksInDb.Add(currentTask);
                }
                ViewBag.Tasks = tasksInDb;
            }
        }

        public ActionResult NewTaskBtnClick(ToDoTask model)
        {
            return View("NewTask");
        }

        public ActionResult SaveTaskBtnClick(ToDoTask model)
        {
            using (SQLiteConnection taskBaseConnection = new SQLiteConnection("Data source =" + Server.MapPath("~/") + "TaskBase.db"))
            {
                taskBaseConnection.Open();
                using (SQLiteCommand insertTaskCommand = taskBaseConnection.CreateCommand())
                {
                    insertTaskCommand.CommandText = "INSERT INTO ToDoTasks (task_name, priority, date_time_ticks, comment, is_completed)" +
                        "VALUES ('" + model.TaskName + "', " + (int)model.Priority + ", " +
                        model.DueDateTime.Ticks + ", '" + model.Comment + "', 0);";
                    insertTaskCommand.ExecuteNonQuery();
                }
                ShowAllTasks(taskBaseConnection);
            }
            return View("Index", ViewBag.Tasks);
        }

        public ActionResult CompleteChanged(List<ToDoTask> models)
        {
            using (SQLiteConnection taskBaseConnection = new SQLiteConnection("Data source =" + Server.MapPath("~/") + "TaskBase.db"))
            {
                taskBaseConnection.Open();
                for (int i = 0; i < models.Count; i++)
                {
                    using (SQLiteCommand insertTaskCommand = taskBaseConnection.CreateCommand())
                    {
                        int isCompleted = models[i].IsCompleted ? 1 : 0;
                        insertTaskCommand.CommandText = "UPDATE ToDoTasks SET is_completed=" + isCompleted + " WHERE task_id=" + i + ";";
                        insertTaskCommand.ExecuteNonQuery();
                    }
                }
            }
            return View("Index",models);
        }
    }
}